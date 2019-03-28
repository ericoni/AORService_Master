using ActiveAORCache;
using Adapter;
using AORService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AORService
{
	class Program
	{
		static void Main(string[] args)
		{
			string message = "Starting AOR LoginService...\nStarting AOR LoginService...\n";
			Console.WriteLine("\n{0}\n", message);
			Console.WriteLine("************************************************************************Starting services", message);

			AORLoginService aorLoginService = new AORLoginService(); // ovo trigeruje upis u bazu (poziva UserHelpberDB);
			AORViewerCommService aorViewerCommService = new AORViewerCommService();// ovo instancira kes

			try
			{
				aorLoginService.Start();
				Console.WriteLine("AORService has been started.");

				aorViewerCommService.Start();
				Console.WriteLine("AORViewerCommunication server has been started.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
				Console.WriteLine("AORServices startup failed.");
			}

			Console.Read();
		}
	}
}
