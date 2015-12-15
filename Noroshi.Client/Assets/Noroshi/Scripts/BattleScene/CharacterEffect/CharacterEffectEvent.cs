namespace Noroshi.BattleScene.CharacterEffect
{
    public enum CharacterEffectCommand
    {
        Play,
        Stop,
        PlayOnce,
        Interrupt,
    }
    public class CharacterEffectEvent
    {
        public CharacterEffectCommand Command;
        public ICharacterView CharacterView;
        public uint CharacterEffectID;
    }
}