using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class RaidTicket : IPossessionObject
    {
        private readonly RaidTicketEntity _raidTicket;

        public RaidTicket(RaidTicketEntity raidTicket, uint num, uint possessionNum)
        {
            _raidTicket = raidTicket;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.RaidTicket;
        public uint ID => _raidTicket.RaidTicketID;
        public uint Num
        { get; }

        public string TextKey => _raidTicket.TextKey;
        public uint PossessingNum
        { get; }

        public Core.WebApi.Response.Possession.PossessionObject ToResponseData()
        {
            return new Core.WebApi.Response.Possession.PossessionObject
            {
                Category = (byte)Category,
                ID = ID,
                Num = Num,

                Name = TextKey,
                PossessingNum = PossessingNum,
            };
        }
    }
}
