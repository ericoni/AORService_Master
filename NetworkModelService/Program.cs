using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using FTN.Common;
using System.Diagnostics;

namespace FTN.Services.NetworkModelService
{
	public class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				string message = "Starting Network Model Service...\nStarting Smart Container Service...";
				Console.WriteLine("\n{0}\n", message);
				Console.WriteLine("************************************************************************Starting services", message);


				Stopwatch stopWatch = new Stopwatch();
				stopWatch.Start();

				NetworkModelService nms = new NetworkModelService();
				Satic(stopWatch, "nms");

				stopWatch.Start();
				SmartContainerService sms = new SmartContainerService();
				Satic(stopWatch, "scs");

				stopWatch.Start();
				sms.Start();
				Satic(stopWatch, "sms Start");

				stopWatch.Start();
				nms.Start();
				Satic(stopWatch, "nms Start()");

				message = "Press <Enter> to stop the service.";
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("NetworkModelService failed.");
				Console.WriteLine(ex.StackTrace);
				Console.ReadLine();
			}
		}
		private static void Satic(Stopwatch stopWatch, string name)
		{
			stopWatch.Stop();

			TimeSpan ts = stopWatch.Elapsed;
			string elapsedTime = String.Format("1 {0:00}:{1:00}:{2:00}.{3:00} " + name,
			   ts.Hours, ts.Minutes, ts.Seconds,
			   ts.Milliseconds / 10);
			Console.WriteLine(elapsedTime);
		}
	}

}

