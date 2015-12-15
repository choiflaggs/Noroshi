using UniRx;
using Noroshi.BattleScene.Sound;

namespace Noroshi.BattleScene.UI
{
    public interface IUIManager : IManager
    {
        IObservable<bool> GetOnTransitSceneObservable();
        IObservable<bool> GetOnTogglePauseObservable();
        IObservable<SoundEvent> GetOnCommandSoundObservable();
    }
}
