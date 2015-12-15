using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.StateTransition;

namespace Noroshi.BattleScene
{
    public class BattleStateHandler : StateManager
    {
        Subject<VictoryOrDefeat> _onFinishBattleSubject = new Subject<VictoryOrDefeat>();
        CompositeDisposable _disposables = new CompositeDisposable();

        public BattleStateHandler()
        {
            var wave       = new Wave();
            var interval   = new Interval();
            var switchWave = new SwitchWave();
            AddState(wave);
            AddState(interval);
            AddState(switchWave);

            wave.OnEnter += (sender, e) => {
                var battleManager = SceneContainer.GetBattleManager();
                // Wave スタート
                battleManager.StartWave();
            };

            interval.OnEnter += (sender, e) => {
                SceneContainer.GetBattleManager().StartInterval().Do(_ => TransitTo<SwitchWave>())
                .Subscribe().AddTo(_disposables);
            };
            interval.OnExit += (sender, e) => {
                _disposables.Clear();
            };

            switchWave.OnEnter += (sender, e) => {
                // Wave 切り替え
                SceneContainer.GetBattleManager().SwitchWave().Do(_ => {
                    TransitTo<Wave>();
                })
                .Subscribe().AddTo(_disposables);
            };
            switchWave.OnExit += (sender, e) => {
                _disposables.Clear();
            };
        }

        public IObservable<VictoryOrDefeat> Start()
        {
            base.Start<Wave>();
            return _onFinishBattleSubject.AsObservable();
        }

        /// Interval へ遷移する。
        public void TransitToInterval()
        {
            TransitTo<Interval>();
        }

        public void Finish(VictoryOrDefeat battleResult)
        {
            Finish();
            _onFinishBattleSubject.OnNext(battleResult);
        }

        /// バトルロジックループを回して良いか
        public bool CanLogicUpdate() { return _currentState != null && _currentState.GetType() == typeof(Wave); }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public class Wave       : State {}
        public class Interval   : State {}
        public class SwitchWave : State {}
    }
}
