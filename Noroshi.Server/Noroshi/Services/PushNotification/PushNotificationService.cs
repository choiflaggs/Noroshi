using System;
using System.Collections.Generic;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Entity.PushNotification;

namespace PushNotification
{
    public class PushNotificationService
    {
        static PushNotificationQueueEntity _pushNotificationQueueEntity;
        static PushNotificationServiceAsync _asyncPushNotificationService;

        public PushNotificationService()
        {
            if (_pushNotificationQueueEntity == null)
            {
                _pushNotificationQueueEntity = new PushNotificationQueueEntity();
            }
            if (_asyncPushNotificationService == null)
            {
                _asyncPushNotificationService = new PushNotificationServiceAsync();
            }
        }

        /// <summary>
        /// プッシュ通知を送信する。
        /// </summary>
        public static void Send()
        {
            var playerDeviceTokens = _pushNotificationQueueEntity.ReadAndBuildOlderActive();
            foreach (var playerDeviceToken in playerDeviceTokens)
            {
                uint id = playerDeviceToken.ID;
                DeviceTokenType type = playerDeviceToken.DeviceType;
                string token = playerDeviceToken.DeviceToken;
                string message = playerDeviceToken.Message;

                _send(token, type, message);
                _deletePushQueueByID(id);
            }
            _asyncPushNotificationService.StopAllServices();
        }

        /// <summary>
        /// プッシュ通知
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        static async void _send(string token, DeviceTokenType type, string message)
        {
            if (type == DeviceTokenType.iOS)
            {
                await _asyncPushNotificationService.PushNotificationAppleAsync(token, message);
            }
            if (type == DeviceTokenType.Android)
            {
                await _asyncPushNotificationService.PushNotificationAndroidAsync(token, message);
            }
        }

        /// <summary>
        /// PushQueueを削除(id)
        /// </summary>
        /// <param name="id">Queue id</param>
        static void _deletePushQueueByID(uint id)
        {
            // 削除する必要があるのでロックを掛けてビルドし直す。
            var pushQueueWithLock = _pushNotificationQueueEntity.ReadAndBuild(id);
            // Entity 内で保有しているレコード情報で DB サーバーへ削除を掛けにいく。                
            if (!pushQueueWithLock.Delete())
            {
                throw new SystemException(string.Join("\t", "Fail to Delete PushQueue.", pushQueueWithLock.ID, pushQueueWithLock.DeviceType));
            }
        }

        /// <summary>
        /// PushQueueを削除(ALL)
        /// </summary>
        /// <param name="playerDeviceTokens"></param>
        static void _deletePushQueueByAll(IEnumerable<PushNotificationQueueEntity> playerDeviceTokens)
        {
            foreach (var playerDeviceToken in playerDeviceTokens)
            {
                uint id = playerDeviceToken.ID;
                DeviceTokenType type = playerDeviceToken.DeviceType;
                string token = playerDeviceToken.DeviceToken;
                string message = playerDeviceToken.Message;

                // 更新する必要があるのでロックを掛けてビルドし直す。
                var pushQueueWithLock = _pushNotificationQueueEntity.ReadAndBuild(playerDeviceToken.ID);
                // Entity 内で保有しているレコード情報で DB サーバーへ削除を掛けにいく。                
                if (!pushQueueWithLock.Delete())
                {
                    throw new SystemException(string.Join("\t", "Fail to Delete PushQueue.", pushQueueWithLock.ID, pushQueueWithLock.DeviceType));
                }
            }
        }
    }
}