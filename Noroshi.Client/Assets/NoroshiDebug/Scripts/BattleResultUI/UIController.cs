using UnityEngine;
using Noroshi.BattleScene.UI;

namespace NoroshiDebug.BattleResultUI
{
    public class UIController : Noroshi.BattleScene.MonoBehaviours.MonoBehaviour
    {
        [SerializeField] Noroshi.BattleScene.MonoBehaviours.MonoBehaviour _modalContainer;

        public void AddResultUIView(IResultUIView uiView)
        {
            uiView.SetParent(_modalContainer, false);
        }
    }
}