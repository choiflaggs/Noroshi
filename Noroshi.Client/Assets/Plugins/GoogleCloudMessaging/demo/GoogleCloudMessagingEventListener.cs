using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace Prime31
{
	public class GoogleCloudMessagingEventListener : MonoBehaviour
	{
#if UNITY_ANDROID
		void OnEnable()
		{
			// Listen to all events for illustration purposes
			GoogleCloudMessagingManager.notificationReceivedEvent += notificationReceivedEvent;
			GoogleCloudMessagingManager.registrationSucceededEvent += registrationSucceededEvent;
			GoogleCloudMessagingManager.unregistrationFailedEvent += unregistrationFailedEvent;
			GoogleCloudMessagingManager.registrationFailedEvent += registrationFailedEvent;
			GoogleCloudMessagingManager.unregistrationSucceededEvent += unregistrationSucceededEvent;
		}
	
	
		void OnDisable()
		{
			// Remove all event handlers
			GoogleCloudMessagingManager.notificationReceivedEvent -= notificationReceivedEvent;
			GoogleCloudMessagingManager.registrationSucceededEvent -= registrationSucceededEvent;
			GoogleCloudMessagingManager.unregistrationFailedEvent -= unregistrationFailedEvent;
			GoogleCloudMessagingManager.registrationFailedEvent -= registrationFailedEvent;
			GoogleCloudMessagingManager.unregistrationSucceededEvent -= unregistrationSucceededEvent;
		}
	
	
	
		void notificationReceivedEvent( Dictionary<string,object> dict )
		{
			Debug.Log( "notificationReceivedEvent" );
			Prime31.Utils.logObject( dict );
		}
	
	
		void registrationSucceededEvent( string registrationId )
		{
			Debug.Log( "registrationSucceededEvent: " + registrationId );
		}
	
	
		void unregistrationFailedEvent( string error )
		{
			Debug.Log( "unregistrationFailedEvent: " + error );
		}
	
	
		void registrationFailedEvent( string error )
		{
			Debug.Log( "registrationFailedEvent: " + error );
		}
	
	
		void unregistrationSucceededEvent()
		{
			Debug.Log( "UnregistrationSucceededEvent" );
		}
	
	
#endif
	}

}
	
	
