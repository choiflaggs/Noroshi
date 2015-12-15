using Noroshi.Core.WebApi.Response.Possession;

namespace Noroshi.Core.WebApi.Response
{
    public class UseRaidTicketResponse
    {
        public PossessionObject GetGold { get; set; }
        public PossessionObject GetExp { get; set; }
        public PossessionObject UseRaidTicket { get; set; }
        public ushort Stamina { get; set; }
        public PossessionObject[][] DropRewards { get; set; }
    }
}
