using System;
using System.Collections.Generic;
using UniLinq;
using UniRx;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;
using CameraShakeByActionType = Noroshi.BattleScene.Actions.CameraShakeByActionType;

namespace Noroshi.BattleScene
{
    /// ウェーブ単位のバトルロジックを扱うクラス。
    /// Dispose 時に加え、ウェーブバトル終了時にも内包している Subject を Oncompleted し、
    // 今後 OnNext で外部処理が走らないようにしている。
    public class Wave
    {
        /// サーバーから取得したウェーブ情報。
        BattleWave _data;
        Dictionary<Character, bool> _subscribedCharacterMap = new Dictionary<Character, bool>();
        /// 時間停止試行（時間停止の中で動いている）キャラクター。
        List<Character> _timeStoppers = new List<Character>();
        /// 時間停止キャラクター。
        List<Character> _timeStoppings = new List<Character>();
        /// ウェーブバトル終了時に結果が OnNext される Subject。
        Subject<VictoryOrDefeat> _onFinishBattleSubject = new Subject<VictoryOrDefeat>();
        /// カウントダウン（1秒毎）で残り時間が OnNext される Subject。
        Subject<byte> _onCountDownSubject = new Subject<byte>();
        /// キャラクター死亡時に死亡キャラクターが OnNext される Subject。
        Subject<Character> _onCharacterDieSubject = new Subject<Character>();
        /// キャラクターエフェクト操作時に操作内容が OnNext される Subject。
        Subject<CharacterEffectEvent> _onCommandCharacterEffectSubject = new Subject<CharacterEffectEvent>();
        /// サウンド操作時に操作内容が OnNext される Subject。
        Subject<SoundEvent> _onCommandSoundSubject = new Subject<SoundEvent>();
        /// フィールド暗転時に true が、暗転解除時に false が OnNext される Subject。
        Subject<bool> _onDarkenFieldSubject = new Subject<bool>();
        /// カメラ操作時に操作内容が OnNext される Subject。
        Subject<CameraShakeByActionType> _onTryCameraShakeSubject = new Subject<CameraShakeByActionType>();
        /// タイマー以外の Disposable を格納する CompositeDisposable。
        CompositeDisposable _disposables;
        /// タイマー用 Disposable。
        IDisposable _timerDisposable;

        /// ウェーブ番号。
        public readonly byte No;
        /// フィールド。
        public readonly WaveField Field;
        /// タイムアップまでの残り時間。
        public byte RemainingTime { get; private set; }

        /// サーバからの情報とウェーブ番号でインスタンス化。
        public Wave(BattleWave data, byte no)
        {
            _data = data;
            No = no;
            Field = new WaveField(no);
            _disposables = new CompositeDisposable();
        }

        /// 破棄する。
        public void Dispose()
        {
            _onCompletedSubjectsWithoutOnFinishBattle();
            _onFinishBattleSubject.OnCompleted();
            Field.Clear();
            _disposables.Dispose();
            _timerDisposable.Dispose();
        }
        void _onCompletedSubjectsWithoutOnFinishBattle()
        {
            _onCountDownSubject.OnCompleted();
            _onCharacterDieSubject.OnCompleted();
            _onCommandCharacterEffectSubject.OnCompleted();
            _onCommandSoundSubject.OnCompleted();
            _onDarkenFieldSubject.OnCompleted();
            _onTryCameraShakeSubject.OnCompleted();
        }

        /// ウェーブバトル終了時に結果が OnNext される Observable を取得。
        public IObservable<VictoryOrDefeat> GetOnFinishBattleObservable()
        {
            return _onFinishBattleSubject.AsObservable();
        }
        /// カウントダウン（1秒毎）で残り時間が OnNext される Observable を取得。
        public IObservable<byte> GetOnCountDownObservable()
        {
            return _onCountDownSubject.AsObservable();
        }
        /// キャラクター死亡時に死亡キャラクターが OnNext される Observable を取得。
        public IObservable<Character> GetOnCharacterDieObservable()
        {
            return _onCharacterDieSubject.AsObservable();
        }
        /// キャラクターエフェクト操作時に操作内容が OnNext される Observable を取得。
        public IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable()
        {
            return _onCommandCharacterEffectSubject.AsObservable();
        }
        /// サウンド操作時に操作内容が OnNext される Observable を取得。
        public IObservable<SoundEvent> GetOnCommandSoundObservable()
        {
            return _onCommandSoundSubject.AsObservable();
        }
        /// フィールド暗転時に true が、暗転解除時に false が OnNext される Observable を取得。
        public IObservable<bool> GetOnDarkenFieldObservable()
        {
            return _onDarkenFieldSubject.AsObservable();
        }
        /// カメラ操作時に操作内容が OnNext される Observable を取得。
        public IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable()
        {
            return _onTryCameraShakeSubject.AsObservable();
        }

