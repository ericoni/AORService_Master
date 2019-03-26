using EventAlarmService.Subscriptions;
using EventAlarmServiceNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
    class Program
    {
        static void Main(string[] args)
        {
			try
			{
				string message = "Starting EventAlarmCollectorService...\n";
				Console.WriteLine("\n{0}\n", message);
				EventAlarmCollectorService eventAlarmService = new EventAlarmCollectorService();
				eventAlarmService.Start();

				EventAlarmSubscriptionService eventAlarmSubService = new EventAlarmSubscriptionService();
				eventAlarmSubService.Start();

				Console.WriteLine("Press <Enter> to stop the service.");
				Console.ReadKey();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine("EventAlarmCollectorService startup failed.");
				Console.ReadLine();
			}
		}
    }
}
