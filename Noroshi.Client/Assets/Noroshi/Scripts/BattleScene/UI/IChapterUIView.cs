using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IChapterUIView : MonoBehaviours.IUIView
    {
        IObservable<IChapterUIView> Activate(string titleTextKey);
    }
}
