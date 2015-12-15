namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class ActionSchema
	{
		public static string TableName => "action";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String TextKey { get; set; }
			public System.UInt32 ClassID { get; set; }
			public System.Byte TriggerID { get; set; }
			public System.Single ExecutableProbability { get; set; }
			public System.Byte ExecutableNum { get; set; }
			public System.Byte ExecutorTargetable { get; set; }
			public System.Byte TargetSortType { get; set; }
			public System.Byte MaxTargetNum { get; set; }
			public System.Byte DamageType { get; set; }
			public System.Byte DamageMagicalAttribute { get; set; }
			public System.Byte TargetStateID { get; set; }
			public System.UInt32 AttributeID1 { get; set; }
			public System.Single AttributeIntercept1 { get; set; }
			public System.Single AttributeSlope1 { get; set; }
			public System.UInt32 AttributeID2 { get; set; }
			public System.Single AttributeIntercept2 { get; set; }
			public System.Single AttributeSlope2 { get; set; }
			public System.Int32 Arg1 { get; set; }
			public System.Int32 Arg2 { get; set; }
			public System.Int32 Arg3 { get; set; }
			public System.Int32 Arg4 { get; set; }
			public System.Int32 Arg5 { get; set; }
			public System.Int32 Arg6 { get; set; }
			public System.Int32 Arg7 { get; set; }
			public System.Int32 Arg8 { get; set; }
			public System.Int32 Arg9 { get; set; }
			public System.Int32 Arg10 { get; set; }
			public System.Single Intercept1 { get; set; }
			public System.Single Slope1 { get; set; }
			public System.Single Intercept2 { get; set; }
			public System.Single Slope2 { get; set; }
			public System.Single Intercept3 { get; set; }
			public System.Single Slope3 { get; set; }
			public System.UInt32 HitCharacterEffectID { get; set; }
			public System.UInt32 SoundID { get; set; }
			public System.UInt32 ExecutionSoundID { get; set; }
			public System.UInt32 HitSoundID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
