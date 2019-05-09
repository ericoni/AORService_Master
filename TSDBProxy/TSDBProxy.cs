using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TSDBProxyNS
{
	public class TSDBProxy
	{
		// Broj pokusaja uspostavljanja komunikacije
		private const int maxTry = 5;

		// Spavanje do narednog pokusaja
		private const int sleepTime = 3000;

		public TSDBProxy()
		{
			int tryCounter = 0;

			while (true)
			{
				try
				{
					Proxy = new TSDBChannel();
					Proxy.Open();

					break;
				}
				catch (Exception)
				{

					if (tryCounter.Equals(maxTry))
					{
						throw;
					}

					tryCounter++;
					Thread.Sleep(sleepTime);
				}
			}
		}

		public TSDBChannel Proxy { get; set; }
	}
}
