using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface ILinkUIView : MonoBehaviours.IUIView
    {
        IObservable<ILinkUIView> Activate(string[] linkNames);
    }
}
