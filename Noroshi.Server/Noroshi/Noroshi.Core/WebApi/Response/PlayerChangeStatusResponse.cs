namespace Noroshi.Core.WebApi.Response
{
    public class PlayerChangeStatusResponse
    {
        public ushort Level { get; set; }
        public ushort Stamina { get; set; }
        public uint Exp { get; set; }
        public ushort StaminaMaxCount { get; set; }
    }
}