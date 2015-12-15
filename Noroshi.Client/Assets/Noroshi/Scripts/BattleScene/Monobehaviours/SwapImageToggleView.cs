using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Noroshi.BattleScene.MonoBehaviours
{
    [RequireComponent(typeof(Toggle))]
    public class SwapImageToggleView : UIView
    {
        [SerializeField] Image _backgroundImage;

        Toggle _toggle;
        Subject<bool> _onToggleSubject = new Subject<bool>();

        new void Awake()
        {
            base.Awake();
            _toggle = GetComponent<Toggle>();
        }

        void OnDestroy()
        {
            _onToggleSubject.OnCompleted();
        }

        public IObservable<bool> GetOnToggleObservable()
        {
            return _onToggleSubject.AsObservable();
        }
        
        /// UnityEngine.UI.Toggle にセットするためのメソッド。
        /// 押した時isOnがtrueなら_backgroundを非表示にするように
        /// falseなら表示するように
        public void ClickToggle()
        {
            _backgroundImage.enabled = !_toggle.isOn;
            _onToggleSubject.OnNext(_toggle.isOn);
        }
    }
}
