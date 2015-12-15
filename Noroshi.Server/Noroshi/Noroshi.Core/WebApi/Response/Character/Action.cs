namespace Noroshi.Core.WebApi.Response.Character
{
    public class Action
    {
        public uint ID { get; set; }
        public uint ClassID { get; set; }
        public byte TriggerID { get; set; }
        public float? ExecutableProbability { get; set; }
        public byte? ExecutableNum { get; set; }
        public byte? ExecutorTargetable { get; set; }
        public string TextKey { get; set; }
        public byte? TargetSortType { get; set; }
        public byte? MaxTargetNum { get; set; }
        public byte DamageType { get; set; }
        public byte DamageMagicalAttribute { get; set; }
        public byte TargetStateID { get; set; }
        public uint AttributeID1 { get; set; }
        public float AttributeIntercept1 { get; set; }
        public float AttributeSlope1 { get; set; }
        public uint AttributeID2 { get; set; }
        public float AttributeIntercept2 { get; set; }
        public float AttributeSlope2 { get; set; }
        public int Arg1 { get; set; }
        public int Arg2 { get; set; }
        public int Arg3 { get; set; }
        public int Arg4 { get; set; }
        public int Arg5 { get; set; }
        public int Arg6 { get; set; }
        public int Arg7 { get; set; }
        public int Arg8 { get; set; }
        public int Arg9 { get; set; }
        public int Arg10 { get; set; }
        public float Intercept1 { get; set; }
        public float Slope1 { get; set; }
        public float Intercept2 { get; set; }
        public float Slope2 { get; set; }
        public float Intercept3 { get; set; }
        public float Slope3 { get; set; }
        public uint HitCharacterEffectID { get; set; }
        public uint? SoundID { get; set; }
        public uint? ExecutionSoundID { get; set; }
        public uint? HitSoundID { get; set; }
    }
}
