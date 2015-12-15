using UnityEngine;
using UniRx;

namespace Noroshi.BattleScene.UI
{
    public class PlayerLevelUpModalUI : AbstractModalUIViewModel<IPlayerLevelUpModalView>
    {
        const string NAME = "PlayerLevelUpModalUI";

        public PlayerLevelUpModalUI()
        {
        }
        
        protected override IObservable<IPlayerLevelUpModalView> _loadView()
        {
            return SceneContainer.GetFactory().BuildModalUIView(NAME)
                .Select(modalView => (IPlayerLevelUpModalView)modalView);
        }

        public IObservable<PlayerLevelUpModalUI> Open(Core.WebApi.Response.Players.AddPlayerExpResult addPlayerExpResult)
        {
            _uiView.SetAddPlayerExpResult(addPlayerExpResult);
            return Open().Select(_ => this);
        }
    }
}
