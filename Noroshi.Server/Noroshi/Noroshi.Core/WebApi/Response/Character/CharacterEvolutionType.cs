namespace Noroshi.Core.WebApi.Response.Character
{
    public class CharacterEvolutionType
    {
        public ushort Type
        { get; set; }
        public ushort EvolutionLevel
        { get; set; }
        public ushort Soul
        { get; set; }
        public uint NecessaryGold
        { get; set; }
    }
}