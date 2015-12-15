using System.Collections.Generic;
using System.Linq;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerItemDao : AbstractShardDBDao<PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => "player_item";

        public class Record : AbstractRecord
        {
            public uint PlayerID { get; set; }
            public uint ItemID { get; set; }
            public uint PossessionsCount { get; set; }
        }

        public class PrimaryKey : IPrimaryKey
        {
            public uint PlayerID { get; set; }
            public uint ItemID { get; set; }
        }
        /* ここまでテーブルマッピング設定 */

        public IEnumerable<Record> ReadByPlayerID(uint playerId)
        {
            return _select("PlayerID = @PlayerID", new { PlayerID = playerId });
        }

        public Record ReadByPlayerIDAndItemID(uint playerId, uint itemId, ReadType readType = ReadType.Slave)
        {
            return _select("PlayerID = @PlayerID AND ItemID = @ItemID", new {PlayerID = playerId, ItemID = itemId}, readType).FirstOrDefault();
        }
        public IEnumerable<Record> ReadByPlayerIDAndItemIDs(uint playerId, IEnumerable<uint> itemIds, ReadType readType = ReadType.Slave)
        {
            return _select("PlayerID = @PlayerID AND ItemID IN @ItemIDs", new { PlayerID = playerId, ItemIDs = itemIds }, readType);
        }

        // ギャップロック回避のためロックをかけたい場合には最初に Insert を試みてしまう。
        public Record CreateOrSelect(uint playerId, uint itemId)
        {
            var record = _create(playerId, itemId) ?? ReadByPlayerIDAndItemID(playerId, itemId, ReadType.Lock);
            return record;
        }
        public IEnumerable<Record> CreateOrReadMulti(uint playerId, IEnumerable<uint> itemIds)
        {
            var map = new Dictionary<uint, Record>();
            var existItemIds = new List<uint>();
            foreach (var itemId in itemIds)
            {
                var record = _create(playerId, itemId);
                if (record != null)
                {
                    map.Add(record.ItemID, record);
                }
                else
                {
                    existItemIds.Add(itemId);
                }
            }
            if (!existItemIds.Any()) return itemIds.Select(itemId => map[itemId]);
            var existRecords = ReadByPlayerIDAndItemIDs(playerId, existItemIds, ReadType.Lock);
            foreach (var record in existRecords)
            {
                map.Add(record.ItemID, record);
            }
            return itemIds.Select(itemId => map[itemId]);
        }

        Record _create(uint playerId, uint itemId)
        {
            var record = new Record { PlayerID = playerId, ItemID = itemId, PossessionsCount = 0 };
            return Create(record);
        }
    }
}