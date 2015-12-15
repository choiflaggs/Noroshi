namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class StoryChapterSchema
	{
		public static string TableName => "story_chapter";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt16 No { get; set; }
			public System.String TextKey { get; set; }
			public System.UInt16 PlayerLevel { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
