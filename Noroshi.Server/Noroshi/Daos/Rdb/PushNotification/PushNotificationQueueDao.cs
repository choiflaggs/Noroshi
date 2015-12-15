using System.Collections.Generic;
using Noroshi.Core.Game.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PushNotificationQueueSchema;

namespace Noroshi.Server.Daos.Rdb.PushNotification
{
    public class PushNotificationQueueDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// ステータス。
        /// </summary>
        enum Status
        {
            /// <summary>
            /// アクティブ状態。プッシュ通知の処理対象。
            /// </summary>
            Active = 1,
        }

        protected override string _tableName => Schema.TableName;

        /// <summary>
        /// 古い順にアクティブ状態のレコードを指定数だけ参照する。
        /// </summary>
        /// <param name="rowCount">参照レコード数</param>
        /// <returns></returns>
        public IEnumerable<Schema.Record> ReadOlderActive(ushort rowCount)
        {
            return _select("Status = @Status ORDER BY CreatedAt ASC LIMIT @RowCount", new
            {
                Status = (byte)Status.Active,
                RowCount = rowCount,
            });
        }

        /// <summary>
        /// レコードを作成する。
        /// </summary>
        /// <param name="deviceType">プッシュ通知先デバイス種別</param>
        /// <param name="deviceToken">プッシュ通知先デバイストークン</param>
        /// <param name="message">通知内容</param>
        /// <returns></returns>
        public Schema.Record Create(DeviceTokenType deviceType, string deviceToken, string message)
        {
            var record = new Schema.Record
            {
                ID = (new SequentialIDTable(_tableName)).GenerateID(),
                Status = (byte)Status.Active,
                DeviceType = (byte)deviceType,
                DeviceToken = deviceToken,
                Message = message,
            };
            return Create(record);
        }
    }
}