namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerStoryStageSchema
	{
		public static string TableName => "player_story_stage";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 StageID { get; set; }
			public System.Byte Rank { get; set; }
			public System.UInt32 PlayCount { get; set; }
			public System.UInt32 MaxPlayCount { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 StageID { get; set; }
		}
	}
}
