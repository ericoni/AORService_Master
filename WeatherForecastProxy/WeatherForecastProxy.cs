using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherForecastProxyNS
{
	public class WeatherForecastProxy
	{
		// Broj pokusaja uspostavljanja komunikacije
		private const int maxTry = 5;

		// Spavanje do narednog pokusaja
		private const int sleepTime = 3000;

		WeatherForecastChannel proxy;

		public WeatherForecastProxy()
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
					proxy = new WeatherForecastChannel();
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

		public WeatherForecastChannel Proxy
		{
			get
			{
				return proxy;
			}
		}
	}
}

