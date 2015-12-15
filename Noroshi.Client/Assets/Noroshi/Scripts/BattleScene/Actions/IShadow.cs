using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public interface IShadow
    {
        IObservable<IShadow> LoadDatasAndAssets(IActionFactory factory);
    }
}