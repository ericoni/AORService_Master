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
		#region Field
		List<IDERMSEventSubscriptionCallback> callbacks = new List<IDERMSEventSubscriptionCallback>();
		static List<IDERMSEventSubscriptionCallback> callbacks2 = new List<IDERMSEventSubscriptionCallback>();
		static Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers2 = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		static DERMSEventSubscription instance = null;
		int counterForCst = 0;
		#endregion Field

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
			if (callbacks2.Contains(callback) == false)
				callbacks2.Add(callback);
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
			{//to do problem prazna lista subscribers

				if (entry.Value.Contains(gid))
				{
					try
					{
						entry.Key.ReceiveEvent(e);
					}
					catch (ObjectDisposedException)
					{
						throw;
					}
					catch (Exception)
					{

						throw;
					}
				}
			}
			//Action<IDERMSEventSubscriptionCallback> invoke = callback => callback.ReceiveEvent(message);
			//callbacks.ForEach(invoke);
		}
	}
}
