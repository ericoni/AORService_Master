using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
	/// <summary>
	/// Implements <seealso cref="IDERMSEventSubscription"/> interface.
	/// Service that manages subscriptions and client notification.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class DERMSEventSubscription : IDERMSEventSubscription
	{
		List<IDERMSEventSubscriptionCallback> callbacks = null;
		private Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers;
        static DERMSEventSubscription instance = null;

        public static DERMSEventSubscription Instance
        {
            get
            {
                if (instance == null)
                    return new DERMSEventSubscription();
                return instance;
            }
        }

		public DERMSEventSubscription()
		{
			callbacks = new List<IDERMSEventSubscriptionCallback>();
			subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		}

		#region IDERMSEvent
		/// <summary>
		/// 
		/// </summary>
		/// <param name="areaGids"></param>
		public void Subscribe(List<long> areaGids)
		{
			OperationContext context = OperationContext.Current;
			IDERMSEventSubscriptionCallback callback = context.GetCallbackChannel<IDERMSEventSubscriptionCallback>();

			subscribers.Add(callback, areaGids); //to do add validation

			if (callbacks.Contains(callback) == false)
				callbacks.Add(callback);
		}

		public void Unsubscribe()
		{
            OperationContext context = OperationContext.Current;
            IDERMSEventSubscriptionCallback callback = context.GetCallbackChannel<IDERMSEventSubscriptionCallback>();

            if (callbacks.Contains(callback) == true)
                callbacks.Remove(callback);
        }
		#endregion IDERMSEvent

		public void NotifyClients(long gid, Event e)
		{
			foreach (KeyValuePair<IDERMSEventSubscriptionCallback, List<long>> entry in subscribers)
			{
				if (entry.Value.Contains(gid))
				{
					entry.Key.ReceiveEvent(e);
				}
			}
			//Action<IDERMSEventSubscriptionCallback> invoke = callback => callback.ReceiveEvent(message);
			//callbacks.ForEach(invoke);
		}
	}
}
