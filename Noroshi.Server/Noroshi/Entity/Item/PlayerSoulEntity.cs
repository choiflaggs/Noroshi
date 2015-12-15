using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerSoulEntity : AbstractDaoWrapperEntity<PlayerSoulEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerSoulEntity> ReadAndBuildMulti(uint playerId, IEnumerable<uint> soulIds, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(soulIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }), readType);
        }
        public static IEnumerable<PlayerSoulEntity> ReadAndBuildAll(uint playerId)
        {
            // TODO : PlayerItem にカラム追加不要かどうかは検討。
            var soulEntities = SoulEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, soulEntities.Select(soul => soul.SoulID));
        }

        public uint SoulID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerSoul ToResponseData()
        {
            return new Core.WebApi.Response.PlayerSoul
            {
                SoulID = SoulID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
