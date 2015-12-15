using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Character;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterSchema;

namespace Noroshi.Server.Entity.Character
{
    public class CharacterEntity : AbstractDaoWrapperEntity<CharacterEntity, CharacterDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CharacterEntity> ReadAndBuildAll()
        {
            return _loadAssociatedEntities(_instantiate((new CharacterDao()).ReadAll()));
        }
        public static CharacterEntity ReadAndBuild(uint id)
        {
            return ReadAndBuildMulti(new[] { id }).FirstOrDefault();
        }
        public static IEnumerable<CharacterEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id })));
        }

        static IEnumerable<CharacterEntity> _loadAssociatedEntities(IEnumerable<CharacterEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var characterIdToCharacterGears = CharacterGearEntity.ReadAndBuildByCharacterIDs(entities.Select(entity => entity.ID)).ToLookup(e => e.CharacterID);
            var characterIdToActionSequences = CharacterActionSequenceEntity.ReadAndBuildAll().ToLookup(e => e.CharacterID);
            return entities.Select(entity =>
            {
                entity.SetCharacterGears(characterIdToCharacterGears[entity.ID]);
                entity.SetActionSequences(characterIdToActionSequences[entity.ID]);
                return entity;
            });
        }


        Dictionary<byte, CharacterGearEntity> _promotionLevelToCharacterGear;
        IEnumerable<CharacterActionSequenceEntity> _actionSequences;

        public void SetCharacterGears(IEnumerable<CharacterGearEntity> characterGears)
        {
            _promotionLevelToCharacterGear = characterGears.ToDictionary(cg => cg.PromotionLevel);
        }
        public void SetActionSequences(IEnumerable<CharacterActionSequenceEntity> actionSequences)
        {
            _actionSequences = actionSequences;
        }

        public uint ID => _record.ID;
        public CharacterTagSet TagSet => new CharacterTagSet(_record.TagFlags);
        public string TextKey => "Master.Character." + _record.TextKey;
        public byte InitialEvolutionLevel => _record.InitialEvolutionLevel;
        public byte Position => _record.Position;
        public uint OrderPriority => _record.OrderPriority;
        public uint OrderInLayer => _record.OrderInLayer;
        public byte Type => _record.Type;
        public float Strength => _record.Strength;
        public float Intellect => _record.Intellect;
        public float Agility => _record.Agility;
        public float StrengthGrowth => _record.StrengthGrowth;
        public float IntellectGrowth => _record.IntellectGrowth;
        public float AgilityGrowth => _record.AgilityGrowth;
        public uint MagicCrit => _record.MagicCrit;
        public uint ArmorPenetration => _record.ArmorPenetration;
        public uint IgnoreMagicResistance => _record.IgnoreMagicResistance;
        public byte Accuracy => _record.Accuracy;
        public byte Dodge => _record.Dodge;
        public uint HPRegen => _record.HPRegen;
        public ushort EnergyRegen => _record.EnergyRegen;
        public byte ImproveHealings => _record.ImproveHealings;
        public byte ReduceEnergyCost => _record.ReduceEnergyCost;
        public uint LifeStealRating => _record.LifeStealRating;
        public uint ActionID0 => _record.ActionID0;
        public uint ActionID1 => _record.ActionID1;
        public uint ActionID2 => _record.ActionID2;
        public uint ActionID3 => _record.ActionID3;
        public uint ActionID4 => _record.ActionID4;
        public uint ActionID5 => _record.ActionID5;
        public ushort EvolutionType => _record.EvolutionType;

        public uint[] GetGearIDs(byte promotionLevel)
        {
            return _promotionLevelToCharacterGear[promotionLevel].GearIDs;
        }

        public Core.WebApi.Response.Character.Character ToResponseData()
        {
            return new Core.WebApi.Response.Character.Character
            {
                ID = ID,
                TagFlags = _record.TagFlags,
                TextKey = TextKey,
                InitialEvolutionLevel = InitialEvolutionLevel,
                Position = Position,
                OrderPriority = OrderPriority,
                OrderInLayer = OrderInLayer,
                Type = Type,
                Strength = Strength,
                Intellect = Intellect,
                Agility = Agility,
                StrengthGrowth = StrengthGrowth,
                IntellectGrowth = IntellectGrowth,
                AgilityGrowth = AgilityGrowth,
                MagicCrit = MagicCrit,
                ArmorPenetration = ArmorPenetration,
                IgnoreMagicResistance = IgnoreMagicResistance,
                Accuracy = Accuracy,
                Dodge = Dodge,
                HPRegen = HPRegen,
                EnergyRegen = EnergyRegen,
                ImproveHealings = ImproveHealings,
                ReduceEnergyCost = ReduceEnergyCost,
                LifeStealRating = LifeStealRating,
                ActionID0 = ActionID0,
                ActionID1 = ActionID1,
                ActionID2 = ActionID2,
                ActionID3 = ActionID3,
                ActionID4 = ActionID4,
                ActionID5 = ActionID5,
                EvolutionType = EvolutionType,
                ActionSequences = _actionSequences.Select(e => e.ToResponseData()).ToArray(),
                GearIDs = _promotionLevelToCharacterGear.OrderBy(kv => kv.Key).Select(kv => kv.Value.GearIDs).ToArray(),
            };
        }
    }
}
