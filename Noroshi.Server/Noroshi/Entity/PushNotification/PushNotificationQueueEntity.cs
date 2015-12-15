using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.PushNotification;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PushNotificationQueueSchema;

namespace Noroshi.Server.Entity.PushNotification
{
    public class PushNotificationQueueEntity : AbstractDaoWrapperEntity<PushNotificationQueueEntity, PushNotificationQueueDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 1クエリで取得する最大レコード数。
        /// </summary>
        const ushort MAX_ROW_COUNT_PER_QUERY = 1000;

        /// <summary>
        /// 単体ビルド。該当レコードが存在しない場合は null を返す。
        /// </summary>
        /// <param name="id">キュー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public PushNotificationQueueEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { id }, readType).FirstOrDefault();
        }
        /// <summary>
        /// 複数ビルド。
        /// </summary>
        /// <param name="ids">キュー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public IEnumerable<PushNotificationQueueEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }), readType);
        }

        /// <summary>
        /// 古い順にアクティブ状態のレコードを指定数だけ参照してビルドする。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PushNotificationQueueEntity> ReadAndBuildOlderActive()
        {
            return _instantiate((new PushNotificationQueueDao()).ReadOlderActive(MAX_ROW_COUNT_PER_QUERY));
        }

        /// <summary>
        /// キューを作成する。
        /// </summary>
        /// <param name="playerDeviceToken">対象プレイヤーデバイストークン情報</param>
        /// <param name="message">通知内容</param>
        /// <returns></returns>
        public PushNotificationQueueEntity CreateAndBuild(PlayerDeviceTokenEntity playerDeviceToken, string message)
        {
            return CreateAndBuildMulti(new[] { playerDeviceToken }, message).FirstOrDefault();
        }
        /// <summary>
        /// キューを一括で複数作成する。
        /// </summary>
        /// <param name="playerDeviceTokens">対象プレイヤーデバイストークン情報</param>
        /// <param name="message">通知内容</param>
        /// <returns></returns>
        public IEnumerable<PushNotificationQueueEntity> CreateAndBuildMulti(IEnumerable<PlayerDeviceTokenEntity> playerDeviceTokens, string message)
        {
            // 必要であれば SQL レベルでの最適化をする。
            return playerDeviceTokens.Select(playerDeviceToken => _instantiate((new PushNotificationQueueDao()).Create(playerDeviceToken.Type, playerDeviceToken.Token, message)));
        }


        /// <summary>
        /// キュー ID。作成時にユニークに採番される。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 送信先デバイス種別。
        /// </summary>
        public DeviceTokenType DeviceType => (DeviceTokenType)_record.DeviceType;
        /// <summary>
        /// 送信先デバイストークン。
        /// </summary>
        public string DeviceToken => _record.DeviceToken;
        /// <summary>
        /// 通知内容。
        /// </summary>
        public string Message => _record.Message;
    }
}