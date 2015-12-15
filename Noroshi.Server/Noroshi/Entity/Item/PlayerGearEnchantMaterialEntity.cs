using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerGearEnchantMaterialEntity : AbstractDaoWrapperEntity<PlayerGearEnchantMaterialEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerGearEnchantMaterialEntity> ReadAndBuildMulti(uint playerId, IEnumerable<GearEnchantMaterialEntity> gearEnchantMaterialEntities)
        {
            var gearEnchantMaterialIds = gearEnchantMaterialEntities.Select(data => data.GearEnchantMaterialID);
            var entities = ReadAndBuildMulti(gearEnchantMaterialIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }));
            return entities;
        }

        public static PlayerGearEnchantMaterialEntity ReadAndBuild(uint playerId, GearEnchantMaterialEntity gearEnchantMaterialEntity)
        {
            var entity = ReadAndBuild(new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = gearEnchantMaterialEntity.GearEnchantMaterialID });
            return entity;
        }


        public static IEnumerable<PlayerGearEnchantMaterialEntity> ReadAndBuildAll(uint playerId)
        {
            var gearEnchantMaterialEntities = GearEnchantMaterialEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, gearEnchantMaterialEntities);
        }

        public uint GearEnchantMaterialID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerGearEnchantMaterial ToResponseData()
        {
            return new Core.WebApi.Response.PlayerGearEnchantMaterial
            {
                GearEnchantMaterialID = GearEnchantMaterialID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
