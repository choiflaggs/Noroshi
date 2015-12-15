using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Character;
using Noroshi.Core.WebApi.Response;

namespace Noroshi.Core.Game.Character
{
    public enum CharacterType
    {
        /// 力タイプ
        Strength  = 1,
        /// 知力タイプ
        Intellect = 2,
        /// 素早さタイプ
        Agility   = 3
    }
    public enum CharacterPosition
    {
        /// 前衛
        Front   = 1,
        /// 中衛
        Central = 2,
        /// 後衛
        Back    = 3
    }

    /// キャラクターのステータスを扱うクラス。
    /// 内部に保有しているサーバからのデータをもとにステータス取得メソッドを提供する。
    ///　バトル依存ではないのでキャラクターステータスクラスの名前空間は後で変える。
    public class CharacterStatus
    {
        const float STRENGTH_MAX_HP_COEFFICIENT = 18.0f;
        const float STRENGTH_ARMOR_COEFFICIENT = 0.14f;
        const float AGILITY_PHYSICAL_ATTACK_COEFFICIENT = 1.4f;
        const float AGILITY_ARMOR_COEFFICIENT = 0.07f;
        const float INTELLECT_MAGIC_POWER_COEFFICIENT = 2.4f;
        const float INTELLECT_MAGIC_RESISTANCE_COEFFICIENT = 0.1f;
        const float PHYSICAL_CRIT_COEFFICIENT = 0.4f;

        const float PROMOTION_LEVEL_BASE_PARAM_COEFFICIENT = 2;

        const float EVOLUTION_1_COEFFICIENT = 1f;
        const float EVOLUTION_2_COEFFICIENT = 1.5f;
        const float EVOLUTION_3_COEFFICIENT = 4f / 3f;
        const float EVOLUTION_4_COEFFICIENT = 1.25f;
        const float EVOLUTION_5_COEFFICIENT = 1.2f;

        const float PROMOTION_1_COEFFICIENT = 1.0f;
        const float PROMOTION_2_COEFFICIENT = 1.0f;
        const float PROMOTION_3_COEFFICIENT = 1.0f;
        const float PROMOTION_4_COEFFICIENT = 1.0f;
        const float PROMOTION_5_COEFFICIENT = 1.0f;
        const float PROMOTION_6_COEFFICIENT = 1.5f;
        const float PROMOTION_7_COEFFICIENT = 1.5f;
        const float PROMOTION_8_COEFFICIENT = 1.5f;
        const float PROMOTION_9_COEFFICIENT = 1.8f;
        const float PROMOTION_10_COEFFICIENT = 1.8f;
        const float PROMOTION_11_COEFFICIENT = 1.8f;
        const float PROMOTION_12_COEFFICIENT = 2.0f;

        const float DEFAULT_ACTION_FREQUENCY = 1f;

        protected const byte MAX_GEAR_SLOT_NUM = 6;

        protected IPersonalCharacter _personalData;
        protected Core.WebApi.Response.Character.Character _masterData;

        ushort? _temporaryLevel = null;
        ushort? _temporaryActionLevel1 = null;
        ushort? _temporaryActionLevel2 = null;
        ushort? _temporaryActionLevel3 = null;
        ushort? _temporaryActionLevel4 = null;
        ushort? _temporaryActionLevel5 = null;
        byte? _temporaryPromotionLevel = null;

        protected GearContainer _gearContainer = new GearContainer();

        public CharacterStatus(IPersonalCharacter personalData, Core.WebApi.Response.Character.Character masterData)
        {
            _personalData = personalData;
            _masterData   = masterData;
        }

        /// キャラクター ID。
        public uint CharacterID { get { return _masterData.ID; } }
        /// タグ。
        public CharacterTagSet TagSet { get { return new CharacterTagSet(_masterData.TagFlags); } }
        /// 能力タイプ（力 / 知力 / 素早さ）
        public CharacterType Type { get { return (CharacterType)_masterData.Type; } }
        /// 隊列（前衛 / 中衛 / 後衛）
        public CharacterPosition Position { get { return (CharacterPosition)_masterData.Position; } }
        public uint OrderPriority { get { return _masterData.OrderPriority; } }
        public uint OrderInLayer { get { return _masterData.OrderInLayer; } }

