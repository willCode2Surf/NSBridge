using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Security.AccessControl;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;


namespace NSB.Utils
{
	/// <summary>
	/// This class contains the code shown in the article "Working with the File System"
	/// </summary>
	public class NotificationUtils
	{
		string deviceToken = string.Empty;
		public string DeviceToken { get { return deviceToken; } }

		public static void RegisterForRemoteNotifications()
		{
			//==== register for remote notifications and get the device token
			// set what kind of notification types we want
			UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge;
			// register for remote notifications
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);

		}



		public static void CheckForNotifications(NSDictionary options)
		{
			// check for a notification
			if(options != null) {

				CheckForLocalNotifications (options);
				CheckForRemoteNotifications (options);

			}
		}

		public static void CheckForLocalNotifications(NSDictionary options)
		{
			// check for a local notification
			if(options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey)) {

				UILocalNotification localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
				if(localNotification != null) {

					new UIAlertView(localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show();
					// reset our badge
					UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
				}
			}


		}

		public static void CreateLocalNotification(NSDate fireDateTime,string action, string body, string uiLocalNotificationSoundName)
		{

			UILocalNotification notification = new UILocalNotification ();
			notification.FireDate = fireDateTime;
			notification.AlertAction = action;
			notification.AlertBody = body;
			notification.ApplicationIconBadgeNumber += 1;
			notification.SoundName = uiLocalNotificationSoundName;
			UIApplication.SharedApplication.ScheduleLocalNotification (notification);
		}

		public static void CreateLocalNotification(UILocalNotification notification)
		{


			UIApplication.SharedApplication.ScheduleLocalNotification (notification);
		}


		public static void CheckForRemoteNotifications(NSDictionary options)
		{
			// check for a remote notification
			if(options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey)) {

				NSDictionary remoteNotification = options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
				if(remoteNotification != null) {
					//new UIAlertView(remoteNotification.AlertAction, remoteNotification.AlertBody, null, "OK", null).Show();
				}
			}
		}


	}
}