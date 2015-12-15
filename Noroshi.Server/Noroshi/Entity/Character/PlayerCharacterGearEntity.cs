using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Noroshi.Server.Entity.Item;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerCharacterGearSchema;

namespace Noroshi.Server.Entity.Character
{
    public class PlayerCharacterGearEntity : AbstractDaoWrapperEntity<PlayerCharacterGearEntity, PlayerCharacterGearDao, Schema.PrimaryKey, Schema.Record>
    {

        public static PlayerCharacterGearEntity ReadAndBuild(uint playerCharacterId, byte promotionLevel, byte gearPosition)
        {
            return _instantiate((new PlayerCharacterGearDao()).ReadByPlayerCharacterIDAndPromotionLevelAndGearPosition(playerCharacterId, promotionLevel, gearPosition));
        }

        public static PlayerCharacterGearEntity ReadAndBuild(uint playerCharacterGearId, ReadType readType=ReadType.Slave)
        {
            return _instantiate((new PlayerCharacterGearDao()).ReadByPK(new Schema.PrimaryKey {ID = playerCharacterGearId }, readType));
        }

        public static IEnumerable<PlayerCharacterGearEntity> ReadAndBuildMulti(IEnumerable<uint> playerCharacterGearIds)
        {
            return _instantiate((new PlayerCharacterGearDao()).ReadMultiByPKs(playerCharacterGearIds.Select(id => new Schema.PrimaryKey { ID = id })));
        }

        public static IEnumerable<PlayerCharacterGearEntity> ReadAndBuildByPlayerCharacterIDAndPromotionLevel(uint playerCharacterId, byte promotionLevel)
        {
            return _instantiate((new PlayerCharacterGearDao()).ReadByPlayerCharacterIDAndPromotionLevel(playerCharacterId, promotionLevel));
        }

        public static PlayerCharacterGearEntity Create(uint playerCharacterId, uint gearId, byte promotionLevel,
            byte gearPosition)
        {
            return
                _instantiate((new PlayerCharacterGearDao()).Create(playerCharacterId, gearId, promotionLevel,
                    gearPosition));
        }

        public void SetExp(uint playerExp)
        {
            var newRecord = _cloneRecord();
            newRecord.Exp = playerExp;
            var gear = GearEntity.ReadAndBuild(GearID);
            if (newRecord.Exp > gear.MaxEnchantExp) {
                newRecord.Exp = gear.MaxEnchantExp;
            }
            newRecord.PromotionLevel = gear.GetLevel(newRecord.Exp);
            _changeLocalRecord(newRecord);
        }
        public void AddExp(uint playerExp)
        {
            SetExp(Exp + playerExp);
        }

        public uint GearID => _record.GearID;
        public uint Exp => _record.Exp;
        public byte PromotionLevel => _record.PromotionLevel;
        public byte GearPosition => _record.GearPosition;
        public uint ID => _record.ID;
        public uint PlayerCharacterID => _record.PlayerCharacterID;

        public Core.WebApi.Response.PlayerCharacterGear ToResponseData()
        {
            return new Core.WebApi.Response.PlayerCharacterGear
            {
                GearID = GearID,
                Level = PromotionLevel,
                Exp = Exp,
                GearPosition = GearPosition,
                ID = ID
            };
        }
    }
}