        /// レベル。
        public ushort Level
        {
            get
            {
                return _temporaryLevel.HasValue ? _temporaryLevel.Value : _personalData.Level;
            }
        }
        /// 進化レベル（キャラクターによって開始レベルは変わる）
        public byte EvolutionLevel { get { return _personalData.EvolutionLevel; } }
        /// 昇格レベル（1からスタート）
        public byte PromotionLevel
        {
            get
            {
                return _temporaryPromotionLevel.HasValue ? _temporaryPromotionLevel.Value : _personalData.PromotionLevel;
            }
        }
        /// 力。
        public virtual float Strength { get { return (_initialStrength  + StrengthGrowth  * (Level - 1) + _getPromotionLevelBaseParam()) + _gearContainer.Strength; } }
        /// 知力。
        public virtual float Intellect { get { return (_initialIntellect + IntellectGrowth * (Level - 1) + _getPromotionLevelBaseParam()) + _gearContainer.Intellect; } }
        /// 素早さ。
        public virtual float Agility { get { return (_initialAgility   + AgilityGrowth   * (Level - 1) + _getPromotionLevelBaseParam()) + _gearContainer.Agility; } }
        float _getPromotionLevelBaseParam()
        {
            return (PromotionLevel - 1) * PROMOTION_LEVEL_BASE_PARAM_COEFFICIENT;
        }
        /// 力の初期値。
        float _initialStrength { get { return _masterData.Strength; } }
        /// 知力の初期値。
        float _initialIntellect { get { return _masterData.Intellect; } }
        /// 素早さの初期値。
        float _initialAgility { get { return _masterData.Agility; } }
        /// 力の成長力。
        public float StrengthGrowth { get { return _masterData.StrengthGrowth * _getEvolutionCoefficient(EvolutionLevel) + _gearContainer.StrengthGrowth; } }
        /// 知力の成長力。
        public float IntellectGrowth { get { return _masterData.IntellectGrowth * _getEvolutionCoefficient(EvolutionLevel) + _gearContainer.IntellectGrowth; } }
        /// 素早さの成長力。
        public float AgilityGrowth { get { return _masterData.AgilityGrowth * _getEvolutionCoefficient(EvolutionLevel) + _gearContainer.AgilityGrowth; } }
        float _getEvolutionCoefficient(int evolutionLevel)
        {
            var coefficients = new []
            {
                EVOLUTION_1_COEFFICIENT,
                EVOLUTION_2_COEFFICIENT,
                EVOLUTION_3_COEFFICIENT,
                EVOLUTION_4_COEFFICIENT,
                EVOLUTION_5_COEFFICIENT,
            };
            var coefficient = 1f;
            for (var i = 0; i < evolutionLevel; i++)
            {
                coefficient *= coefficients[i];
            }
            return coefficient;
        }
        /// 最大 HP。
        public virtual uint MaxHP
        {
            get
            {
                return (uint)(Strength * STRENGTH_MAX_HP_COEFFICIENT + _gearContainer.HP);
            }
        }
        /// 物理攻撃力。
        public virtual uint PhysicalAttack
        {
            get
            {
                var baseValueByType = new Dictionary<CharacterType, float>(){
                    {CharacterType.Strength , Strength},
                    {CharacterType.Agility  , Agility},
                    {CharacterType.Intellect, Intellect},
                };
                return (uint)(baseValueByType[Type] + Agility * AGILITY_PHYSICAL_ATTACK_COEFFICIENT + _gearContainer.PhysicalAttack);
            }
        }
        /// 魔法攻撃力。
        public virtual uint MagicPower
        {
            get
            {
                return (uint)(Intellect * INTELLECT_MAGIC_POWER_COEFFICIENT + _gearContainer.MagicPower);
            }
        }
        /// 物理防御力。
        public virtual uint Armor
        {
            get
            {
                return (uint)(Strength * STRENGTH_ARMOR_COEFFICIENT + Agility * AGILITY_ARMOR_COEFFICIENT + _gearContainer.Armor);
            }
        }
        /// 魔法防御力。
        public virtual uint MagicRegistance
        {
            get
            {
                return (uint)(Intellect * INTELLECT_MAGIC_RESISTANCE_COEFFICIENT + _gearContainer.MagicResistance);
            }
        }
        /// 物理クリティカル。
        public virtual uint PhysicalCrit
        {
            get
            {
                return (uint)(Agility * PHYSICAL_CRIT_COEFFICIENT * _getPromotionCoefficient(PromotionLevel) + _gearContainer.PhysicalCrit);
            }
        }
        float _getPromotionCoefficient(int promotionLevel)
        {
            var coefficients = new float[]{
                PROMOTION_1_COEFFICIENT,
                PROMOTION_2_COEFFICIENT,
                PROMOTION_3_COEFFICIENT,
                PROMOTION_4_COEFFICIENT,
                PROMOTION_5_COEFFICIENT,
                PROMOTION_6_COEFFICIENT,
                PROMOTION_7_COEFFICIENT,
                PROMOTION_8_COEFFICIENT,
                PROMOTION_9_COEFFICIENT,
                PROMOTION_10_COEFFICIENT,
                PROMOTION_11_COEFFICIENT,
                PROMOTION_12_COEFFICIENT,
            };
            return coefficients[promotionLevel - 1];
        }
        /// 魔法クリティカル。
        public uint MagicCrit { get { return (uint)(_masterData.MagicCrit + _gearContainer.MagicCrit); } }
        /// 防御貫通。
        public uint ArmorPenetration { get { return _masterData.ArmorPenetration; } }
        /// 魔法耐性無視。
        public uint IgnoreMagicResistance { get { return (uint)(_masterData.IgnoreMagicResistance + _gearContainer.IgnoreMagicResistance); } }
        /// 命中。
        public virtual byte Accuracy { get { return (byte)(_masterData.Accuracy + _gearContainer.Accuracy); } }
        /// 回避。
        public virtual byte Dodge { get { return (byte)(_masterData.Dodge + _gearContainer.Dodge); } }
        /// HP自動回復。
        public uint HPRegen { get { return (uint)(_masterData.HPRegen + _gearContainer.HPRegen); } }
        /// エネルギー自動回復。
        public ushort EnergyRegen { get { return (ushort)(_masterData.EnergyRegen + _gearContainer.EnergyRegen); } }
        /// 回復上昇。
        public byte ImproveHealings { get { return (byte)(_masterData.ImproveHealings + _gearContainer.ImproveHealings); } }
        /// エネルギー消費軽減。
        public byte ReduceEnergyCost { get { return _masterData.ReduceEnergyCost; } }
        /// ライフ奪取。
        public virtual uint LifeStealRating { get { return (uint)(_masterData.LifeStealRating + _gearContainer.LifeStealRating); } }

