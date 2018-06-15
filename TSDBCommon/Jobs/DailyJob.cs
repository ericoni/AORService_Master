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
	public class DailyJob : Job
	{
		/// <summary>
		/// Konstruktor vrsi pokretanje thread-a za agregaciju podataka
		/// </summary>
		public DailyJob()
		{
			Thread collector = new Thread(() => this.Collector());
			collector.Start();
		}

		/// <summary>
		/// Pokrece se svaki dan u 24h, i vrsi racunanje trenutne potrosnje za prethodni dan
		/// </summary>
		public override void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Daily job is running...");

			// Ceka znak od HourlyJob-a da je zavrsio pa da moze da odradi svoj posao
			HJ_DJ_Semapthore.WaitOne();

			// Inicijalizacija Tabele
			TableInitializer<HourlyItem, DailyItem>();

			// Obavesti MonthlyJob da moze da pocne
			DJ_MJ_Semapthore.Release();

			while (true)
			{
				// Trenutno vreme
				DateTime current = DateTime.Now;

				// Pocetak dana
				DateTime startTime = CalculateStartInterval(current);

				// 23:59 min - kraj dana
				DateTime endTime = CalculateEndInterval(startTime);

				// Vreme koliko treba da se spava do prvog okruglog sata
				int sleepTime = (int)(endTime - current).TotalMilliseconds;

				Console.WriteLine(DateTime.Now + ": Daily job sleeps...");

				// Spava do kraja dana
				Thread.Sleep(sleepTime);

				// Ceka znak od HourlyJob-a da je zavrsio pa da moze da odradi svoj posao
				HJ_DJ_Semapthore.WaitOne();

				Console.WriteLine(DateTime.Now + ": Daily job calculates...");

				CalculateAndStore<HourlyItem, DailyItem>(startTime, endTime);

				// Ako je to zadnji dan u mesecu i kraj dana onda treba da se aktivira MonthlyJob
				if (current.Day == DateTime.DaysInMonth(current.Year, current.Month) && current.Hour == 23)
				{
					// Obavesti MonthlyJob da moze da pocne
					DJ_MJ_Semapthore.Release();
				}
			}
		}

		public override DateTime CalculateStartInterval(DateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
		}

		public override DateTime CalculateEndInterval(DateTime startTime)
		{
			return startTime.AddDays(1);
		}
	}
}
