using System.Collections.Generic;
using UniRx;

namespace Noroshi.BattleScene
{
    public class ShadowHandler
    {
        List<ShadowCharacter> _shadowCharacters = new List<ShadowCharacter>();
        Subject<ShadowCharacter> _onMakeShadow = new Subject<ShadowCharacter>();
        Subject<ShadowCharacter> _onRemoveShadow = new Subject<ShadowCharacter>();

        public IObservable<ShadowCharacter> GetOnMakeShadowObservable() { return _onMakeShadow.AsObservable(); }
        public IObservable<ShadowCharacter> GetOnRemoveShadowObservable() { return _onRemoveShadow.AsObservable(); }

        public int GetShadowNum()
        {
            return _shadowCharacters.Count;
        }
        public IObservable<ShadowCharacter> MakeShadow(ShadowCharacter shadowCharacter, Force force, ushort? level, ushort? actionLevel2, ushort? actionLevel3, ushort initialHorizontalIndex)
        {
            // 前に死亡状態になっているかもしれないので復活させておく。
            shadowCharacter.Resurrect();
            var onExitDeadAnimationObservable = shadowCharacter.GetOnExitDeadAnimationObservable();
            shadowCharacter.Appear(level, actionLevel2, actionLevel3, initialHorizontalIndex);
            shadowCharacter.SetForce(force);
            _shadowCharacters.Add(shadowCharacter);
            _onMakeShadow.OnNext(shadowCharacter);
            return onExitDeadAnimationObservable.Select(c => (ShadowCharacter)c)
                .Do(s => _removeShadowCharacter(s));
        }

        void _removeShadowCharacter(ShadowCharacter shadowCharacter)
        {
            if (_shadowCharacters.Remove(shadowCharacter))
            {
                _onRemoveShadow.OnNext(shadowCharacter);
            }
        }

        public void Clear()
        {
            foreach (var shadow in new List<ShadowCharacter>(_shadowCharacters))
            {
                shadow.Escape();
                _removeShadowCharacter(shadow);
            }
            _shadowCharacters.Clear();
        }
    }
}