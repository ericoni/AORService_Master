using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventAlarmProxyNS
{
	/// <summary>
	/// Proxy which opens duplex channel for communicating with Eventing system.
	/// </summary>
	public class EventSubscriberProxy
	{
		/// <summary>
		/// Channel for Events pub/sub
		/// </summary>
		private IEventSubscription proxy = null;

		private object callbackInstance = null;

		// Broj pokusaja uspostavljanja komunikacije
		private const int maxTry = 7;

		// Spavanje do narednog pokusaja
		private const int sleepTime = 2000;

		/// <summary>
		/// Constructor
		/// </summary>
		public EventSubscriberProxy(object callbackInstance)
		{
			this.callbackInstance = callbackInstance;
			OpenChannel();
		}

		/// <summary>
		/// This method is opening channel to the NMS
		/// </summary>
		private void OpenChannel()
		{
			DuplexChannelFactory<IEventSubscription> factory = new DuplexChannelFactory<IEventSubscription>(
			  new InstanceContext(callbackInstance),
			  new NetTcpBinding(),
			  new EndpointAddress("net.tcp://localhost:10046/IEventSubscription"));

			try
			{
				proxy = factory.CreateChannel();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void SubscribeToAORAreas(HashSet<long> areas) //todo refactor these methods
		{
			int tryCounter = 0;

			while (true)
			{
				try
				{
					//proxy.SubscribeToAORAreas(new HashSet<long>());
					break;
				}
				catch (Exception)
				{
					tryCounter++;

					if (tryCounter.Equals(maxTry))
					{
						throw;
					}

					Thread.Sleep(sleepTime);
					OpenChannel();
				}
			}
		}

		public void UnsubscribeFromAORAreas()
		{
			int tryCounter = 0;

			while (true)
			{
				try
				{
					//proxy.UnsubscribeFromAORAreas();
					break;
				}
				catch (Exception)
				{
					tryCounter++;

					if (tryCounter.Equals(maxTry))
					{
						throw;
					}

					Thread.Sleep(sleepTime);
				}
			}
		}
	}
}
