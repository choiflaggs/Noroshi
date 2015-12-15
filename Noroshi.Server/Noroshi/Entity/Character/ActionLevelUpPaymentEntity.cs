using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ActionLevelUpPaymentSchema;

namespace Noroshi.Server.Entity.Character
{
    public class ActionLevelUpPaymentEntity : AbstractDaoWrapperEntity<ActionLevelUpPaymentEntity, ActionLevelUpPaymentDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<ActionLevelUpPaymentEntity> ReadAndBuildAll()
        {
            return _instantiate(new ActionLevelUpPaymentDao().ReadAll());
        }

        public ushort Level => _record.Level;
        public uint Gold => _record.Gold;


        public Core.WebApi.Response.ActionLevelUpPayment ToResponseData()
        {
            return new Core.WebApi.Response.ActionLevelUpPayment
            {
                Level = Level,
                Gold = Gold
            };
        }
    }
}