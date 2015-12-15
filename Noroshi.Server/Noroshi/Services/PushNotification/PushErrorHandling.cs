using Noroshi.Core.Game.Player;
using Noroshi.Server.Entity.Player;
using System;

namespace PushNotification
{
    public class PushErrorHandling
    {
        /// <summary>
        /// DeviceTokenStatus更新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        public static void ChangeStatusToError(DeviceTokenType type, string token)
        {
            // 更新する必要があるのでロックを掛けてビルドし直す。
            var playerDeviceTokenWithLock = PlayerDeviceTokenEntity.ReadAndBuildByTypeAndToken(type, token);
            // Entity 内で保有しているレコード情報を更新（DB サーバーへの更新 SQL は発行されない）。
            playerDeviceTokenWithLock.SetStatus(DeviceTokenStatus.Error);
            // Entity 内で保有しているレコード情報で DB サーバーへ更新を掛けにいく。                
            if (!playerDeviceTokenWithLock.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Save Player Device Token Status", playerDeviceTokenWithLock.PlayerID, playerDeviceTokenWithLock.Type));
            }
        }

        /// <summary>
        /// DeviceToken変更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="oldToken"></param>
        /// <param name="newToken"></param>
        public static void ChangeDeviceToken(DeviceTokenType type, string oldToken, string newToken)
        {
            // 更新する必要があるのでロックを掛けてビルドし直す。
            var playerDeviceTokenWithLock = PlayerDeviceTokenEntity.ReadAndBuildByTypeAndToken(type, oldToken);
            // Entity 内で保有しているレコード情報を更新（DB サーバーへの更新 SQL は発行されない）。
            playerDeviceTokenWithLock.SetDeviceToken(newToken);
            // Entity 内で保有しているレコード情報で DB サーバーへ更新を掛けにいく。                
            if (!playerDeviceTokenWithLock.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Change Player Device Token", playerDeviceTokenWithLock.PlayerID, playerDeviceTokenWithLock.Type));
            }
        }

        /// <summary>
        /// DeviceToken削除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        public static void DeleteDeviceToken(DeviceTokenType type, string token)
        {
            // 更新する必要があるのでロックを掛けてビルドし直す。
            var playerDeviceTokenWithLock = PlayerDeviceTokenEntity.ReadAndBuildByTypeAndToken(type, token);
            // Entity 内で保有しているレコード情報を更新（DB サーバーへの更新 SQL は発行されない）。
            playerDeviceTokenWithLock.SetStatus(DeviceTokenStatus.Error);
            // Entity 内で保有しているレコード情報で DB サーバーへ更新を掛けにいく。                
            if (!playerDeviceTokenWithLock.Delete())
            {
                throw new SystemException(string.Join("\t", "Fail to Delete Player Device Token", playerDeviceTokenWithLock.PlayerID, playerDeviceTokenWithLock.Type));
            }
        }
    }
}
