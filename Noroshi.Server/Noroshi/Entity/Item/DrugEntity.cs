using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.DrugSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;

namespace Noroshi.Server.Entity.Item
{
    public class DrugEntity : AbstractDaoWrapperEntity<DrugEntity, DrugDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<DrugEntity> ReadAndBuildMulti(IEnumerable<uint> drugIds)
        {
            var entities = ReadAndBuildMulti(drugIds.Select(id => new Schema.PrimaryKey { ID = id }));
            return _addItem(entities);
        }

        public static DrugEntity ReadAndBuild(uint id)
        {
            var entities = ReadAndBuildMulti(new List<uint> { id });
            return _addItem(entities).FirstOrDefault();
        }

        public static IEnumerable<DrugEntity> ReadAndBuildAll()
        {
            var entities = _instantiate((new DrugDao()).ReadAll());
            return _addItem(entities);
        }

        public PossessionParam GetPosssesionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.Drug,
                ID = DrugID,
                Num = num
            };
        }

        public uint DrugID => _record.ID;
        public string TextKey => "Master.Item." + _item.TextKey;
        public uint Rarity => _item.Rarity;
        public uint CharacterExp => _record.CharacterExp;

        public Core.WebApi.Response.Drug ToResponseData()
        {
            return new Core.WebApi.Response.Drug
            {
                ID = DrugID,
                CharacterExp = CharacterExp,
                TextKey = TextKey,
                Rarity = Rarity
            };
        }

        static IEnumerable<DrugEntity> _addItem(IEnumerable<DrugEntity> entities)
        {
            var drugEntities = entities as DrugEntity[] ?? entities.ToArray();
            var itemMap = (new ItemDao()).ReadMultiByPKs(drugEntities.Select(s => new ItemSchema.PrimaryKey { ID = s.DrugID })).ToDictionary(i => i.ID);
            foreach (var entity in drugEntities) {
                entity._item = itemMap[entity.DrugID];
            }
            return drugEntities;
        }

        private ItemSchema.Record _item;
    }
}
