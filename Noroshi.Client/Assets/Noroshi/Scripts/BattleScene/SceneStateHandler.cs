using System;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.StateTransition;

namespace Noroshi.BattleScene
{
    public class SceneStateHandler : StateManager
    {
        Subject<VictoryOrDefeat> _onEnterResultSubject = new Subject<VictoryOrDefeat>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public SceneStateHandler()
        {
            var init   = new Initialization();
            var ready  = new Ready();
            var battle = new Battle();
            var win    = new Win();
            var loss   = new Loss();
            AddState(init);
            AddState(ready);
            AddState(battle);
            AddState(win);
            AddState(loss);

            init.OnEnter += (sender, e) => {
                // 初期データをロードをした後に初期ビューをロードする。
                // 完了したら Ready 状態へ遷移。
                SceneContainer.GetSceneManager().LoadDatas()
                .SelectMany(_ => SceneContainer.GetSceneManager().LoadAssets())
                // TODO
                .Do(_ => UnityEngine.Resources.UnloadUnusedAssets())
                .Do(_ => SceneContainer.GetSceneManager().Prepare())
                .SelectMany(_ => SceneContainer.GetTimeHandler().Timer(0.1f)) // 各 MonoBehaviour の Start() が呼ばれるようにちょっと待つ
                .Do(_ => TransitTo<Ready>())
                .Subscribe().AddTo(_disposables);
            };
            init.OnExit += (sender, e) => {
                _disposables.Clear();
            };

            battle.OnEnter += (sender, e) =>
            {
                SceneContainer.GetBattleManager().Start()
                .Subscribe(result =>
                {
                    if (result == VictoryOrDefeat.Win)
                    {
                        TransitTo<Win>();
                    }
                    else
                    {
                        TransitTo<Loss>();
                    }
                })
                .AddTo(_disposables);
            };

            win.OnEnter += (sender, e) => {
                _onEnterResultSubject.OnNext(VictoryOrDefeat.Win);
            };

            loss.OnEnter += (sender, e) => {
                _onEnterResultSubject.OnNext(VictoryOrDefeat.Loss);
            };
        }

        public IObservable<Type> GetOnExitInitializeObservable()
        {
            return GetOnExitStateObservable().Where(t => t == typeof(Initialization));
        }
        public IObservable<Type> GetOnEnterReadyObservable()
        {
            return GetOnEnterStateObservable().Where(t => t == typeof(Ready));
        }
        public IObservable<VictoryOrDefeat> GetOnEnterResultObservable()
        {
            return _onEnterResultSubject.AsObservable();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void TransitToBattle()
        {
            TransitTo<Battle>();
        }

        public class Initialization : State {}
        public class Ready          : State {}
        public class Battle         : State {}
        public class Win            : State {}
        public class Loss           : State {}
    }
}
