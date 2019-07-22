using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class DERMSEventSubscription : IDERMSEventSubscription
	{
		List<IDERMSEventSubscriptionCallback> callbacks = null;
		private Dictionary<IDERMSEventSubscriptionCallback, List<long>> subscribers;

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
			throw new NotImplementedException();
		}
		#endregion IDERMSEvent

		public void NotifyClients(long gid, string message)
		{
			foreach (KeyValuePair<IDERMSEventSubscriptionCallback, List<long>> entry in subscribers)
			{
				// do something with entry.Value or entry.Key
				if (entry.Value.Contains(gid))
				{
					entry.Key.ReceiveEvent(message);
				}

			}

			//Action<IDERMSEventSubscriptionCallback> invoke = callback => callback.ReceiveEvent(message);
			//callbacks.ForEach(invoke);		}
	}
}
