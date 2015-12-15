using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantLevelSchema;


namespace Noroshi.Server.Entity.Item
{
    public class GearEnchantLevelEntity : AbstractDaoWrapperEntity<GearEnchantLevelEntity, GearEnchantLevelDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GearEnchantLevelEntity> ReadAndBuildByGearID(uint gearId)
        {
            return _instantiate((new GearEnchantLevelDao()).ReadByGearID(gearId));
        }

        public static GearEnchantLevelEntity ReadAndBuild(uint gearId, byte level)
        {
            return _instantiate((new GearEnchantLevelDao()).ReadByPK(new Schema.PrimaryKey {EnchantLevel = level, GearID = gearId}));
        }

        public static IEnumerable<GearEnchantLevelEntity> ReadAndBuildAll()
        {
            return _instantiate((new GearEnchantLevelDao()).ReadAll());
        }

        public uint GearID => _record.GearID;
        public byte EnchantLevel => _record.EnchantLevel;
        public uint Gold => _record.Gold;
        public float GrowthRate => _record.GrowthRate;
        public uint Exp => _record.Exp;

        public Core.WebApi.Response.GearEnchantLevel ToResponseData()
        {
            return new Core.WebApi.Response.GearEnchantLevel
            {
                GearID = GearID,
                EnchantLevel = EnchantLevel,
                Gold = Gold,
                GrowthRate = GrowthRate,
                Exp = Exp
            };
        }
    }
}
