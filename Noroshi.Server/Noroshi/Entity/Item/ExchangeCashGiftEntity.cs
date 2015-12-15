using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ExchangeCashGiftSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class ExchangeCashGiftEntity : AbstractDaoWrapperEntity<ExchangeCashGiftEntity, ExchangeCashGiftDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<ExchangeCashGiftEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return _loadAssociatedRecords(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })));
        }
        public static IEnumerable<ExchangeCashGiftEntity> ReadAndBuildAll()
        {
            return _loadAssociatedRecords(_instantiate((new ExchangeCashGiftDao()).ReadAll()));
        }
        static IEnumerable<ExchangeCashGiftEntity> _loadAssociatedRecords(IEnumerable<ExchangeCashGiftEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var itemMap = (new ItemDao()).ReadMultiByPKs(entities.Select(exchangeCrashGift => new ItemSchema.PrimaryKey { ID = exchangeCrashGift.ExchangeCashGiftID })).ToDictionary(item => item.ID);
            return entities.Select(entity =>
            {
                entity._setItemRecord(itemMap[entity.ExchangeCashGiftID]);
                return entity;
            });
        }


        ItemSchema.Record _itemRecord;

        void _setItemRecord(ItemSchema.Record itemRecord)
        {
            if (ExchangeCashGiftID != itemRecord.ID) throw new InvalidOperationException();
            _itemRecord = itemRecord;
        }


        public uint ExchangeCashGiftID => _record.ID;
        public string TextKey => "Master.Item." + _itemRecord.TextKey;
        public uint Rarity => _itemRecord.Rarity;
        public uint Gold => _record.Gold;

        public Core.WebApi.Response.ExchangeCashGift ToResponseData()
        {
            return new Core.WebApi.Response.ExchangeCashGift
            {
                ID = ExchangeCashGiftID,
                Gold = Gold,
                TextKey = TextKey,
                Rarity = Rarity
            };
        }
    }
}
