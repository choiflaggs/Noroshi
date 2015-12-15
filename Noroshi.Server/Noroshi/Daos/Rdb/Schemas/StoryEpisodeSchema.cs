namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class StoryEpisodeSchema
	{
		public static string TableName => "story_episode";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt16 No { get; set; }
			public System.UInt32 ChapterID { get; set; }
			public System.String TextKey { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
