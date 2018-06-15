using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSDB.Access;
using TSDB.Converter;
using TSDB.Helper;
using TSDB.Model;

namespace TSDB.Jobs
{
	public class MonthlyJob : Job
	{
		/// <summary>
		/// Konstruktor vrsi pokretanje thread-a za agregaciju podataka
		/// </summary>
		public MonthlyJob()
		{
			Thread collector = new Thread(() => this.Collector());
			collector.Start();
		}

		/// <summary>
		/// Pokrece se svaki mesec, i vrsi racunanje trenutne potrosnje za prethodni mesec
		/// </summary>
		public override void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Monthly job is running...");

			// Ceka signal od DailyJob-a da je zavrsio posao 
			DJ_MJ_Semapthore.WaitOne();

			// Inicijalizacija Tabele
			TableInitializer<DailyItem, MonthlyItem>();

			// Signalizira AnnualJob-u da moze da pocne
			MJ_YJ_Semapthore.Release();

			while (true)
			{
				DateTime current = DateTime.Now;

				// Pocetak meseca
				DateTime startTime = CalculateStartInterval(current);

				// Kraj meseca
				DateTime endTime = CalculateEndInterval(startTime);

				Console.WriteLine(DateTime.Now + ": Monthly job sleeps...");

				while (true)
				{
					// Trenutno vreme
					current = DateTime.Now;

					// Vreme koliko treba da se spava 
					int sleepTime = (int)((endTime - current).TotalMilliseconds);

					if (sleepTime < 0)
					{
						Thread.Sleep(Int32.MaxValue);
					}
					else
					{
						Thread.Sleep(sleepTime);
						break;
					}
				}

				// Ceka signal od DailyJob-a da je zavrsio posao 
				DJ_MJ_Semapthore.WaitOne();

				Console.WriteLine(DateTime.Now + ": Monthly job calculates...");

				CalculateAndStore<DailyItem, MonthlyItem>(startTime, endTime);

				// Ako je to decembar, poslednji dan u godini, i poslednji sat u danu tada treba da se aktivira AnnualJob
				if (current.Month == 12 && current.Day == DateTime.DaysInMonth(current.Year, current.Month) && current.Hour == 23)
				{
					// Signalizira AnnualJob-u da moze da pocne
					MJ_YJ_Semapthore.Release();
				}
			}
		}

		public override DateTime CalculateStartInterval(DateTime time)
		{
			return new DateTime(time.Year, time.Month, 1, 0, 0, 0);
		}

		public override DateTime CalculateEndInterval(DateTime startTime)
		{
			return startTime.AddMonths(1);
		}
	}
}
