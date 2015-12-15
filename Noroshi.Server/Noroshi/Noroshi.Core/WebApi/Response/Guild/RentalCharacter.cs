namespace Noroshi.Core.WebApi.Response.Guild
{
    public class RentalCharacter
    {
        public byte No { get; set; }
        public PlayerCharacter PlayerCharacter { get; set; }
        public uint CreatedAt { get; set; }
    }
}
