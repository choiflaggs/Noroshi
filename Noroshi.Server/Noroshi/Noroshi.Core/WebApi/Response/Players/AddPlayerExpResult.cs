namespace Noroshi.Core.WebApi.Response.Players
{
    public class AddPlayerExpResult
    {
        public ushort PreviousPlayerLevel { get; set; }
        public ushort CurrentPlayerLevel { get; set; }
        public ushort PreviousMaxStamina { get; set; }
        public ushort CurrentMaxStamina { get; set; }
        public bool LevelUp { get; set; }
        public uint[] OpenGameContentIDs { get; set; }
    }
}