        /// Wave を残り時間指定で開始する。
        public void Start(byte remainingTime)
        {
            // 残り時間セット。
            RemainingTime = remainingTime;
            // カウントダウン処理（初期時間更新の OnNext 付き）
            _onCountDownSubject.OnNext(RemainingTime);
            _timerDisposable = SceneContainer.GetTimeHandler().Interval(1).Subscribe(_ => _onTimer());

            // フィールド内に配置されている（生存）キャラクターから各 Observable を取得して処理を紐付ける。
            // そのまま該当 Subject を OnNext するものと、内部処理を呼ぶものが存在する。
            _subscribedCharacterMap.Clear();
            foreach (var character in Field.GetAllCharacters())
            {
                _subscribeCharacterObservables(character);
            }
            // ロジック開始
            foreach (var character in Field.GetAllCharacters())
            {
                character.Start();
            }
            // 開始時に発動するアクションがあれば発動
            foreach (var character in Field.GetAllCharacters())
            {
                character.TryToInvokeFirstAction();
            }
        }
        /// キャラクターから必要な Observable を取得して処理を紐付ける。
        void _subscribeCharacterObservables(Character character)
        {
            // 重複処理防止。
            if (_subscribedCharacterMap.ContainsKey(character)) return;
            _subscribedCharacterMap.Add(character, true);

            character.GetOnDieObservable()
            .Subscribe(_onCharacterDie).AddTo(_disposables);

            character.GetOnEnterActiveActionObservable()
            .SelectMany(c => _onCharacterEnterActiveAction(c))
            .Subscribe().AddTo(_disposables);

            character.GetOnCommandCharacterEffectObservable()
            .Subscribe(_onCommandCharacterEffectSubject.OnNext).AddTo(_disposables);

            character.GetOnCommandSoundObservable()
            .Subscribe(_onCommandSoundSubject.OnNext).AddTo(_disposables);

            character.GetOnTryCameraShakeObservable()
            .Subscribe(_onTryCameraShakeSubject.OnNext).AddTo(_disposables);
        }
        public void SetShadowCharacter(ShadowCharacter shadow)
        {
            _subscribeCharacterObservables(shadow);
            Field.SetShadowCharacter(shadow);
        }
        /// 毎秒処理。
        void _onTimer()
        {
            // 残り時間デクリメント。
            RemainingTime--;
            _onCountDownSubject.OnNext(RemainingTime);
            // タイムアップ時。
            if (_timeIsUp())
            {
                _finishBattle(VictoryOrDefeat.TimeUp);
            }
            else
            {
                // （UI のための）アクティブアクション実行可否チェックも定期的にやりたいのでここで実行。
                foreach (var character in Field.GetAllCharacters())
                {
                    character.CheckActiveActionAvailable();
                }
            }
        }
        /// キャラクター死亡時処理。
        void _onCharacterDie(Character character)
        {
            if (_timeStoppers.Contains(character))
            {
                _restartTime(character);
            }
            _onCharacterDieSubject.OnNext(character);
            // まずは死亡キャラクターをフィールドから取り除く。
            Field.RemoveCharacter(character);

            foreach (var executor in Field.GetAllCharacters())
            {
                if (executor.Force != character.Force)
                {
                    executor.TryToExecuteEnemyDeadActionDirectly();
                }
            }
            // 分身キャラクターはここまで。
            if (character.GetType() == typeof(ShadowCharacter)) return;

            // 勝敗判定
            _judge();
        }
        /// アクティブアクション発動時処理。
        IObservable<Character> _onCharacterEnterActiveAction(Character character)
        {
            return _stopTime(character, character.GetOnExitTimeStopObservable());
        }
        /// ウェーブバトル終了時処理。
        void _finishBattle(VictoryOrDefeat victoryOrDefeat)
        {
            foreach (var timeStopper in new List<Character>(_timeStoppers))
            {
                _restartTime(timeStopper);
            }
            // Attribute を外し、分身も消す。
            SceneContainer.GetCharacterManager().RemoveAttributeAndShadow();
            // バトル終了時処理以外の外部処理が走らないようにする。
            _onCompletedSubjectsWithoutOnFinishBattle();
            // 内部処理も走らないようにする。
            _disposables.Dispose();
            // タイマーを止める。
            _timerDisposable.Dispose();
            // 外部処理のために勝敗を OnNext する。
            _onFinishBattleSubject.OnNext(victoryOrDefeat);
            _onFinishBattleSubject.OnCompleted();
        }

