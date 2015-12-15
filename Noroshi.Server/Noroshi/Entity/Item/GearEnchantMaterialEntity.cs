using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Noroshi.Server.Entity.Possession;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantMaterialSchema;
using ItemSchema = Noroshi.Server.Daos.Rdb.Schemas.ItemSchema;


namespace Noroshi.Server.Entity.Item
{
    public class GearEnchantMaterialEntity : AbstractDaoWrapperEntity<GearEnchantMaterialEntity, GearEnchantMaterialDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GearEnchantMaterialEntity> ReadAndBuildMulti(IEnumerable<uint> gearEnchantMaterialIds)
        {
            var entities = ReadAndBuildMulti(gearEnchantMaterialIds.Select(id => new Schema.PrimaryKey { ID = id }));
            return _addItem(entities);
        }


        public static IEnumerable<GearEnchantMaterialEntity> ReadAndBuildAll()
        {
            var entities = _instantiate((new GearEnchantMaterialDao()).ReadAll());
            return _addItem(entities);
        }

        public uint GearEnchantMaterialID => _record.ID;
        public string TextKey => "Master.Item." + _item.TextKey;
        public uint Rarity => _item.Rarity;
        public uint EnchantExp => _record.EnchantExp;

        public Core.WebApi.Response.GearEnchantMaterial ToResponseData()
        {
            return new Core.WebApi.Response.GearEnchantMaterial
            {
                ID = GearEnchantMaterialID,
                EnchantExp = EnchantExp,
                TextKey = TextKey,
                Rarity = Rarity
            };
        }

        public static IEnumerable<PossessionParam> ConvertToExpToEnchantMaterial(uint exp)
        {
            var enchantMaterials = ReadAndBuildAll().OrderByDescending(enchantMaterial => enchantMaterial.EnchantExp).ToList();
            var remainingExp = exp;
            var posessions = enchantMaterials.Select(enchantMaterial =>
            {
                var num = remainingExp / enchantMaterial.EnchantExp;
                remainingExp %= enchantMaterial.EnchantExp;
                return enchantMaterial.GetPosssesionParam(num);
            });
            return posessions;
        }

        public PossessionParam GetPosssesionParam(uint num)
        {
            return new PossessionParam
            {
                Category = PossessionCategory.GearEnchantMaterial,
                ID = GearEnchantMaterialID,
                Num = num
            };
        }


        static IEnumerable<GearEnchantMaterialEntity> _addItem(IEnumerable<GearEnchantMaterialEntity> entities)
        {
            var gearEnchantMaterialEntities = entities as GearEnchantMaterialEntity[] ?? entities.ToArray();
            var itemMap = (new ItemDao()).ReadMultiByPKs(gearEnchantMaterialEntities.Select(s => new ItemSchema.PrimaryKey { ID = s.GearEnchantMaterialID })).ToDictionary(i => i.ID);
            foreach (var entity in gearEnchantMaterialEntities) {
                entity._item = itemMap[entity.GearEnchantMaterialID];
            }
            return gearEnchantMaterialEntities;
        }

        private ItemSchema.Record _item;
    }
}
