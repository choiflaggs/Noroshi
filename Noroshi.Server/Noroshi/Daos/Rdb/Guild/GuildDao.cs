using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildSchema;

namespace Noroshi.Server.Daos.Rdb.Guild
{
    public class GuildDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;


        public IEnumerable<Schema.Record> ReadByCategoryAndMaxAveragePlayerLevel(GuildCategory category, ushort maxAveragePlayerLevel, byte rowCount)
        {
            return _select("Category = @Category AND AveragePlayerLevel <= @MaxAveragePlayerLevel ORDER BY AveragePlayerLevel DESC Limit @RowCount", new
            {
                Category = (byte)category,
                MaxAveragePlayerLevel = maxAveragePlayerLevel,
                RowCount = rowCount,
            });
        }
        public IEnumerable<Schema.Record> ReadByCategoryAndMemberAndRequestNumClusterAndMaxAveragePlayerLevel(GuildCategory category, byte memberAndRequestNumCluster, ushort maxAveragePlayerLevel, byte rowCount)
        {
            return _select("Category = @Category AND MemberAndRequestNumCluster = @MemberAndRequestNumCluster AND AveragePlayerLevel <= @MaxAveragePlayerLevel ORDER BY AveragePlayerLevel DESC Limit @RowCount", new
            {
                Category = (byte)category,
                MemberAndRequestNumCluster = memberAndRequestNumCluster,
                MaxAveragePlayerLevel = maxAveragePlayerLevel,
                RowCount = rowCount,
            });
        }

        public IEnumerable<Schema.Record> ReadAllNormalGuilds()
        {
            // バッチから呼ばれるので速度は気にしない。
            return _select("Category IN @Categories", new { Categories = new[] { GuildCategory.NormalOpen, GuildCategory.NormalClose } });
        }

        public Schema.Record CreateNormalGuild(uint playerId, bool isOpenGuild)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                Category = (byte)(isOpenGuild ? GuildCategory.NormalOpen : GuildCategory.NormalClose),
                LeaderPlayerID = playerId,
                Name = "",
                Introduction = "",
            };
            return Create(record);
        }
        public Schema.Record ReadBeginnerGuild(ReadType readType = ReadType.Slave)
        {
            return _select("Category = @Category", new { Category = GuildCategory.Beginner }, readType).FirstOrDefault();
        }
        /// <summary>
        /// ロックをかけて参照し、レコードがなければ作成する。
        /// 各シャードで最初の一回だけが空振るだけなのでギャップロックを許容し、
        /// ビギナーギルドが一つだけしか作られないことを優先する。
        /// </summary>
        /// <returns></returns>
        public Schema.Record ReadOrCreateBeginnerGuild()
        {
            var record = ReadBeginnerGuild(ReadType.Lock);
            if (record != null) return record;

            record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                Category = (byte)GuildCategory.Beginner,
                LeaderPlayerID = 0,
                Name = "",
                Introduction = "",
            };
            return Create(record);
        }
    }
}
