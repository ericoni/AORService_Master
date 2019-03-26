using FTN.Common.EventAlarm.EventSubscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Models
{
	/// <summary>
	/// Callback class for subscriptions.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class EventSubscriberCallback : IEventSubscriptionCallback
	{
		/// <summary>
		/// Singleton instance.
		/// </summary>
		private static volatile EventSubscriberCallback instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		DuplexChannelFactory<IEventSubscription> factory = null;
		IEventSubscription proxy = null;

		private EventSubscriberCallback() { }

		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventSubscriberCallback Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventSubscriberCallback();
					}
				}

				return instance;
			}
		}

		public void NotifyUser()
		{
			int aaa = 55555;
		}

		public void ConnectToTest()
		{
			factory = new DuplexChannelFactory<IEventSubscription>(
			  new InstanceContext(this),
			  new NetTcpBinding(),
			  new EndpointAddress("net.tcp://localhost:10046/IEventSubscription"));
			proxy = factory.CreateChannel();
			proxy.SubscribeToAORAreas(new HashSet<long>());
		}
	}
}
