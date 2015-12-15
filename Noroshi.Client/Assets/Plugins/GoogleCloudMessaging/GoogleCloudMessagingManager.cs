using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;


#if UNITY_ANDROID

namespace Prime31
{
	public class GoogleCloudMessagingManager : AbstractManager
	{
		// Fired when a notification is received and provides the message parameter sent via push
		public static event Action<Dictionary<string,object>> notificationReceivedEvent;
	
		// Fired when remote registration succeeds and provides the registrationId
		public static event Action<string> registrationSucceededEvent;
	
		// Fired when registration fails
		public static event Action<string> registrationFailedEvent;
		
		// Fired when unregistration succeeds
		public static event Action unregistrationSucceededEvent;
		
		// Fired when unregistration fails
		public static event Action<string> unregistrationFailedEvent;
	
		
		
		static GoogleCloudMessagingManager()
		{
			AbstractManager.initialize( typeof( GoogleCloudMessagingManager ) );
		}
	
	
		public void notificationReceived( string json )
		{		
			notificationReceivedEvent.fire( json.dictionaryFromJson() );
		}
	
	
		public void registrationSucceeded( string registrationId )
		{
			registrationSucceededEvent.fire( registrationId );
		}
	
	
		public void unregistrationFailed( string param )
		{
			unregistrationFailedEvent.fire( param );
		}
	
	
		public void registrationFailed( string error )
		{
			registrationFailedEvent.fire( error );
		}
	
	
		public void unregistrationSucceeded( string empty )
		{
			unregistrationSucceededEvent.fire();
		}
	
	
	
	}

}
#endif
