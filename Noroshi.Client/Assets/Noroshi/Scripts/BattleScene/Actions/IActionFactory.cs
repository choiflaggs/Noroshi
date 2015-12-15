using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public interface IActionFactory
    {
        IObservable<IActionView> BuildActionView(uint characterId);
        IObservable<IActionRelationView> BuildActionRelationView(uint characterId);
        IObservable<IBulletView> BuildBulletView(uint characterId);
        IObservable<IBulletView[]> BuildBulletViewMulti(uint characterId, int num);
    }
}
