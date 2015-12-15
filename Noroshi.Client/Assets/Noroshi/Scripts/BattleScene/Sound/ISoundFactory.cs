using System.Collections.Generic;
using UniRx;

namespace Noroshi.BattleScene.Sound
{
    public interface ISoundFactory
    {
        IObservable<ISoundPlayer> LoadBgmPlayer();
        IObservable<ISoundPlayer> LoadSEPlayer();
        IObservable<ISoundPlayer> LoadVoicePlayer();
        IObservable<ISound[]> LoadSounds(IEnumerable<Core.WebApi.Response.Master.Sound> sounds);
    }
}
