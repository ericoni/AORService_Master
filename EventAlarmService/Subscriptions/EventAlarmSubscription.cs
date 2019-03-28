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
	/// Service class. Handles subscriptions for event, alarm service. 
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

	public class EventAlarmSubscription : IEventSubscription
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		private static volatile EventAlarmSubscription instance;
		/// <summary>
		/// Objekat za lock
		/// </summary>
		private static object syncRoot = new Object();
		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventAlarmSubscription Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventAlarmSubscription();
					}
				}

				return instance;
			}
		}
		public EventAlarmSubscription() { }

		public void SubscribeToAORAreas(HashSet<long> areas)
		{
			IEventSubscriptionCallback subscriptionCallback =  OperationContext.Current.GetCallbackChannel<IEventSubscriptionCallback>();

			try
			{
				subscriptionCallback.NotifyUser();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void UnsubscribeFromAORAreas()
		{
			int a = 5;
		}
	}
}
