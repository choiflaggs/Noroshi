using System;

namespace Noroshi.Server.Daos.Rdb.Schemas
{
    public class GuildRankingSchema
    {
        public static string TableName
        {
            get { throw new InvalidOperationException(); }
        }
        public class Record : AbstractRecord
        {
            public System.UInt32 GuildID { get; set; }
            public System.UInt32 UniqueRank { get; set; }
            public System.UInt32 Rank { get; set; }
            public System.Int32 Value { get; set; }
        }
        public class PrimaryKey : IPrimaryKey
        {
            public System.UInt32 GuildID { get; set; }
        }
    }
}
