using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IStoryUIView : MonoBehaviours.IUIView
    {
        IObservable<bool> Begin();
        IObservable<IStoryUIView> GoNext(ICharacterView ownCharacterView, ICharacterView enemyCharacterView, string textKey, string characterNameTextKey);
        IObservable<bool> Finish();
    }
}
