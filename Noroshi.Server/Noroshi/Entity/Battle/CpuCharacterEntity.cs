using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuCharacterSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class CpuCharacterEntity : AbstractDaoWrapperEntity<CpuCharacterEntity, CpuCharacterDao, Schema.PrimaryKey, Schema.Record>, IBattleCharacter
    {
        public static IEnumerable<CpuCharacterEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey() { ID = id }));
        }


        uint? _initialHp = null;
        ushort? _initialEnergy = null;

        public void SetInitialHPAndEnergy(uint? initialHp, ushort? initialEnergy)
        {
            _initialHp = initialHp;
            _initialEnergy = initialEnergy;
        }


        public bool IsBoss { get; private set; }

        public void SetBoss()
        {
            IsBoss = true;
        }

        public uint ID => _record.ID;
        public uint CharacterID => _record.CharacterID;
        public ushort Level => _record.Level;
        public byte PromotionLevel => _record.PromotionLevel;
        public byte EvolutionLevel => _record.EvolutionLevel;
        public uint? FixedMaxHP => _record.FixedMaxHP > 0 ? (uint?)_record.FixedMaxHP : null;
        public ushort? InitialEnergy => _record.InitialEnergy > 0 ? (ushort?)_record.InitialEnergy : null;
        public ushort ActionLevel1 => Level;
        public ushort ActionLevel2 => Level;
        public ushort ActionLevel3 => Level;
        public ushort ActionLevel4 => Level;
        public ushort ActionLevel5 => Level;
        public uint GearID1 => 0;
        public uint GearID2 => 0;
        public uint GearID3 => 0;
        public uint GearID4 => 0;
        public uint GearID5 => 0;
        public uint GearID6 => 0;

        public Core.WebApi.Response.Battle.BattleCharacter ToResponseData()
        {
            return new Core.WebApi.Response.Battle.BattleCharacter()
            {
                Type = (byte)BattleCharacterType.Cpu,
                ID = ID,
                IsBoss = IsBoss,
                CharacterID = CharacterID,
                Level = Level,
                PromotionLevel = PromotionLevel,
                EvolutionLevel = EvolutionLevel,
                FixedMaxHP = FixedMaxHP,
                ActionLevel1 = ActionLevel1,
                ActionLevel2 = ActionLevel2,
                ActionLevel3 = ActionLevel3,
                ActionLevel4 = ActionLevel4,
                ActionLevel5 = ActionLevel5,
                GearID1 = GearID1,
                GearID2 = GearID2,
                GearID3 = GearID3,
                GearID4 = GearID4,
                GearID5 = GearID5,
                GearID6 = GearID6,
                InitialHP = _initialHp,
                InitialEnergy = _initialEnergy ?? InitialEnergy,
            };
        }
    }
}
