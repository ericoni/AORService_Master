using FTN.Common.WeatherForecast.Model;
using FTN.Services.NetworkModelService.DataModel.Wires;
using ModBusSimulatorService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeatherForecastProxyNS;

namespace ModBusSimulatorService.Jobs
{
	public class CurrentWeatherJob
	{
		/// <summary>
		/// Referenca na model
		/// </summary>
		private SimulatorModel model = null;

		/// <summary>
		/// Thread za prikupljanje vremenske prognoze
		/// </summary>
		private Thread collectorThread = null;

		/// <summary>
		/// WeahterForecast proxy
		/// </summary>
		//private WeatherForecastProxy wfProxy = null; // to do vrati weather moje

		public CurrentWeatherJob()
		{
			// Dobijanje referenca modela
			model = SimulatorModel.Instance;

			// Otvaranje kanala prema vremenskoj prognozi
			//wfProxy = new WeatherForecastProxy(); /// to do vrati weather moje

			// Kreiranje i pokretanje Thread-a
			collectorThread = new Thread(() => Collector());
			collectorThread.Start();
		}

		/// <summary>
		/// Thread za prikupljanje trenutne vremenske prognoze za svaki signal, izvrsava se svaki sat vremena
		/// </summary>
		private void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Weather forecast job is running...");

			Dictionary<long, WeatherInfo> currentWeatherData = new Dictionary<long, WeatherInfo>();

			while (true)
			{
				Console.WriteLine(DateTime.Now + ": Weather forecast job started forecasting...");

				foreach (SynchronousMachine der in model.Ders.Values)
				{
					try
					{
						// Dobijemo trenutnu vremensku prognozu za zeljeni der
						//WeatherInfo temp = wfProxy.Proxy.GetCurrentWeatherDataByGlobalId(der.GlobalId); // to do vrati weather moje
						WeatherInfo temp = new WeatherInfo();

						// Ako do sad nije postojao der u Dictionary-ju onda se doda
						if (!currentWeatherData.ContainsKey(der.GlobalId))
						{
							currentWeatherData.Add(der.GlobalId, temp);
							continue;
						}

						// U suprotnom se azurira vrednost
						currentWeatherData[der.GlobalId] = temp;

					}
					catch (Exception ex)
					{
						Console.WriteLine(DateTime.Now + ": " + ex.Message);
					}
				}

				model.CurrentWeathers = currentWeatherData;

				// Trenutno vreme
				DateTime current = DateTime.Now;

				// Prvi okrugao sat
				DateTime wakeUpTime = DateTime.Now;
				wakeUpTime = wakeUpTime.AddHours(1);
				wakeUpTime = wakeUpTime.AddMinutes(-wakeUpTime.Minute);
				wakeUpTime = wakeUpTime.AddSeconds(-wakeUpTime.Second);
				wakeUpTime = wakeUpTime.AddMilliseconds(-wakeUpTime.Millisecond);

				// Vreme koliko treba da se spava do prvog okruglog sata
				int sleepTime = (int)(wakeUpTime - current).TotalMilliseconds;

				Console.WriteLine(DateTime.Now + ": Weather forecast job sleeps...");

				//Thread.Sleep(sleepTime);
				SimulatorModel.Instance.TwoPhaseCommitSemaptore.WaitOne(sleepTime);
			}
		}
	}
}
