namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class GuildSchema
	{
		public static string TableName => "guild";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.Byte Category { get; set; }
			public System.UInt32 CountryID { get; set; }
			public System.UInt16 NecessaryPlayerLevel { get; set; }
			public System.String Name { get; set; }
			public System.String Introduction { get; set; }
			public System.UInt32 LeaderPlayerID { get; set; }
			public System.UInt32 MemberNum { get; set; }
			public System.Byte RequestNum { get; set; }
			public System.Byte MemberAndRequestNumCluster { get; set; }
			public System.UInt16 AveragePlayerLevel { get; set; }
			public System.UInt16 CooperationPoint { get; set; }
			public System.UInt32 CooperationPointUpdatedAt { get; set; }
			public System.UInt16 CooperationPointConsumedNum { get; set; }
			public System.UInt32 CreatedAt { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
