namespace Noroshi.BattleScene.Sound
{
    public enum SoundCommand
    {
        Play,
        Stop,
    }
    public class SoundEvent
    {
        public SoundCommand Command;
        public uint SoundID;
    }
}
