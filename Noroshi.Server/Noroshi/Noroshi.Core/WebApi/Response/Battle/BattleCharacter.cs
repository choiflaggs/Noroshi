using Noroshi.Core.Game.Character;

namespace Noroshi.Core.WebApi.Response.Battle
{
    public class BattleCharacter : IPersonalCharacter
    {
        public byte Type { get; set; }
        public uint ID { get; set; }
        public bool IsBoss { get; set; }

        public uint CharacterID { get; set; }
        public ushort Level { get; set; }
        public byte PromotionLevel { get; set; }
        public byte EvolutionLevel { get; set; }
        public uint? FixedMaxHP { get; set; }
        public uint? InitialHP { get; set; }
        public ushort? InitialEnergy { get; set; }
        public float? DamageCoefficient { get; set; }
        public ushort ActionLevel1 { get; set; }
        public ushort ActionLevel2 { get; set; }
        public ushort ActionLevel3 { get; set; }
        public ushort ActionLevel4 { get; set; }
        public ushort ActionLevel5 { get; set; }
        public uint GearID1 { get; set; }
        public uint GearID2 { get; set; }
        public uint GearID3 { get; set; }
        public uint GearID4 { get; set; }
        public uint GearID5 { get; set; }
        public uint GearID6 { get; set; }
    }
}
