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
	public class AnnualJob : Job
	{
		public AnnualJob()
		{
			Thread collector = new Thread(() => this.Collector());
			collector.Start();
		}

		/// <summary>
		/// Pokrece se svake godine, i vrsi racunanje potrosnje za prethodnu godinu
		/// </summary>
		public override void Collector()
		{
			Console.WriteLine(DateTime.Now + ": Annual job is running...");

			// Ceka signal od MonthlyJob-a
			MJ_YJ_Semapthore.WaitOne();

			// Inicijalizacija Tabele
			TableInitializer<MonthlyItem, AnnualItem>();
			
			while (true)
			{
				DateTime current = DateTime.Now;

				// Pocetak godine
				DateTime startTime = CalculateStartInterval(current);

				// Kraj godine
				DateTime endTime = CalculateEndInterval(startTime);

				Console.WriteLine(DateTime.Now + ": Annual job sleeps...");

				while (true)
				{
					// Trenutno vreme
					current = DateTime.Now;

					// Vreme koliko treba da se spava 
					int sleepTime = (int)(endTime - current).TotalMilliseconds;

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

				// Ceka signal od MonthlyJob-a
				MJ_YJ_Semapthore.WaitOne();

				Console.WriteLine(DateTime.Now + ": Annual job calculates...");

				CalculateAndStore<MonthlyItem, AnnualItem>(startTime, endTime);
			}
		}

		public override DateTime CalculateStartInterval(DateTime time)
		{
			return new DateTime(time.Year, 1, 1, 0, 0, 0);
		}

		public override DateTime CalculateEndInterval(DateTime startTime)
		{
			return startTime.AddYears(1);
		}
	}
}
