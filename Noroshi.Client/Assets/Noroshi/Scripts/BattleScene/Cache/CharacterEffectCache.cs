using UniRx;
using Noroshi.BattleScene.CharacterEffect;

namespace Noroshi.BattleScene.Cache
{
    public class CharacterEffectCache : AbstractCache<ICharacterEffectView>
    {
        ICharacterFactory _factory;
        uint _characterEffectId;
        public CharacterEffectCache(ICharacterFactory factory, uint characterEffectId)
        {
            _factory = factory;
            _characterEffectId = characterEffectId;
        }
        protected override IObservable<ICharacterEffectView> _loadContent()
        {
            return _factory.BuildCharacterEffectView(_characterEffectId).Do(v => v.SetActive(false));
        }
        protected override IObservable<ICharacterEffectView[]> _loadContents(int num)
        {
            return _factory.BuildCharacterEffectViewMulti(_characterEffectId, num).Select(vs =>
            {
                for (var i = 0; i < vs.Length; i++)
                {
                    vs[i].SetActive(false);
                }
                return vs;
            });
        }
    }
}
