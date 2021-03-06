﻿namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class PresentBoxReceivedLogSchema
	{
		public static string TableName => "present_box_received_log";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 PlayerID { get; set; }
			public System.String PossessionData { get; set; }
			public System.String TextID { get; set; }
			public System.String TextData { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
			public System.UInt32 CreatedAt { get; set; }
		}
	}
}
