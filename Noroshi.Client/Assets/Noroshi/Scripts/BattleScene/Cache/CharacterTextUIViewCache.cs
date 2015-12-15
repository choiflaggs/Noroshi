using UniRx;
using UniLinq;
using Noroshi.BattleScene.UI;

namespace Noroshi.BattleScene.Cache
{
    public class CharacterTextUIViewCache : AbstractCache<ICharacterTextUIView>
    {
        IFactory _factory;
        public CharacterTextUIViewCache(IFactory factory)
        {
            _factory = factory;
        }
        protected override IObservable<ICharacterTextUIView> _loadContent()
        {
            return _factory.BuildCharacterTextUI().Do(v => v.SetActive(false));
        }
        protected override IObservable<ICharacterTextUIView[]> _loadContents(int num)
        {
            return _factory.BuildCharacterTextUIMulti(num).Select(vs =>
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
