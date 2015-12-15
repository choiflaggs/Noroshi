namespace Noroshi.BattleScene.Drop
{
    public enum DropCommand
    {
        Drop,
        PickUp,
    }
    public class DropItemEvent
    {
        public DropCommand Command { get; set; }
        public byte No { get; set; }
        public uint ItemID { get; set; }
        public byte ItemNum { get; set; }
        public System.Func<byte> CurrentTotalNumFunc { get; set; }
        public ICharacterView CharacterView { get; set; }
    }
}