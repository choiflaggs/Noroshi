using System.Collections.Generic;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerLoginBonusSchema;

namespace Noroshi.Server.Daos.Rdb.LoginBonus
{
    public class PlayerLoginBonusDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /* ここからテーブルマッピング設定 */
        protected override string _tableName => "player_login_bonus";


        public Schema.Record Create(uint playerId, uint loginBonusId, byte currentNum)
        {
            var record = new Schema.Record
            {
                PlayerID = playerId,
                LoginBonusID = loginBonusId,
                CurrentNum = currentNum,
                ReceiveRewardThreshold = 0,
                LastLoggedInAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
            return Create(record);
        }
    }
}
