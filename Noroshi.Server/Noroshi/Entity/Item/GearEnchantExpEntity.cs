using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Item;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GearEnchantExpSchema;


namespace Noroshi.Server.Entity.Item
{
    public class GearEnchantExpEntity : AbstractDaoWrapperEntity<GearEnchantExpEntity, GearEnchantExpDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<GearEnchantExpEntity> ReadAndBuildMulti(IEnumerable<uint> gearIds)
        {
            return ReadAndBuildMulti(gearIds.Select(id => new Schema.PrimaryKey { GearID = id }));
        }

        public static GearEnchantExpEntity ReadAndBuild(uint gearId)
        {
            return ReadAndBuild(new Schema.PrimaryKey { GearID = gearId });
        }

        public static IEnumerable<GearEnchantExpEntity> ReadAndBuildAll()
        {
            return _instantiate((new GearEnchantExpDao()).ReadAll());
        }

        public uint GearID => _record.GearID;
        public uint EnchantExp => _record.EnchantExp;

        public Core.WebApi.Response.GearEnchantExp ToResponseData()
        {
            return new Core.WebApi.Response.GearEnchantExp
            {
                GearID = GearID,
                EnchantExp = EnchantExp
            };
        }
    }
}
