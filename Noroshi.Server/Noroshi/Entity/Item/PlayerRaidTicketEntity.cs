using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerRaidTicketEntity : AbstractDaoWrapperEntity<PlayerRaidTicketEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerRaidTicketEntity> ReadAndBuildMulti(uint playerId, IEnumerable<RaidTicketEntity> raidTicketEntities)
        {
            var raidTicketIds = raidTicketEntities.Select(data => data.RaidTicketID);
            var entities = ReadAndBuildMulti(raidTicketIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }));
            return entities;
        }

        public static PlayerRaidTicketEntity ReadAndBuild(uint playerId, RaidTicketEntity raidTicketEntity)
        {
            var entity = ReadAndBuild(new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = raidTicketEntity.RaidTicketID });
            return entity;
        }


        public static IEnumerable<PlayerRaidTicketEntity> ReadAndBuildAll(uint playerId)
        {
            var raidTicketEntities = RaidTicketEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, raidTicketEntities);
        }

        public uint RaidTicketID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerRaidTicket ToResponseData()
        {
            return new Core.WebApi.Response.PlayerRaidTicket
            {
                RaidTicketID = RaidTicketID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
