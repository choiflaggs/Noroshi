using AudioClip = UnityEngine.AudioClip;
using Noroshi.Core.Game.Sound;

namespace Noroshi.BattleScene.Sound
{
    public interface ISound
    {
        uint ID { get; }
        PlayType PlayType { get; }
        AudioClip GetAudioClip();
        AudioClip GetIntro();
        float? GetLength();
    }
}