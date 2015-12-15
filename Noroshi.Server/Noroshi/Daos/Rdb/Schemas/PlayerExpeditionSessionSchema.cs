namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PlayerExpeditionSessionSchema
	{
		public static string TableName => "player_expedition_session";

		public class Record : AbstractRecord
		{
			public System.UInt32 PlayerID { get; set; }
			public System.UInt32 ExpeditionID { get; set; }
			public System.Byte ClearStep { get; set; }
			public System.Byte State { get; set; }
			public System.String StageData { get; set; }
			public System.String PlayerCharacterData { get; set; }
			public System.UInt32 StartedAt { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 PlayerID { get; set; }
		}
	}
}
