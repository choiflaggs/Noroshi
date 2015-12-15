using System;
using UniLinq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Noroshi.Core.Game.Enums;
using Noroshi.TimeUtil;
using Noroshi.WebApi;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class Main : UnityEngine.MonoBehaviour
    {
        [SerializeField] Factory _factory;

        [SerializeField] bool _mockWebAPI = false;

        [SerializeField] BattleCategory _battleCategory;
        [SerializeField] uint _battleContentId = 1;
        [SerializeField] uint _payment = 0;
        [SerializeField] uint _debugPlayerCharacterID1;
        [SerializeField] uint _debugPlayerCharacterID2;
        [SerializeField] uint _debugPlayerCharacterID3;
        [SerializeField] uint _debugPlayerCharacterID4;
        [SerializeField] uint _debugPlayerCharacterID5;
        
        CompositeDisposable _disposables = new CompositeDisposable();

        void Awake()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
            // Unity から直接起動した場合は各インスペクタに設定したデータを使って遷移し直す。
            var reloadForDebug = false;
            uint[] playerCharacterIds = Bridge.Transition.PlayerCharacterIDs;
            if (playerCharacterIds == null)
            {
                var debugPlayerCharacterIds = (new uint[]{
                    _debugPlayerCharacterID1,
                    _debugPlayerCharacterID2,
                    _debugPlayerCharacterID3,
                    _debugPlayerCharacterID4,
                    _debugPlayerCharacterID5,
                }).Where(id => id > 0).ToArray();
                playerCharacterIds = debugPlayerCharacterIds;
                reloadForDebug = true;
            }
            var battleContentId = Bridge.Transition.ID;
            if (reloadForDebug)
            {
                battleContentId = _battleContentId;
            }
            var battleCategory = Bridge.Transition.BattleCategory;
            if (reloadForDebug)
            {
                battleCategory = _battleCategory;
            }
            if (reloadForDebug)
            {
                WebApiRequester.Login().SelectMany(_ => GlobalContainer.Load()).Subscribe(_ => 
                {
                    if (battleCategory == BattleCategory.Arena)
                    {
                        Bridge.Transition.TransitToPlayerBattle(battleContentId, playerCharacterIds);
                    }
                    else if (battleCategory == BattleCategory.RaidBoss)
                    {
                        Bridge.Transition.TransitToGuildRaidBossBattle(battleContentId, playerCharacterIds, (byte)_payment);
                    }
                    else
                    {
                        Bridge.Transition.TransitToCpuBattle(battleCategory, battleContentId, playerCharacterIds);
                    }
                })
                .AddTo(_disposables);
                gameObject.SetActive(false);
            }
            // FPS 設定
            Application.targetFrameRate = Constant.ENGINE_FPS;
        }

        void Start()
        {
            // GlobalContainer に DI
            GlobalContainer.SetFactory<Repositories.RepositoryManager>(() => new Repositories.RepositoryManager());
            GlobalContainer.SetFactory<Random.IRandomGenerator>(() => new Random.RandomGenerator());

            // SceneContainer に DI
            SceneContainer.Register<IFactory>    (() => _factory);
            SceneContainer.Register<ITimeHandler>(() => new TimeHandler());
            SceneContainer.Register<SceneManager>(() => new SceneManager(Bridge.Transition.BattleCategory, Bridge.Transition.ID, Bridge.Transition.PlayerCharacterIDs, Bridge.Transition.PaymentNum));
            SceneContainer.Register<IWebApiRequester>(() =>
            {
                // API 接続先デバッグ対応
                return _mockWebAPI ? (IWebApiRequester)new MockResourceWebApiRequester() : (IWebApiRequester)new WebApiRequester();
            });

            // シーン遷移処理を設定
            SceneContainer.GetSceneManager().GetOnTransitSceneObservable()
            .Subscribe(retry => 
            {
                Time.timeScale = 1;
                var sceneName = retry ? Application.loadedLevelName : Bridge.Transition.AfterBattleSceneName;
                Application.LoadLevel(sceneName);
            })
            .AddTo(_disposables);

            // 時間停止切り替え処理を設定
            SceneContainer.GetSceneManager().GetOnTogglePauseObservable()
            .Subscribe(isOn => Time.timeScale = isOn ? 0 : 1).AddTo(_disposables);

            // 一時的に時間の流れを遅くする処理を設定
            SceneContainer.GetSceneManager().GetOnSlowObservable().SelectMany(duration =>
            {
                var timeScale = Constant.SLOW_TIME_SCALE;
                Time.timeScale = timeScale;
                return SceneContainer.GetTimeHandler().Timer(duration * timeScale);
            })
            .Subscribe(_ => Time.timeScale = 1).AddTo(_disposables);

            // 初期化（Manager 間の処理）
            SceneContainer.GetSceneManager().Initialize();

            // 処理開始
            SceneContainer.GetSceneManager().Start();

            this.UpdateAsObservable().Subscribe(_ => SceneContainer.GetSceneManager().UpdatePerFrame()).AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
            // 本シーンで利用した static 領域は忘れずに削除。
            SceneContainer.Dispose();
        }
    }
}
