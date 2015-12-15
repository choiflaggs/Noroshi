using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class PlayerBattleUIManager : AbstractUIManager
    {
        public override void Initialize()
        {
            base.Initialize();
            SceneContainer.GetSceneManager().GetOnEnterReadyObservable()
            .Subscribe(_ => _onEnterReady()).AddTo(_disposables);
        }
        void _onEnterReady()
        {
            _uiController.PlayPrepareAnimation()
            .Subscribe().AddTo(_disposables);
        }
    }
}
