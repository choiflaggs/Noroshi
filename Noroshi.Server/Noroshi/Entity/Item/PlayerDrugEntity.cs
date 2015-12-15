using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;

namespace Noroshi.Server.Entity.Item
{
    public class PlayerDrugEntity : AbstractDaoWrapperEntity<PlayerDrugEntity, PlayerItemDao, PlayerItemDao.PrimaryKey, PlayerItemDao.Record>
    {
        public static IEnumerable<PlayerDrugEntity> ReadAndBuildMulti(uint playerId, IEnumerable<DrugEntity> drugEntities)
        {
            var drugIds = drugEntities.Select(data => data.DrugID);
            var entities = ReadAndBuildMulti(drugIds.Select(id => new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = id }));
            return entities;
        }

        public static PlayerDrugEntity ReadAndBuild(uint playerId, DrugEntity drugEntity)
        {
            var entity = ReadAndBuild(new PlayerItemDao.PrimaryKey {PlayerID = playerId, ItemID = drugEntity.DrugID });
            return entity;
        }


        public static IEnumerable<PlayerDrugEntity> ReadAndBuildAll(uint playerId)
        {
            var drugEntities = DrugEntity.ReadAndBuildAll();
            return ReadAndBuildMulti(playerId, drugEntities);
        }

        public uint DrugID => _record.ItemID;
        public uint PossessionsCount => _record.PossessionsCount;

        public Core.WebApi.Response.PlayerDrug ToResponseData()
        {
            return new Core.WebApi.Response.PlayerDrug
            {
                DrugID = DrugID,
                PossessionsCount = PossessionsCount
            };
        }
    }
}
