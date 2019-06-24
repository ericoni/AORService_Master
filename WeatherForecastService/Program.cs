using FTN.Common.WeatherForecast.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WeatherForecast;

namespace WeatherForecast
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string message = "Starting Weather Forecast Service...";
				Console.WriteLine("\n{0}\n", message);
				Console.WriteLine("************************************************************************Starting services", message);

				//WeatherForecastService wf = new WeatherForecastService();

				//wf.Start();

				message = "Press <Enter> to stop the service.";
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("WeatherForecast failed.");
				Console.WriteLine(ex.StackTrace);
				Console.ReadLine();
			}
		}
	}
}
