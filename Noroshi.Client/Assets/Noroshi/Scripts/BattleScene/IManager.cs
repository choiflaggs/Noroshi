using UniRx;

namespace Noroshi.BattleScene
{
    public interface IManager
    {
        void Initialize();
        IObservable<IManager> LoadDatas();
        IObservable<IManager> LoadAssets(IFactory factory);
        void Prepare();
        void Dispose();
    }
}
