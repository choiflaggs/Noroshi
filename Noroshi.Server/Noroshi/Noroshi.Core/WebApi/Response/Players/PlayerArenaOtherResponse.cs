namespace Noroshi.Core.WebApi.Response.Players
{
    public class PlayerArenaOtherResponse
    {
        public uint Rank { get; set; }
        public PlayerCharacter[] DeckCharacters { get; set; }
        public uint Win { get; set; }
        public uint Lose { get; set; }
        public uint DefenseWin { get; set; }
        public uint DefenseLose { get; set; }
        public uint AllHP { get; set; }
        public uint AllStrength { get; set; }
        public OtherPlayerStatus OtherPlayerStatus { get; set; }
    }
}