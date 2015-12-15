using System.Collections.Generic;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class RaidTicketEntity : AbstractDaoWrapperEntity<RaidTicketEntity, ItemDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<RaidTicketEntity> ReadAndBuildAll()
        {
            return new[] { _instantiate((new ItemDao()).ReadByRaidTicket()) };
        }

        public static IEnumerable<RaidTicketEntity> ReadAndBuildMulti(uint[] raidTicketIds)
        {
            return new[] { _instantiate((new ItemDao()).ReadByRaidTicket()) };
        }

        public PossessionParam GetUsingRaidTicketPosssesionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.RaidTicket,
                ID = RaidTicketID,
                Num = num
            };
        }

        public uint RaidTicketID => _record.ID;
        public string TextKey => "Master.Item." + _record.TextKey;
        public uint Rarity => _record.Rarity;

        public Core.WebApi.Response.RaidTicket ToResponseData()
        {
            return new Core.WebApi.Response.RaidTicket
            {
                ID = RaidTicketID,
                TextKey = TextKey,
                Rarity = Rarity
            };
        }
    }
}
