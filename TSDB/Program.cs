using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSDB.Access;
using TSDB.Jobs;
using TSDB.Services;

namespace TSDB
{
	class Program
	{
		static void Main(string[] args)
		{
			FiveMinutesJob fiveMinutesJob = new FiveMinutesJob();
			HourlyJob hourlyJob = new HourlyJob();
			DailyJob dailyJob = new DailyJob();
			MonthlyJob monthlyJob = new MonthlyJob();
			AnnualJob annualJob = new AnnualJob();

			try
			{
				Console.WriteLine("Starting Historian database service...");
				Console.WriteLine("************************************************************************Starting service");
				HistorianDBService historianDB = new HistorianDBService();

				historianDB.Start();

				string message = "\nPress <Enter> to stop the service.";
				Console.WriteLine(message);
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Historian database failed.");
				Console.WriteLine(ex.StackTrace);
				Console.ReadLine();
			}
		}
	}
}
