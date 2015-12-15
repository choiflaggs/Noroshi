using System.Collections.Generic;
using System.Linq;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerCharacterGearSchema;

namespace Noroshi.Server.Daos.Rdb.Character
{
    public class PlayerCharacterGearDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        public IEnumerable<Schema.Record> ReadByPlayerCharacterIDAndPromotionLevel(uint playerCharacterId, byte promotionLevel)
        {
            return _select("PlayerCharacterID = @PlayerCharacterID AND PromotionLevel = @PromotionLevel", new { PlayerCharacterID = playerCharacterId, PromotionLevel = promotionLevel });

        }

        public Schema.Record ReadByPlayerCharacterIDAndPromotionLevelAndGearPosition(uint playerCharacterId, byte promotionLevel, byte gearPosition)
        {
            return _select("PlayerCharacterID = @PlayerCharacterID AND PromotionLevel = @PromotionLevel AND GearPosition = @GearPosition", new { PlayerCharacterID = playerCharacterId, PromotionLevel = promotionLevel, GearPosition = gearPosition}).FirstOrDefault();
        }

        public Schema.Record Create(uint playerCharacterId, uint gearId,  byte promotionLevel, byte gearPosition)
        {
            var record = new Schema.Record {ID = (new SequentialIDTable(_tableName)).GenerateID(), PlayerCharacterID = playerCharacterId, Exp = 0, GearID = gearId, GearPosition = gearPosition, PromotionLevel = promotionLevel};
            return Create(record);
        }

    }
}
