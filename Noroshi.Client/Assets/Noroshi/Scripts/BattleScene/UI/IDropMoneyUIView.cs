using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IDropMoneyUIView : MonoBehaviours.IUIView
    {
        IObservable<bool> Drop(ICharacterView characterView);
    }
}