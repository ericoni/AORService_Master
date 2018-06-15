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
	public class HourlyJob : Job
	{
		/// <summary>
		/// Konstruktor vrsi pokretanje thread-a za agregaciju podataka
		/// </summary>
		public HourlyJob()
		{
			Thread collector = new Thread(() => this.Collector());
			collector.Start();
		}

		/// <summary>
		/// Pokrece se svaki sat, i vrsi racunanje trenutne potrosnje za prethodni sat
		/// </summary>
		public override void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Hourly job is running...");

			//Sacekati da odradi FiveMinuteJob, i da znak
			FMJ_HJ_Semapthore.WaitOne();

			// Inicijalizacija Tabele
			TableInitializer<FiveMinutesItem, HourlyItem>();

			//Signalizirati DailyJob da moze da pocne
			HJ_DJ_Semapthore.Release();

			while (true)
			{
				// Trenutno vreme
				DateTime current = DateTime.Now;

				// Jedan sat pre budjenja
				DateTime startTime = CalculateStartInterval(current);

				// Prvi okrugao sat
				DateTime endTime = CalculateEndInterval(startTime);

				// Vreme koliko treba da se spava do prvog okruglog sata
				int sleepTime = (int)(endTime - current).TotalMilliseconds;

				Console.WriteLine(DateTime.Now + ": Hourly job sleeps...");

				// Spava
				Thread.Sleep(sleepTime);

				Console.WriteLine(DateTime.Now + ": Hourly job calculates...");

				//Sacekati da odradi FiveMinuteJob, i da znak
				FMJ_HJ_Semapthore.WaitOne();

				CalculateAndStore<FiveMinutesItem, HourlyItem>(startTime, endTime);

				// Ako je to kraj dana i treba da se aktivira DailyJob
				if (startTime.Hour == 23)
				{
					//Signalizirati DailyJob da moze da pocne
					HJ_DJ_Semapthore.Release();
				}
			}
		}

		public override DateTime CalculateStartInterval(DateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
		}

		public override DateTime CalculateEndInterval(DateTime startTime)
		{
			return startTime.AddHours(1);
		}
	}
}
