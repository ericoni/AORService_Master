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
		#region Fields
		List<IDERMSEventSubscriptionCallback> callbacks = new List<IDERMSEventSubscriptionCallback>();
		static Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers2 = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		static DERMSEventSubscription instance = null;
        private static object syncRoot = new Object();
        int counterForCst = 0;
		#endregion Fields

		public static DERMSEventSubscription Instance
		{
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DERMSEventSubscription();
                    }
                }

                return instance;
            }
        }

		public DERMSEventSubscription()
		{
			counterForCst++;
			//callbacks = new List<IDERMSEventSubscriptionCallback>();
			//subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
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
			subscribers2.Add(callback, areaGids); //to do add validation

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
            List<IDERMSEventSubscriptionCallback> callbacksToRemove = new List<IDERMSEventSubscriptionCallback>();

            foreach (KeyValuePair<IDERMSEventSubscriptionCallback, List<long>> subscriber in subscribers)
			{
				if (subscriber.Value.Contains(gid))
				{
					try
					{
                        subscriber.Key.ReceiveEvent(e);
					}
					catch (System.ServiceModel.CommunicationObjectAbortedException)
					{
                        callbacksToRemove.Add(subscriber.Key);
                        continue;
					}
					catch (Exception)
					{
						throw;
					}
				}
			}
            callbacksToRemove.ForEach(a => subscribers.Remove(a));
			//Action<IDERMSEventSubscriptionCallback> invoke = callback => callback.ReceiveEvent(message);
			//callbacks.ForEach(invoke);
		}
	}
}
