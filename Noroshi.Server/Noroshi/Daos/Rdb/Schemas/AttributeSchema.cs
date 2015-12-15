namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class AttributeSchema
	{
		public static string TableName => "attribute";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 ClassID { get; set; }
			public System.UInt32 GroupID { get; set; }
			public System.UInt32 Lifetime { get; set; }
			public System.UInt32 EffectID { get; set; }
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
			public System.Int32 Arg11 { get; set; }
			public System.Int32 Arg12 { get; set; }
			public System.Int32 Arg13 { get; set; }
			public System.Int32 Arg14 { get; set; }
			public System.Int32 Arg15 { get; set; }
			public System.UInt32 ReceiveDamageCharacterEffectID { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
