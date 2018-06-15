using FTN.Common.WeatherForecast.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastService;

namespace WeatherForecast
{
	public class WeatherForecastService : IDisposable
	{
		ServiceHost host = null;
		NetTcpBinding binding = null;
		string address = "net.tcp://localhost:10020/IWeatherForecast";

		public WeatherForecastService()
		{
			binding = new NetTcpBinding();
			InitializeHosts();
		}

		private void InitializeHosts()
		{
			host = new ServiceHost(WeatherForcast.Instance);
		}

		public void Dispose()
		{
			CloseHosts();
			GC.SuppressFinalize(this);
		}

		private void CloseHosts()
		{
			if (host == null)
			{
				throw new Exception("Weather Forecast Services can not be closed because it is not initialized.");
			}

			host.Close();

			string message = "The Weather Forecast Service is closed.";
			Console.WriteLine("\n\n{0}", message);
		}

		public void Start()
		{
			StartHosts();
		}

		private void StartHosts()
		{
			if (host == null)
			{
				throw new Exception("Weather Forecast can not be opend because it is not initialized.");
			}

			string message = string.Empty;

			try
			{
				host.AddServiceEndpoint(typeof(IWeatherForecast), binding, address);
				host.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			message = string.Format("The WCF service {0} is ready.", host.Description.Name);
			Console.WriteLine(message);

			message = "Endpoints:";
			Console.WriteLine(message);
			Console.WriteLine(address);

			message = "The Weather Forecast Service is started.";
			Console.WriteLine("{0}", message);

			Console.WriteLine("************************************************************************Weather Forecast sterted", message);
		}
	}
}
