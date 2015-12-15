namespace Noroshi.Core.WebApi.Response
{
    public class CharacterEffect
    {
        public uint ID { get; set; }
        public uint PrefabID { get; set; }
        public string AnimationName { get; set; }
        public byte MultiAnimation { get; set; }
        public bool HasText { get; set; }
        public short OrderInCharacterLayer { get; set; }
        public byte Position { get; set; }
        public bool FixedRotationY { get; set; }
        public bool IsMultiAnimation { get { return MultiAnimation > 0; } }
    }
}