namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class RankingReferenceSchema
	{
		public static string TableName => "ranking_reference";

		public class Record : AbstractRecord
		{
			public System.UInt32 RankingID { get; set; }
			public System.Byte ReferenceID { get; set; }
			public System.UInt32 UpdatedAt { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 RankingID { get; set; }
		}
	}
}
