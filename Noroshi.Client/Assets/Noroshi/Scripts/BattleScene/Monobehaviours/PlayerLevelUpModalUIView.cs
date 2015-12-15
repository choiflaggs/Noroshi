using UniRx;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;
using Noroshi.Core.Game.GameContent;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class PlayerLevelUpModalUIView : ModalUIView, UI.IPlayerLevelUpModalView
    {
        [SerializeField] Text _previousPlayerLevel;
        [SerializeField] Text _currentPlayerLevel;
        [SerializeField] Text _previousMaxStamina;
        [SerializeField] Text _currentMaxStamina;
        [SerializeField] Text _openGameContents;

        public void SetAddPlayerExpResult(Core.WebApi.Response.Players.AddPlayerExpResult addPlayerExpResult)
        {
            _previousPlayerLevel.text = addPlayerExpResult.PreviousPlayerLevel.ToString();
            _currentPlayerLevel.text = addPlayerExpResult.CurrentPlayerLevel.ToString();
            _previousMaxStamina.text = addPlayerExpResult.PreviousMaxStamina.ToString();
            _currentMaxStamina.text = addPlayerExpResult.CurrentMaxStamina.ToString();
            if (addPlayerExpResult.OpenGameContentIDs.Length > 0)
            {
                var openGameContents = GameContent.BuildMulti(addPlayerExpResult.OpenGameContentIDs);
                _openGameContents.text = string.Join("\n", openGameContents.Select(gc => GlobalContainer.LocalizationManager.GetText(gc.TextKey)).ToArray()) + "\nが解放されました！";
            }
        }
    }
}
