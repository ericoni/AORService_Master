using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherForecastProxyNS
{
	public class WeatherForecastEventProxy
	{
		// Broj pokusaja uspostavljanja komunikacije
		private const int maxTry = 5;

		// Spavanje do narednog pokusaja
		private const int sleepTime = 3000;

		WeatherForecastEventChannel proxy;

		public WeatherForecastEventProxy()
		{
			OpenChannel();
		}

		private void OpenChannel()
		{
			int tryCounter = 0;

			while (true)
			{
				if (tryCounter.Equals(maxTry))
				{
					throw new Exception("WeatherForecastEventProxy: Connection error.");
				}

				try
				{
					proxy = new WeatherForecastEventChannel();
					proxy.Open();

					break;
				}
				catch (Exception)
				{
					tryCounter++;
					Thread.Sleep(sleepTime);
				}
			}
		}

		public WeatherForecastEventChannel Proxy
		{
			get
			{
				return proxy;
			}
		}
	}
}
