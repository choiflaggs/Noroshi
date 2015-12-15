namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerStoryEpisodeSchema
	{
		public static string TableName => "player_story_episode";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 EpisodeID { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
