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
	public class FiveMinutesJob : Job
	{
		/// <summary>
		/// Konstruktor vrsi pokretanje thread-a za agregaciju podataka
		/// </summary>
		public FiveMinutesJob()
		{
			Thread collector = new Thread(() => this.Collector());
			collector.Start();
		}

		public override void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Five minutes job is running...");

			// Inicijalizacija Tabele
			TableInitializer<CollectItem, FiveMinutesItem>();

			// Signaliziraj HourlyJob-u da moze da odradi posao
			FMJ_HJ_Semapthore.Release();

			while (true)
			{
				// Trenutno vreme
				DateTime current = DateTime.Now;

				// Pocetak intervala
				DateTime startTime = CalculateStartInterval(current);

				// Kraj intervala
				DateTime endTime = CalculateEndInterval(startTime);

				// Vreme koliko treba da se spava 
				int sleepTime = (int)(endTime - current).TotalMilliseconds;

				Console.WriteLine(DateTime.Now + ": Five minutes sleeps...");

				// Spava
				Thread.Sleep(sleepTime);

				Console.WriteLine(DateTime.Now + ": Five minutes calculates...");

				// Racunaj i sacuvaj
				CalculateAndStore<CollectItem, FiveMinutesItem>(startTime, endTime);

				// To znaci da treba da pocne novi sat odnosno da se startuje HourlyJob
				if (startTime.Minute == 55)
				{
					// Signaliziraj HourlyJob-u da moze da odradi posao
					FMJ_HJ_Semapthore.Release();
				}
			}
		}

		public override DateTime CalculateStartInterval(DateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, time.Hour,
					(int)Math.Floor((decimal)(time.Minute) / (decimal)5) * 5, 0);
		}

		public override DateTime CalculateEndInterval(DateTime startTime)
		{
			return startTime.AddMinutes(5);
		}
	}
}
