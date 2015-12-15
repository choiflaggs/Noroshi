namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class StoryStageSchema
	{
		public static string TableName => "story_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt16 No { get; set; }
			public System.UInt32 EpisodeID { get; set; }
			public System.Byte Type { get; set; }
			public System.UInt32 BattleID { get; set; }
			public System.Byte IsFixedParty { get; set; }
			public System.UInt32 FixedCharacterID1 { get; set; }
			public System.UInt32 FixedCharacterID2 { get; set; }
			public System.UInt32 FixedCharacterID3 { get; set; }
			public System.UInt32 FixedCharacterID4 { get; set; }
			public System.UInt32 FixedCharacterID5 { get; set; }
			public System.UInt32 CpuCharacterID1 { get; set; }
			public System.UInt32 CpuCharacterID2 { get; set; }
			public System.UInt32 CpuCharacterID3 { get; set; }
			public System.UInt32 CpuCharacterID4 { get; set; }
			public System.UInt32 CpuCharacterID5 { get; set; }
			public System.String TextKey { get; set; }
			public System.UInt16 Stamina { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
