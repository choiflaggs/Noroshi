using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



#if UNITY_ANDROID

namespace Prime31
{
	public class GoogleCloudMessaging
	{
		private static AndroidJavaObject _plugin;

		static GoogleCloudMessaging()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			// find the plugin instance
			using( var pluginClass = new AndroidJavaClass( "com.prime31.GoogleCloudMessagingPlugin" ) )
				_plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
		}


		// Call this at application launch so the plugin can check for any received notifications via the Intent extras
		// used to launch the application. Note that calling it more than once will result in duplication notificationReceivedEvents
		public static void checkForNotifications()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "checkForNotifications" );
		}


		// Registers the device for push notifications. gcmSenderId is the sender ID as detailed in Google's setup guide (see docs for link)
		public static void register( string gcmSenderId )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "register", gcmSenderId );
		}


		// Unregisters a device for push notification support
		public static void unRegister()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "unRegister" );
		}


		// Cancels all notfications and removes them from the system tray
		public static void cancelAll()
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "cancelAll" );
		}


		// Registers with Push.io
		public static IEnumerator registerDeviceWithPushIO( string deviceId, string pushIOApiKey, List<string> pushIOCategories, Action<bool,string> completionHandler )
		{
			var url = string.Format( "https://api.push.io/r/{0}?di={1}&dt={2}", pushIOApiKey, SystemInfo.deviceUniqueIdentifier, deviceId );

			// add categories if we have them
			if( pushIOCategories != null && pushIOCategories.Count > 0 )
				url += "&c=" + string.Join( ",", pushIOCategories.ToArray() );

			using( WWW www = new WWW( url ) )
			{
				yield return www;

				if( completionHandler != null )
				{
					if( www.text.StartsWith( "ok" ) )
						completionHandler( true, null );
					else
						completionHandler( false, www.error );
				}
			}
		}


		// Sets an alternate key that the plugin will check in the JSON payload when a push notification comes in. Example: if you wanted to use the key
		// 'small-title' instead of the deafult 'subtitle' key you could call this method with both keys to map your key to the 'subtitle' key.
		public static void setPushNotificationAlternateKey( string originalKey, string alternateKey )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setPushNotificationAlternateKey", originalKey, alternateKey );
		}


		// Sets a default value for the specified push notification key. If a notification is received that does not contain the key in the JSON payload
		// the default value set here will be used.
		public static void setPushNotificationDefaultValueForKey( string key, string value )
		{
			if( Application.platform != RuntimePlatform.Android )
				return;

			_plugin.Call( "setPushNotificationDefaultValueForKey", key, value );
		}

	}

}
#endif
