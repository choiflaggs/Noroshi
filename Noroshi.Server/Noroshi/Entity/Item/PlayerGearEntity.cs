using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerGearEntity : AbstractDaoWrapperEntity<PlayerGearEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerGearEntity> ReadAndBuildMulti(uint playerId, IEnumerable<GearEntity> gearEntities)
        {
            var gearIds = gearEntities.Select(data => data.GearID);
            var entities = ReadAndBuildMulti(gearIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }));
            return entities;
        }

        public static PlayerGearEntity ReadAndBuild(uint playerId, GearEntity gearEntity)
        {
            var entity = ReadAndBuild(new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID =  gearEntity.GearID });
            return entity;
        }


        public static IEnumerable<PlayerGearEntity> ReadAndBuildAll(uint playerId)
        {
            var gearEntities = GearEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, gearEntities);
        }

        public uint GearID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerGear ToResponseData()
        {
            return new Core.WebApi.Response.PlayerGear
            {
                GearID = GearID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