        /// 勝敗判定。
        void _judge()
        {
            if (_hasWonCompletely())
            {
                _finishBattle(VictoryOrDefeat.Win);
            }
            else if (_hasLostCompletely())
            {
                _finishBattle(VictoryOrDefeat.Loss);
            }
            else if (_hasDrawn())
            {
                _finishBattle(VictoryOrDefeat.Draw);
            }
        }
        /// 完全勝利。
        bool _hasWonCompletely()
        {
            var characterManager = SceneContainer.GetCharacterManager();
            return characterManager.CurrentOwnCharacterSet.AreAlive() && !characterManager.CurrentEnemyCharacterSet.AreAlive();
        }
        /// 完全敗北。
        bool _hasLostCompletely()
        {
            var characterManager = SceneContainer.GetCharacterManager();
            return !characterManager.CurrentOwnCharacterSet.AreAlive() && characterManager.CurrentEnemyCharacterSet.AreAlive();
        }
        /// 引き分け
        bool _hasDrawn()
        {
            var characterManager = SceneContainer.GetCharacterManager();
            return !characterManager.CurrentOwnCharacterSet.AreAlive() && !characterManager.CurrentEnemyCharacterSet.AreAlive();
        }
        /// 時間切れ
        bool _timeIsUp()
        {
            var characterManager = SceneContainer.GetCharacterManager();
            return RemainingTime <= 0 && characterManager.CurrentOwnCharacterSet.AreAlive() && characterManager.CurrentEnemyCharacterSet.AreAlive();
        }

        /// キャラクターを初期位置に配置する。
        public void SetCharactersToInitialPosition()
        {
            var characterManager = SceneContainer.GetCharacterManager();
            var aliveOwnCharacters = characterManager.GetCurrentOwnCharacters().Where(c => !c.IsDead);
            var aliveEnemyCharacters = characterManager.GetCurrentEnemyCharacters().Where(c => !c.IsDead);
            _setCharactersToInitialGrid(aliveOwnCharacters, aliveEnemyCharacters);
            foreach (var character in aliveOwnCharacters)
            {
                character.SetViewToCorrectPosition();
            }
            foreach (var character in aliveEnemyCharacters)
            {
                character.SetViewToCorrectPosition();
            }
        }
        /// キャラクターを初期位置に配置する。自キャラクターは歩き、もしくは走りアニメーションで該当位置へ移動する。
        public IObservable<Wave> SetCharactersToInitialPositionWithAnimation(float duration, bool isWalking = false)
        {
            var aliveOwnCharacters = SceneContainer.GetCharacterManager().GetCurrentOwnCharacters().Where(c => !c.IsDead);
            var aliveEnemyCharacters = SceneContainer.GetCharacterManager().GetCurrentEnemyCharacters().Where(c => !c.IsDead);
            _setCharactersToInitialGrid(aliveOwnCharacters, aliveEnemyCharacters);
            foreach (var character in aliveEnemyCharacters)
            {
                character.SetViewToCorrectPosition();
            }
            return isWalking ? Field.SetCharactersToCorrectPositionWithWalking(aliveOwnCharacters, duration).Select(_ => this) : Field.SetCharactersToCorrectPositionWithRunnging(aliveOwnCharacters, duration).Select(_ => this);
        }
        public IObservable<Wave> RunOwnCharactersToCorrectPosition(float duration)
        {
            var aliveOwnCharacters = SceneContainer.GetCharacterManager().GetCurrentOwnCharacters().Where(c => !c.IsDead);
            return Field.SetCharactersToCorrectPositionWithRunnging(aliveOwnCharacters, duration).Select(_ => this);
        }

        void _setCharactersToInitialGrid(IEnumerable<Character> ownCharacters, IEnumerable<Character> enemyCharacters)
        {
            Field.SetOwnCharactersToInitialGrid(ownCharacters);
            Field.SetEnemyCharactersToInitialGrid(enemyCharacters);
        }

        /// 時間を停止する。
        IObservable<Character> _stopTime(Character timeStopper, IObservable<Character> onExitTimeStop)
        {
            // 最初の時間停止の場合、
            if (_timeStoppers.Count() == 0)
            {
                // 暗くして、
                _onDarkenFieldSubject.OnNext(true);
                // 他キャラクターを止める。
                foreach (var character in Field.GetAllCharacters().Where(c => c != timeStopper))
                {
                    character.PauseOn();
                    _timeStoppings.Add(character);
                }
            }
            // 他に時間停止を行っているキャラクターがいる場合、
            else
            {
                // 時間停止実行キャラクターは強制時間停止解除。
                _timeStoppings.Remove(timeStopper);
                timeStopper.PauseOff();
            }
            _timeStoppers.Add(timeStopper);
            return onExitTimeStop.Do(_restartTime);
        }
        /// 時間を再び動かす。
        void _restartTime(Character timeStopper)
        {
            _timeStoppers.Remove(timeStopper);

            // まだ他に時間停止を行っているキャラクターがいる場合、
            if (_timeStoppers.Count() > 0)
            {
                // 動く権利がなくなったので停止する。
                timeStopper.PauseOn();
                _timeStoppings.Add(timeStopper);
            }
            // 該当キャラクターしか時間停止を行っていない場合、
            else
            {
                // 他キャラクターの停止を解除し、
                foreach (var character in _timeStoppings)
                {
                    character.PauseOff();
                }
                _timeStoppings.Clear();
                // 明るくする。
                _onDarkenFieldSubject.OnNext(false);
            }
        }
    }
}
