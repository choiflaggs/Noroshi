using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerItemEntity : AbstractDaoWrapperEntity<PlayerItemEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerItemEntity> CreateOrReadAndBuildMulti(uint playerId, IEnumerable<uint> itemIds)
        {
            return (new PlayerItemDao()).CreateOrReadMulti(playerId, itemIds).Select(r => _instantiate(r));
        }

        public static PlayerItemEntity CreateOrReadAndBuild(uint playerId, uint itemId)
        {
            return _instantiate((new PlayerItemDao()).CreateOrSelect(playerId, itemId));
        }

        public uint PlayerID => _record.PlayerID;
        public uint ItemID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerItem ToResponseData()
        {
            return new Core.WebApi.Response.PlayerItem
            {
                PlayerID = PlayerID,
                ItemID = ItemID,
                PossessionsCount = PossessionsCount
            };
        }
        public static IEnumerable<PlayerItemEntity> ReadAndBuildMultiByPlayerID(uint playerId)
        {
            var dao = new PlayerItemDao();
            return dao.ReadByPlayerID(playerId).Select(r => _instantiate(r));
        }

        public static IEnumerable<PlayerItemEntity> ReadAndBuildMultiByPlayerIDAndItemIDs(uint playerId, IEnumerable<uint> itemIds, ReadType readType = ReadType.Slave)
        {
            var pks = itemIds.Select(itemId => new PlayerItemDao.PrimaryKey { PlayerID = playerId, ItemID = itemId });
            return ReadAndBuildMulti(pks, readType);
        }

        public static PlayerItemEntity ReadAndBuildByPlayerIDAndItemID(uint playerId, uint itemId, ReadType readType = ReadType.Slave)
        {
            var dao = new PlayerItemDao();
            var playerItemData = dao.ReadByPlayerIDAndItemID(playerId, itemId);
            return _instantiate(playerItemData);
        }

        // çÌèúó\íË
        public void SetNum(uint num)
        {
            var record = _cloneRecord();
            record.PossessionsCount = num;
            _changeLocalRecord(record);
        }
    }
}