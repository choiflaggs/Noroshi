namespace Noroshi.Core.WebApi.Response
{
    public class PlayerCharacter : AbstractPersonalCharacter
    {
        public uint ID { get; set; }
        public uint Exp { get; set; }
        public uint ExpInLevel { get; set; }
        public PlayerCharacterGear[] PlayerCharacterGears { get; set; } 
    }
}