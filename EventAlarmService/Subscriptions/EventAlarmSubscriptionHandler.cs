using FTN.Common.EventAlarm.EventSubscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService.Subscriptions
{
	/// <summary>
	/// Handle subscriptions for event, alarm service.
	/// </summary>
	/// 
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

	public class EventAlarmSubscriptionHandler : IEventSubscription
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		private static volatile EventAlarmSubscriptionHandler instance;
		/// <summary>
		/// Objekat za lock
		/// </summary>
		private static object syncRoot = new Object();
		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventAlarmSubscriptionHandler Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventAlarmSubscriptionHandler();
					}
				}

				return instance;
			}
		}
		public EventAlarmSubscriptionHandler() { }

		public void SubscribeToAORAreas(HashSet<long> areas)
		{
			IEventSubscriptionCallback subscriptionCallback =  OperationContext.Current.GetCallbackChannel<IEventSubscriptionCallback>();
			subscriptionCallback.NotifyUser();
			int a = 5;
		}

		public void UnsubscribeFromAORAreas()
		{
			int a = 5;
		}
	}
}
