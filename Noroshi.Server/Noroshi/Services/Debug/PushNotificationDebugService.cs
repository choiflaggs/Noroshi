using PushNotification;

namespace Noroshi.Server.Services.Debug
{
    public class PushNotificationDebugService
    {
        /// <summary>
        /// プッシュ通知を送信する。
        /// </summary>
        /// <param name="targetPlayerIds">送信先プレイヤー ID</param>
        /// <param name="message">送信内容</param>
        public static void Send(uint[] targetPlayerIds, string message)
        {
            PushNotificationService.Send();
        }
    }
}
