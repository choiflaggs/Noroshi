using UniRx;

namespace Noroshi.BattleScene.Sound
{
    public interface ISoundPlayer
    {
        IObservable<bool> Play(ISound sound);
        void Stop(ISound sound);
    }
}
