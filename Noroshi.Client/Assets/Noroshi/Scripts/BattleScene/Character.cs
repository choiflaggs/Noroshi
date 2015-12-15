using System;
using System.Collections.Generic;
using Vector2 = UnityEngine.Vector2;
using UniLinq;
using UniRx;
using Noroshi.Core.Game.Character;
using Noroshi.Core.Game.Action;
using Noroshi.Grid;
using Noroshi.BattleScene.CharacterState;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene
{
    public enum Force
    {
        Own,
        Enemy
    }
    public enum SpecialEvent
    {
        Dodge,
    }
    /// キャラクターを表現するクラス。
    public abstract class Character : ViewModel<ICharacterView>, IActionExecutor, IActionTarget, IGridContent, CharacterActionEventHandler.ICharacter
    {
        /// 所属勢力（敵 or 味方）
        public Force Force { get; private set; }
        /// 一時的な所属勢力（敵 or 味方）
        public Force CurrentForce { get; private set; }
        /// キャラクター番号（同一キャラクターセットの中で一意に割り当てられる）
        byte? _no = null;
        /// サーバーから受け取った非マスターな情報。
        protected Core.WebApi.Response.Battle.BattleCharacter _battleCharacter;
        /// キャラクターステータス。
        CharacterStatus _characterStatus;
        /// HP
        public CharacterHP HP { get; private set; }
        /// エネルギー
        public CharacterEnergy Energy { get; private set; }
        /// 与ダメージ係数。レイドボスバトル時の BP 効果や、特攻に利用。
        public float? DamageCoefficient { get; private set; }
        /// フィールド上におけるグリッド座標（配置されていない場合は null）
        GridPosition? _gridPosition = null;
        /// フィールド上における向き（配置されていない場合は null）
        Direction? _direction = null;
        /// キャラクター状態遷移操作オブジェクト。
        readonly CharacterStateHandler _stateHandler = new CharacterStateHandler();
        /// キャラクター状態遷移ブロッカー。
        readonly StateTransitionBlocker _stateTransitionBlocker = new StateTransitionBlocker();
        /// 保有アクションを扱うオブジェクト。
        ActionHandler _actionHandler;
        /// アクションイベント操作オブジェクト。
        CharacterActionEventHandler _actionEventHandler;
        /// 通常アクション発動のためのゲージ。
        float _normalActionGaugeRatio = 0;
        /// 自動的にアクティブアクションを発動するかフラグ。
        bool _autoActiveAction = false;
        /// 分身処理用オブジェクト。
        public readonly ShadowHandler ShadowHandler = new ShadowHandler();
        /// 停止中フラグ。
        bool _pause = false;
        /// アクティブアクション発動可能時に true が、発動不可能時に false が OnNext される Subject。
        readonly Subject<bool> _onToggleActiveActionAvailable = new Subject<bool>();
        /// キャラクターエフェクト操作時に操作内容が OnNext される Subject。
        readonly Subject<CharacterEffectEvent> _onCommandCharacterEffect = new Subject<CharacterEffectEvent>();
        /// 時間停止解除時に解除キャラクターが OnNext される Subject。
        readonly Subject<Character> _onExitTimeStopSubject = new Subject<Character>();
        /// 死亡演出が完了した際に死亡キャラクターが OnNext される Subject。
        protected Subject<Character> _onExitDeadAnimationSubject;
        /// View 以外の disposables。
        protected readonly CompositeDisposable _disposablesWithoutView = new CompositeDisposable();
        /// View の disposables。
        protected readonly CompositeDisposable _disposablesForView = new CompositeDisposable();

        public Character()
        {
            _actionEventHandler = new CharacterActionEventHandler(this);
        }
        void _subscribeWithoutViewObservable()
        {
            _disposablesWithoutView.Clear();
            // 状態遷移イベント設定
            var stateTypeToActionMap = new Dictionary<Type, Action>
            {
                { typeof(CharacterStateHandler.IdleState), _idleOnEnter },
                { typeof(CharacterStateHandler.ProceedState), _proceedOnEnter },
                { typeof(CharacterStateHandler.ActionState), _actionOnEnter },
                { typeof(CharacterStateHandler.ActiveActionState), _activeActionOnEnter },
                { typeof(CharacterStateHandler.KnockbackState), _knockbackOnEnter },
                { typeof(CharacterStateHandler.DeadState), _deadOnEnter },
                { typeof(CharacterStateHandler.ApparentDeathState), _apparentDeathOnEnter },
            };
            _stateHandler.GetOnEnterStateObservable().Where(state => stateTypeToActionMap.ContainsKey(state.GetType()))
            .Subscribe(state => stateTypeToActionMap[state.GetType()].Invoke()).AddTo(_disposablesWithoutView);

            // アクティブアクション状態終了時には時間停止も終了するようにする。死亡などでアクティブアクションを正常に終了できなかった場合のため。
            _stateHandler.GetOnExitStateObservable().Where(state => state.GetType() == typeof(CharacterStateHandler.ActiveActionState))
            .Subscribe(_ => _onExitTimeStopSubject.OnNext(this)).AddTo(_disposablesWithoutView);

            // アクション系状態開始時に PreProcess を実行。
            _stateHandler.GetOnEnterActionObservable().Subscribe(_ => _actionHandler.PreProcess(this)).AddTo(_disposablesWithoutView);
            // アクション系状態終了時にキャンセル試行。
            _stateHandler.GetOnExitActionObservable().Subscribe(_ => _actionHandler.TryToCancel(this)).AddTo(_disposablesWithoutView);

            // 状態遷移によるキャラクターエフェクト命令を紐付ける。
            _stateHandler.GetOnCommandCharacterEffectObservable().Subscribe(e => _onCommandCharacterEffect.OnNext(e)).AddTo(_disposablesWithoutView);
            // アクションイベント操作オブジェクトによるキャラクターエフェクト命令を紐付ける。
            _actionEventHandler.GetOnCommandCharacterEffectObservable().Subscribe(e => _onCommandCharacterEffect.OnNext(e)).AddTo(_disposablesWithoutView);
        }
        public IObservable<Character> LoadDatas()
        {
            return _loadCharacterStatus()
            .Do(characterStatus =>
            {
                _characterStatus = characterStatus;
                _actionHandler   = new ActionHandler(_characterStatus.AvailableActionIDsWithZero, _characterStatus.ActionLevels.Select(al => (ushort)al).ToArray(), characterStatus.ActionSequence);
                HP     = new CharacterHP(_characterStatus.MaxHP, _characterStatus.HPRegen);
                Energy = new CharacterEnergy(_characterStatus.EnergyRegen);
                _applyInitialCondition();
            })
            .SelectMany(_ => _actionHandler.LoadDatas())
            .SelectMany(_ => _actionEventHandler.LoadData())
            .Select(_ => this);
        }
        /// キャラクターステータスをロードする。子クラス毎にロード内容を実装する。
        protected abstract IObservable<CharacterStatus> _loadCharacterStatus();
        /// サーバーからロードした初期設定を適用する。
        void _applyInitialCondition()
        {
            if (_battleCharacter == null) return;
            if (_battleCharacter.FixedMaxHP.HasValue) HP.ChangeMaxHP(_battleCharacter.FixedMaxHP.Value);
            if (_battleCharacter.InitialHP.HasValue) HP.ForceInitialize(_battleCharacter.InitialHP.Value);
            if (_battleCharacter.InitialEnergy.HasValue) Energy.ForceInitialize(_battleCharacter.InitialEnergy.Value);
            if (_battleCharacter.DamageCoefficient.HasValue) DamageCoefficient = _battleCharacter.DamageCoefficient.Value;
        }

        public virtual IObservable<Character> LoadAssets(IActionFactory actionFactory)
        {
            return LoadView().Do(_ =>
            {
                if (_direction.HasValue) _view.SetHorizontalDirection(_direction.Value);
            })
            .SelectMany(v => _actionHandler.LoadAssets(this, actionFactory, v.GetActionAnimations()))
            .Select(_ => this);
        }
        protected override IObservable<ICharacterView> _loadView()
        {
            return SceneContainer.GetFactory().BuildCharacterView(CharacterID).Do(view => view.SetSkin(_characterStatus.SkinLevel));
        }
        void _initializeView()
        {
            _disposablesForView.Clear();
            // 状態遷移操作オブジェクトへ View を引き渡し。
            _stateHandler.SetCharacterView(_view);
            // View に方向を反映。
            _view.SetHorizontalDirection(_direction.Value);
            // View に仕込んである時間停止解除トリガーに本クラス内の Subject を紐付ける。
            _view.GetOnExitTimeStopObservable().Subscribe(_ => _onExitTimeStopSubject.OnNext(this)).AddTo(_disposablesForView);
            // アクションアニメーション終了時に該当処理を紐付ける。
            _view.GetOnExitActionAnimationObservable().Subscribe(_onExitActionAnimation).AddTo(_disposablesForView);
            // アクションアニメーションに仕込んであるアクション実行トリガーに該当処理を紐付ける。
            _view.GetOnInvokeActionObservable().Subscribe(_ => _onInvokeAction()).AddTo(_disposablesForView);
        }
        void _onExitActionAnimation(bool success)
        {
            // アクションアニメーションが割り込みなしで終了した場合、
            if (success)
            {
                // ポストプロセス実行。
                _actionHandler.PostProcess(this);
                // 瀕死状態、かつ、HP が 0 のままアニメーション終了を迎えた場合は死亡。
                if (_stateHandler.IsApparentDeath() && CurrentHP == 0)
                {
                    _stateHandler.TryToTransitToDead();
                }
                else
                {
                    _stateHandler.TryToTransitToIdle();
                }
            }
        }
        void _onInvokeAction()
        {
            _actionHandler.ExecuteCurrentAction(_getField(), this);
        }

        /// タグ。
        public IActionTagSet TagSet { get { return _characterStatus.TagSet; } }
        /// 隊列（前衛 / 中衛 / 後衛）
        public CharacterPosition CharacterPosition { get { return _characterStatus.Position; } }
        /// 順番優先度。
        public uint OrderPriority { get { return _characterStatus.OrderPriority; } }
        /// 奥行き位置。
        public uint OrderInLayer { get { return _characterStatus.OrderInLayer; } }
        /// キャラクター ID。
        public uint CharacterID { get { return _characterStatus.CharacterID; } }
        /// レベル。
        public ushort Level { get { return _characterStatus.Level; } }
        /// 物理攻撃力。
        public uint PhysicalAttack { get { return _characterStatus.PhysicalAttack; } }
        /// 魔法攻撃力。
        public uint MagicPower { get { return _characterStatus.MagicPower; } }
        /// 物理防御力。
        public uint Armor { get { return _characterStatus.Armor; } }
        /// 魔法防御力。
        public uint MagicRegistance { get { return _characterStatus.MagicRegistance; } }
        /// 物理クリティカル。
        public uint PhysicalCrit { get { return _characterStatus.PhysicalCrit; } }
        /// 魔法クリティカル。
        public uint MagicCrit { get { return _characterStatus.MagicCrit; } }
        /// 物理防御貫通。
        public uint ArmorPenetration { get { return _characterStatus.ArmorPenetration; } }
        /// 魔法防御貫通。
        public uint IgnoreMagicResistance { get { return _characterStatus.IgnoreMagicResistance; } }
        /// 命中。
        public byte Accuracy { get { return _characterStatus.Accuracy; } }
        /// 回避。
        public byte Dodge { get { return _characterStatus.Dodge; } }
        /// ライフ奪取力。
        public uint LifeStealRating { get { return _characterStatus.LifeStealRating; } }
        /// 回復増加。
        public byte ImproveHealings { get {  return _characterStatus.ImproveHealings; } }

        public uint MaxHP { get { return (uint)HP.Max; } }
        public uint CurrentHP { get { return (uint)HP.Current; } }
        public ushort MaxEnergy { get { return (ushort)Energy.Max; } }
        public int SkinLevel { get { return _characterStatus.SkinLevel; } }

        float  _actionFrequency { get { return 1 / (Constant.IDEAL_ACTION_INTERVAL * Constant.LOGIC_FPS) * _characterStatus.ActionFrequency; } }

        public void AddStatusBoosterFactor   (IStatusBoostFactor factor)
        {
            _characterStatus.AddStatusBoosterFactor(factor);
            HP.ChangeMaxHP(_characterStatus.MaxHP);
        }
        public void RemoveStatusBoosterFactor(IStatusBoostFactor factor) { _characterStatus.RemoveStatusBoosterFactor(factor); }
        public void AddStateTransitionBlockerFactor   (StateTransitionBlocker.Factor factor) { _stateTransitionBlocker.AddFactor(factor)   ; }
        public void RemoveStateTransitionBlockerFactor(StateTransitionBlocker.Factor factor) { _stateTransitionBlocker.RemoveFactor(factor); }
        public void AddStatusBreakerFactor(StatusForceSetter.Factor factor)    { _characterStatus.AddStatusBreakerFactor(factor); }
        public void RemoveStatusBreakerFactor(StatusForceSetter.Factor factor) { _characterStatus.RemoveStatusBreakerFactor(factor); }

        public int GetBaseActionRange()
        {
            return _actionHandler.GetBaseRange();
        }
        public bool IsDead { get { return _stateHandler.IsDead(); } }
        public bool IsTargetable
        {
            get
            {
                var isTargetable = true;
                if (!_actionHandler.IsTargetable()) isTargetable = !_stateHandler.IsAction();
                return _stateHandler.IsTargetable() && isTargetable; 
            } 
        }
        public bool HasMissDamage()
        {
            return _actionEventHandler.HasMissDamage();
        }

        /// 死亡時に自身が OnNext される Observable を取得。
        public IObservable<Character> GetOnDieObservable() { return _stateHandler.GetOnEnterStateObservable().Where(state => state.GetType() == typeof(CharacterStateHandler.DeadState)).Select(t => this); }
        /// 死亡演出終了後に自身が OnNext される Observable を取得。
        public IObservable<Character> GetOnExitDeadAnimationObservable()
        {
            if (_onExitDeadAnimationSubject == null) _onExitDeadAnimationSubject = new Subject<Character>();
            return _onExitDeadAnimationSubject.AsObservable();
        }
        /// 与ダメージ時にダメージが OnNext される Observable を取得。
        public IObservable<int> GetOnAddDamageObservable() { return _actionEventHandler.GetOnAddDamageObservable(); }
        /// アクティブアクション発動時に自身が OnNext される Observable を取得。
        public IObservable<Character> GetOnEnterActiveActionObservable() { return _stateHandler.GetOnEnterStateObservable().Where(state => state.GetType() == typeof(CharacterStateHandler.ActiveActionState)).Select(t => this); }
        /// TODO : 確認。
        public IObservable<SpecialEvent> GetOnSpecialEventObservable() { return _actionEventHandler.GetOnSpecialEventObservable(); }
        public IObservable<ChangeableValueEvent> GetOnChangeShieldRatioObservable() { return _actionEventHandler.GetOnChangeShieldRatioObservable(); }
        public IObservable<Character> GetOnExitTimeStopObservable() { return _onExitTimeStopSubject.AsObservable(); }
        public IObservable<bool> GetOnToggleActiveActionAvailable() { return _onToggleActiveActionAvailable.AsObservable(); }
        public IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable()
        {
            return _onCommandCharacterEffect.Do(e => e.CharacterView = _view);
        }
        public IObservable<SoundEvent> GetOnCommandSoundObservable() { return _actionEventHandler.GetOnCommandSoundObservable().Merge(_actionHandler.GetOnCommandSoundObservable()); }
        public IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable() { return _actionHandler.GetOnTryCameraShakeObservable(); }
        public IObservable<ChangeableValueEvent> GetOnHPChangeObservable()
        {
            return HP.GetOnChangeObservable().Do(cve =>
            {
                if (cve.Current <= 0) _tryToTransitToApparentDeathOrDead();
            });
        }
        public IObservable<ChangeableValueEvent> GetOnEnergyChangeObservable() { return Energy.GetOnChangeObservable(); }

        public IObservable<CharacterStatusBoostEvent> GetOnChangeStatusBooster()
        {
            return _characterStatus.GetOnChangeStatusBooster();
        }

        public GridPosition? GetGridPosition() { return _gridPosition; }
        public void SetGridPosition(GridPosition currentGrid)
        {
            _gridPosition = currentGrid;
            _view.SetOrderInLayer((_getField().VerticalSize - currentGrid.VerticalIndex) * Constant.ORDER_RANGE_IN_CHARACTER_LAYER);
        }

        public bool IsFirstHidden { get{ return _actionHandler.IsFirstHidden(); } }

        public void SetForce(Force force)
        {
            Force = force;
            CurrentForce = force;
            SetHorizontalDirection(_getCurrentForwardDirection());
        }
        public byte No { get { return _no.Value; } }
        public void SetNo(byte no)
        {
            _no = no;
        }
        public Direction GetDirection() { return _direction.Value; }
        public void SetHorizontalDirection(Direction direction)
        {
            _direction = direction;
            if (_view != null) _view.SetHorizontalDirection(_direction.Value);
        }

        /// 死んでいるキャラクターを生き返らせる。
        /// これはあくまでもキャラクターを再利用するために利用するメソッドで、
        /// アクションに組み込んではいけない。
        public void Resurrect()
        {
            _view.Resurrect();
            _stateHandler.ForceReset();
        }
        /// バトルロジック開始。
        public void Start()
        {
            if (_onExitDeadAnimationSubject == null) _onExitDeadAnimationSubject = new Subject<Character>();
            _subscribeWithoutViewObservable();
            _initializeView();
            _stateHandler.Start();
        }
        public void RemoveGridPosition() { _gridPosition = null; }

        public void SendActionEvent(ActionEvent actionEvent)
        {
            _actionEventHandler.SendActionEvent(actionEvent);
        }
        public void ReceiveActionEvent(ActionEvent actionEvent)
        {
            if (!IsDead)
            {
                var stateId = _actionEventHandler.ReceiveActionEvent(actionEvent);
                if (_actionHandler.PrepareNextReceiveEventAction(_getField(), this, actionEvent))
                {
                    _stateHandler.TryToTransitToAction(_actionHandler.GetCurrentActionAnimationName());
                }
                else if (stateId.HasValue)
                {
                    _stateHandler.TryToTransitByStateID(stateId.Value);
                }
            }
        }
        public IObservable<BulletHitEvent> GetOnHitObservable()
        {
            return _view.GetOnHitObservable().Do(he => he.ActionTarget = this);
        }

        public bool TryToTransitToStop()
        {
            return _stateHandler.TryToTransitToStop();
        }
        public bool TryToTransitFromStop()
        {
            return _stateHandler.TryToTransitToIdle();
        }
        public void SetCurrentForceReverse()
        {
            CurrentForce = Force == Force.Enemy ? Force.Own : Force.Enemy;
        }

        public void SetCurrentForceOriginal()
        {
            CurrentForce = Force;
        }

        /// ウェーブ間遷移時の回復処理。
        public void RecoverAtInterval()
        {
            HP.RecoverWhenMoveNextWave();
            Energy.RecoverWhenMoveNextWave();
        }

        /// 待機状態へ入った際の処理。
        void _idleOnEnter()
        {
            TryToChangeDirection();
        }

        /// 前進状態へ遷移すべきかどうか判定。
        bool _shouldTransitToProceed()
        {
            return _getField().ShouldMoveForward(this, _direction.Value, (ushort)GetBaseActionRange());
        }
        /// 前進状態への遷移を試みる。
        bool _tryToTransitToProceed()
        {
            if (!_shouldTransitToProceed()) return false;
            if (_stateTransitionBlocker.ProceedBlock) return false;
            return _stateHandler.TryToTransitToProceed();
        }
        /// 前進状態へ入った際の処理。
        void _proceedOnEnter()
        {
            _proceed();
        }
        void _proceed()
        {
            _getField().MoveCharacter(this, _direction.Value);
            _view.Move(_getCorrectPosition(), Constant.WALK_TIME_PER_GRID / _characterStatus.ActionFrequency).Subscribe(success =>
            {
                if (_shouldTransitToProceed())
                {
                    if (TryToChangeDirection() && _stateHandler.TryToTransitToIdle()) return;
                    _proceed();
                }
                else
                {
                    _stateHandler.TryToTransitToIdle();
                }
            });
        }

        /// 瀕死状態への遷移を試み、無理なら死亡状態へ遷移する。
        void _tryToTransitToApparentDeathOrDead()
        {
            if (_stateHandler.IsApparentDeath()) return;
            if (_actionHandler.PrepareNextDeadAction(_getField(), this))
            {
                _stateHandler.TryToTransitToApparentDeath();
            }
            else
            {
                _tryToTransitToDead();
            }
        }
        /// 瀕死状態へ入った際の処理。
        void _apparentDeathOnEnter()
        {
            _view.PlayAction(_actionHandler.GetCurrentActionAnimationName());
        }
        /// 死亡状態へ遷移する。原則、直接呼び出すのは禁止。
        protected bool _tryToTransitToDead()
        {
            return _stateHandler.TryToTransitToDead();
        }
        /// 死亡状態へ入った際の処理。
        protected virtual void _deadOnEnter()
        {
            Energy.Reset();
            RemoveAttributeAndShadow();
            _disposablesWithoutView.Clear();
            _disposablesForView.Clear();
            // 内部で OnCompleted() が呼ばれるので Dispose はしなくても良い前提。
            _playDieAnimation().Subscribe(_ => _stock());
        }
        /// 死亡（死亡時に流れるだけで必ずしも死亡アニメーションとは限らず逃走、待機もあり）アニメーションを再生する。
        /// オーバーライドする場合は、終了時に処理を引っ掛けるので演出完了時に OnNext(this) される Observable を返し、OnCompleted() も忘れずに呼ぶこと。
        protected virtual IObservable<Character> _playDieAnimation()
        {
            return _view.PlayAnimationAtEnterDead().Select(_ => this);
        }

        /// 通常アクション発動可否チェック。
        bool _canInvokeNormalAction()
        {
            return _normalActionGaugeRatio == 1 && _stateHandler.CanTransitToAction();
        }
        /// 通常アクションをセットした上でアクション状態への遷移を試みる。
        bool _tryToTransitToNormalAction()
        {
            if (!_canInvokeNormalAction()) return false;
            if (!_actionHandler.PrepareNextNormalAction(_getField(), this)) return false;
            // 遷移ブロッカーによる遷移ブロック。
            if (_isBlockedActionTransition())
            {
                // この場合はアクションゲージもリセットしてしまう。
                _normalActionGaugeRatio = 0;
                return false;
            }
            return _stateHandler.TryToTransitToAction(_actionHandler.GetCurrentActionAnimationName());
        }
        /// アクション状態へ入った際の処理。
        void _actionOnEnter()
        {
            _normalActionGaugeRatio = 0;
        }

        /// アクティブアクション発動可否チェック。
        public void CheckActiveActionAvailable()
        {
            _onNextOnToggleActiveActionAvailable(_canInvokeActiveAction(false));
        }
        /// アクティブアクション状態への遷移を試みる。
        public bool TryToTransitToActiveAction()
        {
            var canAct = _canInvokeActiveAction(true);
            // ユーザアクションベースでも更新しておく
            _onNextOnToggleActiveActionAvailable(canAct);
            if (!canAct) return false;
            return _stateHandler.TryToTransitToActiveAction(_actionHandler.GetCurrentActionAnimationName());
        }
        /// アクティブアクション発動可否チェック。引数によって実際に次のアクションとしてセットするかどうかの分岐あり。
        bool _canInvokeActiveAction(bool executePrepareNextActiveAction)
        {
            // エネルギーが満タンでないと発動できない
            if (!Energy.IsFull) return false;
            // ブロックされていたら発動できない
            if (_stateTransitionBlocker.ActiveActionBlock) return false;
            // 遷移ブロッカーによる遷移ブロック。
            if (_isBlockedActionTransition()) return false;
            // アクションが実際に発動できるかチェック。
            return executePrepareNextActiveAction ? _actionHandler.PrepareNextActiveAction(_getField(), this) : _actionHandler.CanPrepareNextActiveAction(_getField(), this);
        }
        /// アクティブアクション状態へ入った際の処理。
        void _activeActionOnEnter()
        {
            Energy.Consume(_characterStatus.ReduceEnergyCost);
            _onCommandCharacterEffect.OnNext(new CharacterEffectEvent
            {
                Command           = CharacterEffectCommand.PlayOnce,
                CharacterEffectID = Constant.CHARGE_ACTION_CHARACTER_EFFECT_ID,
            });
            _onNextOnToggleActiveActionAvailable(false);
        }
        /// アクティブアクション発動可否情報を OnNext する。
        void _onNextOnToggleActiveActionAvailable(bool available)
        {
            _onToggleActiveActionAvailable.OnNext(available);
        }

        /// バトル開始時に発動するアクション呼び出しを試みる（アニメーション有無による違いあり）。
        public void TryToInvokeFirstAction()
        {
            _tryToExecuteFirstActionDirectly();
            _tryToTransitToFirstAction();
        }
        /// バトル開始時に発動するアクション実行を試みる（状態遷移なし）。
        bool _tryToExecuteFirstActionDirectly()
        {
            if (_isBlockedActionTransition()) return false;
            return _actionHandler.ExecuteFirstActionDirectly(_getField(), this);
        }
        /// バトル開始時に発動するアクションをセットした上でアクション状態への遷移を試みる。
        bool _tryToTransitToFirstAction()
        {
            if (_isBlockedActionTransition()) return false;
            var canAct = _actionHandler.PrepareNextFirstWithAnimationAction(_getField(), this);
            if (!canAct) return false;
            return _stateHandler.TryToTransitToAction(_actionHandler.GetCurrentActionAnimationName());
        }
        /// 敵死亡時に発動するアクション実行を試みる（状態遷移なし）。
        public bool TryToExecuteEnemyDeadActionDirectly()
        {
            if (_isBlockedActionTransition()) return false;
            return _actionHandler.ExecuteEnemyDeadActionDirectly(_getField(), this);
        }
        // アクションブロック判定。
        bool _isBlockedActionTransition()
        {
            if (_stateTransitionBlocker.MagicActionBlock)
            {
                if (_actionHandler.GetCurrentActionDamageType().HasValue && _actionHandler.GetCurrentActionDamageType().Value == DamageType.Magical)
                {
                    return true;
                }
            }
            return false;
        }

        void _knockbackOnEnter()
        {
            var direction = _direction.Value == Direction.Right ? Direction.Left : Direction.Right;
            var nextGrid  = GetGridPosition().Value.BuildNextGrid(direction, Constant.KNOCKBACK_DISTANCE);
            _getField().MoveCharacter(this, nextGrid);
            _view.PlayKnockback(_getCorrectPosition(), Constant.KNOCKBACK_TIME);
        }

        public void SetSpeed(float speed)
        {
            _view.SetSpeed(speed);
        }
        public void HorizontalMove(short horizontalDiff, float duration)
        {
            _getField().MoveCharacterHorizontallyWithView(this, _view, GetDirection(), horizontalDiff, duration).Subscribe();
        }
        public void HorizontalMove(Func<short> getHorizontalDiff, float duration)
        {
            _getField().MoveCharacterHorizontallyWithView(this, _view, GetDirection(), getHorizontalDiff, duration).Subscribe();
        }
        public void GoStraight(float duration)
        {
            _getField().MoveCharacterToHorizontalEndWithView(this, _view, GetDirection(), duration).Subscribe();
        }
        public void Appear()
        {
            _view.StopMove();
            _getField().MoveCharacterToAvailableStartPositionWithView(this, _view);
            TryToChangeDirection();
            _stateHandler.TryToTransitToIdle();
        }

        public void SetViewToCorrectPosition()
        {
            if (GetGridPosition().HasValue)
            {
                var correctPosition = _getCorrectPosition();
                _view.SetPosition(correctPosition);
            }
        }
        public IObservable<bool> SetViewToCorrectPositionWithWalking(float duration)
        {
            return _view.WalkTo(_getCorrectPosition(), duration);
        }
        public IObservable<bool> SetViewToCorrectPositionWithRunning(float duration)
        {
            return _view.RunTo(_getCorrectPosition(), duration);
        }

        Vector2 _getCorrectPosition()
        {
            return _getField().GetPosition(GetGridPosition().Value);
        }

        public void SetViewToStoryWavePosition()
        {
            _view.SetPosition(_getField().GetStoryWavePosition(GetGridPosition().Value));
        }


        public IActionTargetView GetActionTargetView()
        {
            return _view.GetActionTargetView();
        }
        public ICharacterView GetView()
        {
            return _view;
        }
        public IActionExecutorView GetViewAsActionExecutorView()
        {
            return (IActionExecutorView)_view;
        }

        public bool TryToChangeDirection()
        {
            var newDirection = _getField().GetProceedDirectionToChange(this);
            if (newDirection.HasValue)
            {
                SetHorizontalDirection(newDirection.Value);
                return true;
            }
            return false;
        }

        /// オートモードセット。
        public void SetAuto(bool auto) { _autoActiveAction = auto; }
        /// オートモード時処理。
        void _autoModeProcess()
        {
            if (!_autoActiveAction) return;
            TryToTransitToActiveAction();
        }

        /// ロジック更新ループ。FPS で数えられるループと一致しない頻度のループであることに注意。
        public void LogicUpdate()
        {
            if (!_stateHandler.CanLogicUpdate()) return;

            if (_pause) return;

            _autoModeProcess();
            _tryToChargeNormalActionGauge();
            _tryToTransitToNormalAction();
            _tryToTransitToProceed();
        }
        /// 通常アクションゲージのチャージを試みる。
        void _tryToChargeNormalActionGauge()
        {
            if (!_stateHandler.CanChargeNormalActionGauge()) return;
            _normalActionGaugeRatio += _actionFrequency;
            if (_normalActionGaugeRatio > 1) _normalActionGaugeRatio = 1;
        }

        public void PauseOn()
        {
            _pause = true;
            _actionHandler.PauseOn();
            _view.PauseOn();
        }
        public void PauseOff()
        {
            _view.PauseOff();
            _actionHandler.PauseOff();
            _pause = false;
        }

        public CharacterThumbnail BuildCharacterThumbnail()
        {
            // TODO
            return new CharacterThumbnail(CharacterID, (ushort)_characterStatus.Level, (byte)_characterStatus.EvolutionLevel, (byte)_characterStatus.PromotionLevel, _characterStatus.SkinLevel, IsDead);
        }

        public int GetActiveActionLevel()
        {
            return _actionHandler.GetActiveActionLevel();
        }

        public int GetShadowNum()
        {
            return ShadowHandler.GetShadowNum();
        }
        public IShadow BuildShadow(uint shadowCharacterId)
        {
            return new ShadowCharacter(shadowCharacterId);
        }
        public IObservable<ShadowCharacter> MakeShadow(IShadow shadowCharacter, ushort? level, ushort? actionLevel2, ushort? actionLevel3, ushort initialHorizontalIndex)
        {
            return ShadowHandler.MakeShadow((ShadowCharacter)shadowCharacter, Force, level, actionLevel2, actionLevel3, initialHorizontalIndex);
        }

        Direction _getCurrentForwardDirection()
        {
            return WaveField.GetForwardDirection(Force);
        }
        WaveField _getField()
        {
            // TODO : 抽象化
            return SceneContainer.GetBattleManager().CurrentWave.Field;
        }

        public void RemoveAttributeAndShadow()
        {
            _actionEventHandler.RemoveAttributes();
            ShadowHandler.Clear();
        }

        public void PrepareNextWave()
        {
            _stateHandler.Reset();
            _normalActionGaugeRatio = 0;
            _actionHandler.Reset(this);
            SetHorizontalDirection(_getCurrentForwardDirection());
        }

        public bool TryToIdleAtFinishBattle()
        {
            return _stateHandler.TryToTransitToIdle();
        }
        public IObservable<bool> Win()
        {
            _stateHandler.TransitToWin();
            return _view.GetOnExitWinAnimationObservable();
        }

        protected void _stock()
        {
            GlobalContainer.Logger.Debug("Stock");
            _view.SetActive(false);
            HP.Reset();
            Energy.Reset();
            _normalActionGaugeRatio = 0;
            _onExitDeadAnimationSubject.OnNext(this);
            _onExitDeadAnimationSubject.OnCompleted();
            _onExitDeadAnimationSubject = null;
        }

        public override void Dispose()
        {
            RemoveAttributeAndShadow();
            _disposablesWithoutView.Dispose();
            _disposablesForView.Dispose();
            base.Dispose();
        }
    }
}
