using UniRx;
using Noroshi.BattleScene.Actions;

namespace Noroshi.BattleScene.Cache
{
    public class BulletCache : AbstractCache<IBulletView>
    {
        IActionFactory _actionFactory;
        uint _characterId;
        public BulletCache(IActionFactory actionFactory, uint characterId)
        {
            _actionFactory = actionFactory;
            _characterId = characterId;
        }
        protected override IObservable<IBulletView> _loadContent()
        {
            return _actionFactory.BuildBulletView(_characterId);
        }
        protected override IObservable<IBulletView[]> _loadContents(int num)
        {
            return _actionFactory.BuildBulletViewMulti(_characterId, num);
        }
    }
}
