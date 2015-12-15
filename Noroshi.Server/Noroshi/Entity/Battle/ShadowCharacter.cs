using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ShadowCharacterSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class ShadowCharacter : AbstractDaoWrapperEntity<ShadowCharacter, ShadowCharacterDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<ShadowCharacter> ReadAndBuildAll()
        {
            return (new ShadowCharacterDao()).ReadAll().Select(r => _instantiate(r));
        }

        public uint ID { get { return _record.ID; } }
        public uint CharacterID { get { return _record.CharacterID; } }
        public ushort Level { get { return _record.Level; } }
        public byte PromotionLevel { get { return _record.PromotionLevel; } }
        public byte EvolutionLevel { get { return _record.EvolutionLevel; } }
        public ushort ActionLevel1 { get { return _record.ActionLevel1; } }
        public ushort ActionLevel2 { get { return _record.ActionLevel2; } }
        public ushort ActionLevel3 { get { return _record.ActionLevel3; } }
        public ushort ActionLevel4 { get { return _record.ActionLevel4; } }
        public ushort ActionLevel5 { get { return _record.ActionLevel5; } }

        public Core.WebApi.Response.Battle.ShadowCharacter ToResponseData()
        {
            return new Core.WebApi.Response.Battle.ShadowCharacter()
            {
                ID = ID,
                CharacterID = CharacterID,
                Level = Level,
                PromotionLevel = PromotionLevel,
                EvolutionLevel = EvolutionLevel,
                ActionLevel1 = ActionLevel1,
                ActionLevel2 = ActionLevel2,
                ActionLevel3 = ActionLevel3,
                ActionLevel4 = ActionLevel4,
                ActionLevel5 = ActionLevel5,
            };
        }
    }
}