namespace Noroshi.Server.Daos.Rdb.Schemas
{
	public class SoundSchema
	{
		public static string TableName => "sound";

		public class Record : AbstractRecord
		{
			public System.UInt32 ID { get; set; }
			public System.String Path { get; set; }
			public System.Byte ChannelNum { get; set; }
			public System.Byte PlayType { get; set; }
		}
		public class PrimaryKey : IPrimaryKey
		{
			public System.UInt32 ID { get; set; }
		}
	}
}
