using UniRx;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.Cache
{
    public class CharacterAboveUIViewCache : AbstractCache<ICharacterAboveUIView>
    {
        IFactory _factory;
        public CharacterAboveUIViewCache(IFactory factory)
        {
            _factory = factory;
        }
        protected override IObservable<ICharacterAboveUIView> _loadContent()
        {
            return _factory.BuildCharacterAboveUIView().Do(v => v.SetActive(false));
        }
        protected override IObservable<ICharacterAboveUIView[]> _loadContents(int num)
        {
            return _factory.BuildCharacterAboveUIViewMulti(num).Select(vs =>
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
