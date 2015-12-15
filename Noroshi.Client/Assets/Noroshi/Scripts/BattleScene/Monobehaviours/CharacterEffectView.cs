using UniRx;
using UnityEngine;
using Noroshi.BattleScene.CharacterEffect;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class CharacterEffectView : SpineView, ICharacterEffectView
    {
        const string CHARACTER_EFFECT_TEXT_ANIMATION_NAME_REGEX = "_text";
        const string DEFAULT_SORTING_LAYER_NAME = "Default";
        int _orderInCharacterLayer;
        bool _unreversible = false;

        public IObservable<bool> PlayOnce(ICharacterView characterView, string animationName, PositionInCharacterView position, int orderInCharacterLayer, bool unreversible = false)
        {
            return _play(characterView, animationName, position, orderInCharacterLayer, false, unreversible);
        }
        public IObservable<bool> Play(ICharacterView characterView, string animationName, PositionInCharacterView position, int orderInCharacterLayer, bool unreversible = false)
        {
            return _play(characterView, animationName, position, orderInCharacterLayer, true, unreversible);
        }
        public IObservable<bool> Play(ICharacterView icharacterView, string onceAnimationName, string loopAnimationName, PositionInCharacterView position, int orderInCharacterLayer)
        {
            return _play(icharacterView, onceAnimationName, position, orderInCharacterLayer, false, false)
            .SelectMany(b => b ? _play(loopAnimationName, true) : Observable.Return<bool>(false));
        }
        IObservable<bool> _play(ICharacterView icharacterView, string animationName, PositionInCharacterView position, int orderInCharacterLayer, bool loop, bool unreversible)
        {
            _unreversible = unreversible;
            _orderInCharacterLayer = orderInCharacterLayer;
            var characterView = (CharacterView)icharacterView;

            var showTextEffctAnimation = _isTextEffectAnimation(animationName);
            if (showTextEffctAnimation)
            {
                SetSortingLayerName(Constant.UI_SORTING_LAYER_NAME);
            }
            else
            {
                SetSortingLayerName(DEFAULT_SORTING_LAYER_NAME);
            }

            SetParent(characterView, showTextEffctAnimation);
            SetLocalPosition(characterView.GetLocalPosition(position));

            SetOrder(characterView.GetOrderInLayer(), orderInCharacterLayer);

            return _play(animationName, loop);
        }

        public void SetParent(CharacterView parent, bool fixedLocalScale)
        {
            SetParent(parent.GetTransform());
            _transform.localRotation = Quaternion.identity;
            _transform.localScale = fixedLocalScale ? Vector3.one : Vector3.one * parent.GetEffectScale();
        }

        public void StopWithPlayOnce(string animationName)
        {
            _stopWithPlay(animationName);
        }

        public void SetOrder(int characterViewOrder, int orderInCharacterLayer)
        {
            SetOrderInLayer(characterViewOrder + orderInCharacterLayer);
        }

        bool _isTextEffectAnimation(string animationName)
        {
            return animationName.IndexOf(CHARACTER_EFFECT_TEXT_ANIMATION_NAME_REGEX) >= 0;
        }

        void Update()
        {
            if (_unreversible)
            {
                _transform.rotation = Quaternion.identity;
            }
        }
    }
}
