using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterLevelSchema;

namespace Noroshi.Server.Entity.Character
{
    public class CharacterLevelEntity : AbstractLevelExpEntity<CharacterLevelEntity, CharacterLevelDao, Schema.PrimaryKey, Schema.Record>
    {
        public override ushort Level => _record.Level;
        public override uint Exp => _record.Exp;

        public Core.WebApi.Response.Master.CharacterLevel ToResponseData()
        {
            return new Core.WebApi.Response.Master.CharacterLevel
            {
                Level = Level,
                Exp = Exp,
            };
        }
    }
}
