using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IDropItemUIView : MonoBehaviours.IUIView
    {
        /// アイテムドロップ演出開始メソッド。拾われたらプッシュされる Observable を返す。
        IObservable<bool> Drop(ICharacterView characterView, byte dropItemNo);
        /// アイテム取得演出開始メソッド。演出が完了したらプッシュされる Observable を返す。
        IObservable<IDropItemUIView> Gain();
    }
}