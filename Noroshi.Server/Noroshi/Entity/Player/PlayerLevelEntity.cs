using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerLevelSchema;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerLevelEntity : AbstractLevelExpEntity<PlayerLevelEntity, PlayerLevelDao, Schema.PrimaryKey, Schema.Record>
    {
        public override ushort Level => _record.Level;
        public override uint Exp => _record.Exp;

        public Core.WebApi.Response.Master.PlayerLevel ToResponseData()
        {
            return new Core.WebApi.Response.Master.PlayerLevel
            {
                Level = Level,
                Exp = Exp,
            };
        }
    }
}
