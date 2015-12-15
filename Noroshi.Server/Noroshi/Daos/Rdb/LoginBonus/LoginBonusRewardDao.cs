using System.Collections.Generic;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.LoginBonusRewardSchema;

namespace Noroshi.Server.Daos.Rdb.LoginBonus
{
    public class LoginBonusRewardDao : AbstractCommonDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByLoginBonusIDs(IEnumerable<uint> loginBonusIds)
        {
            return _select("LoginBonusID IN @LoginBonusIDs", new { LoginBonusIDs = loginBonusIds });
        }
    }
}
