using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class PauseModalUI : AbstractModalUIViewModel<IPauseModalUIView>
    {
        const string NAME = "PauseModalUI";
        public PauseModalUI(){}

        protected override IObservable<IPauseModalUIView> _loadView()
        {
            return SceneContainer.GetFactory().BuildModalUIView(NAME)
                .Select(modalView => (IPauseModalUIView)modalView);
        }

        public IObservable<bool> GetOnClickWithdrawalObservable()
        {
            return _uiView.GetOnClickWithdrawalObservable();
        }
    }
}
