using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerVipLevelSchema;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerVipLevelEntity : AbstractLevelExpEntity<PlayerVipLevelEntity, PlayerVipLevelDao, Schema.PrimaryKey, Schema.Record>
    {
        public override ushort Level => _record.Level;
        public override uint Exp => _record.Exp;

        public Core.WebApi.Response.Master.PlayerVipLevel ToResponseData()
        {
            return new Core.WebApi.Response.Master.PlayerVipLevel
            {
                Level = Level,
                Exp = Exp,
            };
        }
    }
}
