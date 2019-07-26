using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventCollectorProxyNS
{
	public class EventCollectorProxy
	{
		// Broj pokusaja uspostavljanja komunikacije
		private const int maxTry = 7;

		// Spavanje do narednog pokusaja
		private const int sleepTime = 2000;

		EventCollectorChannel proxy;

		public EventCollectorProxy()
		{
			OpenChannel();
		}

		private void OpenChannel()
		{
			int tryCounter = 0;

			while (true)
			{
				try
				{
					proxy = new EventCollectorChannel();
					proxy.Open();

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

		public EventCollectorChannel Proxy
		{
			get
			{
				if (proxy.State != System.ServiceModel.CommunicationState.Opened)
				{
					OpenChannel();
				}

				return proxy;
			}
		}
	}
}
