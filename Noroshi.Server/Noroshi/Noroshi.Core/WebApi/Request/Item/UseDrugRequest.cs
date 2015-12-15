namespace Noroshi.Core.WebApi.Request.Item
{
    class UseDrugRequest
    {
        public uint DrugID
        { get; set; }
        public uint CharacterID
        { get; set; }
        public ushort UsePossessionsCount
        { get; set; }
    }
}
