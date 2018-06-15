using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using FTN.Common;

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

				NetworkModelService nms = new NetworkModelService();
				SmartContainerService sms = new SmartContainerService();

				sms.Start();
				nms.Start();

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
	}
}
