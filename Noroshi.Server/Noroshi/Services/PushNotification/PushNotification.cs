using System;
using System.IO;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Android;
using Newtonsoft.Json;
using System.Collections.Generic;
using Noroshi.Core.Game.Player;
using System.Diagnostics;

namespace PushNotification
{
    public class PushNotification
    {
        static PushBroker _pushBroker;
        static bool _isAndroid = true;
        static bool _isApple = true;
        static bool _useResponseHandling = false;
        static bool _debug = true;

        string _appleCertFileName;
        string _appleCertPassword;
        string _androidSenderID;
        string _androidAPIKey;
        string _androidAppPackageName;

        /// <summary>
        /// Create push services
        /// </summary>
        public PushNotification()
        {
            SettingsModel settings = UtilLog.GetSettingsInfo();
            
            _appleCertFileName = settings.AppleCertFileName;
            _appleCertPassword = settings.AppleCertPassword;
            _androidSenderID = settings.AndroidSenderID;
            _androidAPIKey = settings.AndroidAPIKey;
            _androidAppPackageName = settings.AndroidAppPackageName;
            
            string _appleCertFilePath = UtilLog.GetPushSettingsFileDirectory() + _appleCertFileName;

            if (_pushBroker == null)
            {
                _pushBroker = new PushBroker();

                _pushBroker.OnNotificationSent += NotificationSent;
                _pushBroker.OnChannelException += ChannelException;
                _pushBroker.OnServiceException += ServiceException;
                _pushBroker.OnNotificationFailed += NotificationFailed;
                _pushBroker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                _pushBroker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                _pushBroker.OnChannelCreated += ChannelCreated;
                _pushBroker.OnChannelDestroyed += ChannelDestroyed;

                if (_isApple)
                {
                    var appleCert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _appleCertFilePath));
                    _pushBroker.RegisterAppleService(new ApplePushChannelSettings(false, appleCert, _appleCertPassword));
                }

                if (_isAndroid)
                {
                    _pushBroker.RegisterGcmService(new GcmPushChannelSettings(_androidSenderID, _androidAPIKey, _androidAppPackageName));
                }
            }
        }

        /// <summary>
        /// QueueNotification:Apple
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="message"></param>
        public void PushNotificationApple(string deviceToken, string message)
        {
            if (_pushBroker != null)
            {
                _pushBroker.QueueNotification(new AppleNotification()
                                            .ForDeviceToken(deviceToken)
                                            .WithAlert(message)
                                            .WithBadge(1)
                                            .WithSound("sound.caf"));
            }
        }

        /// <summary>
        /// QueueNotification:Android
        /// </summary>
        /// <param name="deviceRegistrationId"></param>
        /// <param name="message"></param>
        public void PushNotificationAndroid(string deviceRegistrationId, string message)
        {
            if (_pushBroker != null)
            {
                string json;
                IDictionary<string, string> pushdata = new Dictionary<string, string>();

                pushdata.Add("alert", message);
                pushdata.Add("badge", "1");
                pushdata.Add("sound", "sound.caf");
                json = JsonConvert.SerializeObject(pushdata);

                _pushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceRegistrationId)
                                                            .WithJson(json));
            }
        }

        /// <summary>
        /// StopAllServices：PushのQueueが空になるとき
        /// </summary>
        public void StopAllServices()
        {
            if (_debug)
                Debug.WriteLine(DateTime.Now.ToString("F") + "::Waiting for Queue to Finish...");

            //Stop and wait for the queues to drains
            _pushBroker.StopAllServices();

            if (_debug)
                Debug.WriteLine(DateTime.Now.ToString("F") + "::Queue Finished...\r\n");
        }

        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            string respMessage = "Device Registration Changed: Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + "-> " + notification;

            if (sender.ToString() == "PushSharp.Android.GcmPushService")
            {
                if (_useResponseHandling)
                    PushErrorHandling.ChangeDeviceToken(DeviceTokenType.Android, oldSubscriptionId, newSubscriptionId);
            }

            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            string respMessage = "Sent: " + sender + "-> " + notification;
            
            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            string respMessage = "Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification;

            var gcmNotification = notification as GcmNotification;
            if (sender.ToString() == "PushSharp.Android.GcmPushService")
            {
                foreach (var registrationId in gcmNotification.RegistrationIds)
                {
                    if (_useResponseHandling)
                        PushErrorHandling.ChangeStatusToError(DeviceTokenType.Android, registrationId);
                }
            }
            
            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            string respMessage = "Channel Exception: " + sender + " -> " + exception;

            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void ServiceException(object sender, Exception exception)
        {
            string respMessage = "Service Exception: " + sender + " -> " + exception;

            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            string respMessage = "Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId;
            if (sender.ToString() == "PushSharp.Android.GcmPushService")
            {
                if (_useResponseHandling)
                    PushErrorHandling.ChangeStatusToError(DeviceTokenType.Android, expiredDeviceSubscriptionId);
            }
            else if (sender.ToString() == "PushSharp.Apple.ApplePushService")
            {
                if (_useResponseHandling)
                    PushErrorHandling.ChangeStatusToError(DeviceTokenType.iOS, expiredDeviceSubscriptionId);
            }

            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void ChannelDestroyed(object sender)
        {
            string respMessage = "Channel Destroyed for: " + sender;

            if (_debug)
                Debug.WriteLine(respMessage);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            string respMessage = "Channel Created for: " + sender;

            if (_debug)
                Debug.WriteLine(respMessage);
        }
    }
}
