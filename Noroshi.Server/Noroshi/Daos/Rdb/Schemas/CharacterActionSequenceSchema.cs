namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class CharacterActionSequenceSchema
	{
		public static string TableName => "character_action_sequence";

		public class Record : AbstractRecord
		{
			public System.UInt32 CharacterID { get; set; }
			public System.Byte TargetActionNum { get; set; }
			public System.Byte SecondLoopStartPosition { get; set; }
			public System.SByte ActionSequence1 { get; set; }
			public System.SByte ActionSequence2 { get; set; }
			public System.SByte ActionSequence3 { get; set; }
			public System.SByte ActionSequence4 { get; set; }
			public System.SByte ActionSequence5 { get; set; }
			public System.SByte ActionSequence6 { get; set; }
			public System.SByte ActionSequence7 { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 CharacterID { get; set; }
			public System.Byte TargetActionNum { get; set; }
		}
	}
}
