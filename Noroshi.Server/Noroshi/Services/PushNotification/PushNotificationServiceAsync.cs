using System.Threading.Tasks;

namespace PushNotification
{
    public class PushNotificationServiceAsync
    {
        static PushNotification _pushNotification;

        public PushNotificationServiceAsync()
        {
            if (_pushNotification == null)
            {
                _pushNotification = new PushNotification();
            }
        }

        /// <summary>
        /// iOS : Send the push notifications to the device
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="message"></param>
        public async Task PushNotificationAppleAsync(string deviceToken, string message)
        {
            if (_pushNotification != null)
            {
                await Task.Run(() => {
                    _pushNotification.PushNotificationApple(deviceToken, message);
                });
            }
        }

        /// <summary>
        /// Android : Send the push notifications to the device
        /// </summary>
        /// <param name="registerationId"></param>
        /// <param name="message"></param>
        public async Task PushNotificationAndroidAsync(string registerationId, string message)
        {
            if (_pushNotification != null)
            {
                await Task.Run(() => {
                    _pushNotification.PushNotificationAndroid(registerationId, message);
                });
            }
        }

        /// <summary>
        /// Stop All Services
        /// </summary>
        public void StopAllServices()
        {
            Task.Run(() =>
            {
                //Stop and wait for the queues to drains
                _pushNotification.StopAllServices();
            });
        }
    }
}

