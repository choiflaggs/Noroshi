using System.Collections.Generic;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerDeviceTokenSchema;
using System.Linq;

namespace Noroshi.Server.Entity.Player
{
    /// <summary>
    /// プレイヤーのデバイストークンを管理するクラス。
    /// </summary>
    public class PlayerDeviceTokenEntity : AbstractDaoWrapperEntity<PlayerDeviceTokenEntity, PlayerDeviceTokenDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 単体ビルド。該当レコードが存在しない場合は null を返す。
        /// 参照時はこちらのメソッドを利用すること。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="type">対象デバイス種別</param>
        /// <returns></returns>
        public static PlayerDeviceTokenEntity ReadAndBuild(uint playerId, DeviceTokenType type)
        {
            return ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId, Type = (byte)type });
        }
        /// <summary>
        /// 複数のプレイヤー ID でビルド。
        /// </summary>
        /// <param name="playerIds">対象プレイヤー ID</param>
        /// <returns></returns>
        public static IEnumerable<PlayerDeviceTokenEntity> ReadAndBuildByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _instantiate((new PlayerDeviceTokenDao()).ReadByPlayerIDs(playerIds));
        }

        /// <summary>
        /// レコード作成を試行し、失敗（既にレコードが存在）したらロックを掛けて参照したレコードでビルド。
        /// 更新時はこちらのメソッドを利用すること。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="type">対象デバイス種別</param>
        /// <returns></returns>
        public static PlayerDeviceTokenEntity ReadAndBuildMulti(uint playerId, DeviceTokenType type)
        {
            return _instantiate((new PlayerDeviceTokenDao()).CreateOrRead(playerId, type));
        }

        /// <summary>
        /// （ユニーク制約のかかっている）デバイストークン種別とトークンで参照してビルドする。
        /// 外部プッシュ通知 API 利用時など、トークンしか保有していない状態でビルドする時に利用する。
        /// ギャップロックを防ぐために、ロックを掛ける際には必ず ReadType.Master 参照でレコードの有無をチェックすること。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="readType"></param>
        /// <returns></returns>
        public static PlayerDeviceTokenEntity ReadAndBuildByTypeAndToken(DeviceTokenType type, string token, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildByTypeAndTokens(type, new[] { token }, readType).FirstOrDefault();
        }

        /// <summary>
        /// （ユニーク制約のかかっている）デバイストークン種別とトークンで参照してビルドする。
        /// 外部プッシュ通知 API 利用時など、トークンしか保有していない状態でビルドする時に利用する。
        /// ギャップロックを防ぐために、ロックを掛ける際には必ず ReadType.Master 参照でレコードの有無をチェックすること。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tokens"></param>
        /// <param name="readType"></param>
        /// <returns></returns>
        public static IEnumerable<PlayerDeviceTokenEntity >ReadAndBuildByTypeAndTokens(DeviceTokenType type, IEnumerable<string> tokens, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerDeviceTokenDao()).ReadByTypeAndTokens(type, tokens, readType));
        }

        /// <summary>
        /// プレイヤー ID。
        /// </summary>
        public uint PlayerID => _record.PlayerID;
        /// <summary>
        /// デバイストークン種別。
        /// </summary>
        public DeviceTokenType Type => (DeviceTokenType)_record.Type;
        /// <summary>
        /// デバイストークン状態。
        /// </summary>
        public DeviceTokenStatus Status => (DeviceTokenStatus)_record.Status;
        /// <summary>
        /// デバイストークン。
        /// </summary>
        public string Token => _record.Token;

        /// <summary>
        /// プッシュ通知送信可否チェック。
        /// </summary>
        /// <returns></returns>
        public bool CanSendPushNotification()
        {
            return Status == DeviceTokenStatus.Normal && !string.IsNullOrEmpty(Token);
        }

        /// <summary>
        /// トークンをセットする（更新のためには、別途 Save() を呼ぶ必要あり）。
        /// </summary>
        /// <param name="token">トークン</param>
        public void SetDeviceToken(string token)
        {
            var record = _cloneRecord();
            record.Token = token;
            _changeLocalRecord(record);
            // デバイストークンセット時にはステータスを正常にしておく。
            SetStatus(DeviceTokenStatus.Normal);
        }
        /// <summary>
        /// ステータスをセットする（更新のためには、別途 Save() を呼ぶ必要あり）。
        /// </summary>
        /// <param name="status">ステータス</param>
        public void SetStatus(DeviceTokenStatus status)
        {
            var record = _cloneRecord();
            record.Status = (byte)status;
            _changeLocalRecord(record);
        }
    }
}
