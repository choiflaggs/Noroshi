using System.Collections.Generic;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterGearSchema;

namespace Noroshi.Server.Entity.Character
{
    public class CharacterGearEntity : AbstractDaoWrapperEntity<CharacterGearEntity, CharacterGearDao, Schema.PrimaryKey, Schema.Record>
    {
        public static CharacterGearEntity ReadAndBuild(uint characterId, byte promotionLevel)
        {
            return ReadAndBuild(new Schema.PrimaryKey { CharacterID = characterId, PromotionLevel = promotionLevel });
        }

        public static IEnumerable<CharacterGearEntity> ReadAndBuildByCharacterID(uint characterId)
        {
            return ReadAndBuildByCharacterIDs(new[] { characterId });
        }
        public static IEnumerable<CharacterGearEntity> ReadAndBuildByCharacterIDs(IEnumerable<uint> characterIds)
        {
            return _instantiate((new CharacterGearDao()).ReadByCharacterIDs(characterIds));
        }


        public uint CharacterID => _record.CharacterID;
        public byte PromotionLevel => _record.PromotionLevel;

        public uint[] GearIDs => new uint[]
        {
            _record.GearID1,
            _record.GearID2,
            _record.GearID3,
            _record.GearID4,
            _record.GearID5,
            _record.GearID6,
        };
    }
}
