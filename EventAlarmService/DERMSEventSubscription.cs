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
	/// Service that manages subscriptions and client notification. Problem, kada se radi poziv iz CE instancira se nova instanca.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class DERMSEventSubscription : IDERMSEventSubscription
	{
		#region Fields
		List<IDERMSEventSubscriptionCallback> callbacks = new List<IDERMSEventSubscriptionCallback>();
		static Dictionary<IDERMSEventSubscriptionCallback, List<string>> subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<string>>();
		Dictionary<IDERMSEventSubscriptionCallback, List<string>> subscribers2 = new Dictionary<IDERMSEventSubscriptionCallback, List<string>>();
		static readonly DERMSEventSubscription instance = new DERMSEventSubscription();// to do novo , kako bi se off re-instanciranje kad se javi CE s komandom
		private static object syncRoot = new Object();
		#endregion Fields

		public static DERMSEventSubscription Instance { get { return instance; } }
		//public static DERMSEventSubscription Instance
		//{
		//	get
		//	{
		//		if (instance == null)
		//		{
		//			lock (syncRoot)
		//			{
		//				if (instance == null)
		//					instance = new DERMSEventSubscription();
		//			}
		//		}

		//		return instance;
		//	}
		//}

		static DERMSEventSubscription()
		{
			//callbacks = new List<IDERMSEventSubscriptionCallback>();
			//subscribers = new Dictionary<IDERMSEventSubscriptionCallback, List<long>>();
		}

		private DERMSEventSubscription() { }

		#region IDERMSEvent
		public void Subscribe(List<string> areaNames)
		{
			OperationContext context = OperationContext.Current;
			IDERMSEventSubscriptionCallback callback = context.GetCallbackChannel<IDERMSEventSubscriptionCallback>();

			subscribers.Add(callback, areaNames); //to do add validations here and down there
			subscribers2.Add(callback, areaNames);

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

		public void NotifyClients(string areaName, Event e)
		{
			List<IDERMSEventSubscriptionCallback> callbacksToRemove = new List<IDERMSEventSubscriptionCallback>();

			foreach (KeyValuePair<IDERMSEventSubscriptionCallback, List<string>> subscriber in subscribers)
			{
				if (subscriber.Value.Contains(areaName))
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
