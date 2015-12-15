using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearPieceSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class GearPieceEntity : AbstractDaoWrapperEntity<GearPieceEntity, GearPieceDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GearPieceEntity> ReadAndBuildMulti(IEnumerable<uint> drugIds)
        {
            var entities = ReadAndBuildMulti(drugIds.Select(id => new Schema.PrimaryKey { ID = id }));
            return _addItem(entities);
        }

        public PossessionParam GetPosssesionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.GearPiece,
                ID = GearPieceID,
                Num = num
            };
        }

        public static IEnumerable<GearPieceEntity> ReadAndBuildAll()
        {
            var entities = _instantiate((new GearPieceDao()).ReadAll());
            return _addItem(entities);
        }

        public uint GearPieceID => _record.ID;
        public string TextKey => "Master.Item." + _item.TextKey;
        public uint Rarity => _item.Rarity;
        public uint EnchantExp => _record.EnchantExp;

        public Core.WebApi.Response.GearPiece ToResponseData()
        {
            return new Core.WebApi.Response.GearPiece
            {
                ID = GearPieceID,
                EnchantExp = EnchantExp,
                TextKey = TextKey,
                Rarity = Rarity
            };
        }

        static IEnumerable<GearPieceEntity> _addItem(IEnumerable<GearPieceEntity> entities)
        {
            var gearPieceEntities = entities as GearPieceEntity[] ?? entities.ToArray();
            var itemMap = (new ItemDao()).ReadMultiByPKs(gearPieceEntities.Select(s => new ItemSchema.PrimaryKey { ID = s.GearPieceID })).ToDictionary(i => i.ID);
            foreach (var entity in gearPieceEntities)
            {
                entity._item = itemMap[entity.GearPieceID];
            }
            return gearPieceEntities;
        }

        private ItemSchema.Record _item;
    }
}
