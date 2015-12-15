using UnityEngine;
using UniLinq;
using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class DebugUIView : UIView
    {
        [SerializeField] UnityEngine.UI.Text _fps;

        void Start()
        {
            var frameCount = (int)(Application.targetFrameRate * 0.5f);
            Observable.EveryUpdate().Select(_ => Time.time)
            .Buffer(frameCount)
            .Subscribe(ts =>
            {
                _fps.text = ((int)(frameCount / (ts.Last() - ts.First()))).ToString();
            }).AddTo(this);
        }

        public void RecoverEnergy()
        {
            foreach (var character in SceneContainer.GetCharacterManager().GetCurrentOwnCharacters())
            {
                character.Energy.RecoverFully();
            }
        }
        public void KillAll()
        {
            foreach (var character in SceneContainer.GetCharacterManager().CurrentEnemyCharacterSet.GetCharacters())
            {
                var actionEvent = new ActionEvent(null, character);
                actionEvent.SetHPDamage(99999);
                character.ReceiveActionEvent(actionEvent);
            }
        }
    }
}
