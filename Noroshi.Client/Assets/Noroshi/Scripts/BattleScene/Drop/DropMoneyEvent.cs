namespace Noroshi.BattleScene.Drop
{
    public class DropMoneyEvent
    {
        public uint CurrentTotalMoney { get; set; }
        public uint Money { get; set; }
        public ICharacterView CharacterView { get; set; }
    }
}