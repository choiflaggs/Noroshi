using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerGearPieceEntity : AbstractDaoWrapperEntity<PlayerGearPieceEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerGearPieceEntity> ReadAndBuildMulti(uint playerId, IEnumerable<GearPieceEntity> gearPieceEntities)
        {
            var gearPieceIds = gearPieceEntities.Select(data => data.GearPieceID);
            var entities = ReadAndBuildMulti(gearPieceIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }));
            return entities;
        }

        public static PlayerGearPieceEntity ReadAndBuild(uint playerId, GearPieceEntity gearPieceEntity)
        {
            var entity = ReadAndBuild(new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = gearPieceEntity.GearPieceID });
            return entity;
        }


        public static IEnumerable<PlayerGearPieceEntity> ReadAndBuildAll(uint playerId)
        {
            var gearPieceEntities = GearPieceEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, gearPieceEntities);
        }

        public uint GearPieceID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerGearPiece ToResponseData()
        {
            return new Core.WebApi.Response.PlayerGearPiece
            {
                GearPieceID = GearPieceID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