        public uint[] GearIDs
        {
            get
            {
                return Enumerable.Range(1, MAX_GEAR_SLOT_NUM).Select(no => _gearContainer.Get((byte)no) != null ? _gearContainer.Get((byte)no).ID : 0).ToArray();
            }
        }

        public virtual float ActionFrequency { get { return DEFAULT_ACTION_FREQUENCY; } }

        public uint[] ActionIDs
        {
            get
            {
                var actionIds = new uint[]{
                    _masterData.ActionID0,
                    _masterData.ActionID1,
                    _masterData.ActionID2,
                    _masterData.ActionID3,
                    _masterData.ActionID4,
                    _masterData.ActionID5,
                };
                return actionIds;
            }
        }
        /// 利用可能なアクション ID 配列を取得
        public uint[] AvailableActionIDs { get { return AvailableActionIDsWithZero.Where(id => id != 0).ToArray(); } }

        public uint[] AvailableActionIDsWithZero
        {
            get
            {
                // TODO : 設定化
                var num = 
                  PromotionLevel < Constant.PROMOTION_RANK_MAP[2] ? 2
                : PromotionLevel < Constant.PROMOTION_RANK_MAP[3] ? 3
                : PromotionLevel < Constant.PROMOTION_RANK_MAP[4] ? 4
                : PromotionLevel < Constant.PROMOTION_RANK_MAP[5] ? 5
                : 6;
                if (ActionIDs.Length < num) num = ActionIDs.Length;
                return Enumerable.Range(0, num).Select(i => ActionIDs[i]).ToArray();
            }
        }

        public ushort[] ActionLevels
        {
            get { return new ushort[] { 1, _actionLevel1, _actionLevel2, _actionLevel3, _actionLevel4, _actionLevel5 }; }
        }
        ushort _actionLevel1 { get { return _temporaryActionLevel1.HasValue ? _temporaryActionLevel1.Value : _personalData.ActionLevel1; } }
        ushort _actionLevel2 { get { return _temporaryActionLevel2.HasValue ? _temporaryActionLevel2.Value : _personalData.ActionLevel2; } }
        ushort _actionLevel3 { get { return _temporaryActionLevel3.HasValue ? _temporaryActionLevel3.Value : _personalData.ActionLevel3; } }
        ushort _actionLevel4 { get { return _temporaryActionLevel4.HasValue ? _temporaryActionLevel4.Value : _personalData.ActionLevel4; } }
        ushort _actionLevel5 { get { return _temporaryActionLevel5.HasValue ? _temporaryActionLevel5.Value : _personalData.ActionLevel5; } }

        public void EquipGear(byte slot, Gear gear)
        {
            _gearContainer.Add(slot, gear);
        }

        // 子クラスには強制的にレベルをセットできるメソッドを提供しておく。
        protected void _overrideLevel(ushort level)
        {
            _temporaryLevel = level;
        }
        protected void _overrideActionLevel2(ushort level) { _temporaryActionLevel2 = level; }
        protected void _overrideActionLevel3(ushort level) { _temporaryActionLevel3 = level; }

        public void LevelUp()
        {
            _temporaryLevel = (ushort)(Level + 1);
        }
        public void PromotionLevelUp()
        {
            if (GearIDs.Any(gearId => gearId == 0)) return;
            _temporaryPromotionLevel = (byte)(PromotionLevel + 1);
            _gearContainer.Clear();
        }

        public byte SkinLevel
        {
            get
            {
                if (TagSet.IsDeca)
                {
                    return (byte)EvolutionLevel;
                }
                else
                {
                    return (byte)(EvolutionLevel < 3 ? 1 : EvolutionLevel < 5 ? 2 : 3);
                }
            }
        }
    }
}
