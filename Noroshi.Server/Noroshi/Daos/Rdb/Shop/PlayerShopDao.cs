using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerShopSchema;

namespace Noroshi.Server.Daos.Rdb.Shop
{
    public class PlayerShopDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> CreateOrReadMulti(uint playerId, IEnumerable<uint> shopIds)
        {
            var createdRecordMap = shopIds.Select(shopId => Create(playerId, shopId)).Where(record => record != null).ToDictionary(record => record.ShopID);
            var notCreatedPks = shopIds.Where(shopId => !createdRecordMap.ContainsKey(shopId)).Select(shopId => new Schema.PrimaryKey { PlayerID = playerId, ShopID = shopId });
            var readRecordMap = notCreatedPks.Count() > 0 ? ReadMultiByPKs(notCreatedPks, ReadType.Lock).ToDictionary(record => record.ShopID) : new Dictionary<uint, Schema.Record>();
            var map = (new Dictionary<uint, Schema.Record>()).Concat(createdRecordMap).Concat(readRecordMap).ToDictionary(kv => kv.Key, kv => kv.Value);
            return shopIds.Select(shopId => map[shopId]);
        }

        public Schema.Record Create(uint playerId, uint shopId)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                ShopID = shopId,
                Merchandises = "",
                BoughtDisplayNos = "",
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }
    }
}
