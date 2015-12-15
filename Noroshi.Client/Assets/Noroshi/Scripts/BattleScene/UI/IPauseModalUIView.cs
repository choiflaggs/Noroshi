using UniRx;

namespace Noroshi.BattleScene.UI
{
    public interface IPauseModalUIView : UI.IModalUIView
    {
        IObservable<bool> GetOnClickWithdrawalObservable();
    }
}
