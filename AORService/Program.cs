using ActiveAORCache;
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
			//AORCache aorCache = new AORCache(); // vrati se da sredis ovo 

			string message = "Starting AOR LoginService...\nStarting AOR LoginService...\n";
			Console.WriteLine("\n{0}\n", message);
			Console.WriteLine("************************************************************************Starting services", message);

			AORLoginService aorLoginService = new AORLoginService();

			try
			{
				aorLoginService.Start();
				Console.WriteLine("AORService has been started.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("{0}, StackTrace: {1}", ex.Message, ex.StackTrace));
				Console.WriteLine("AORService startup failed.");
			}

			Console.Read();
		}
	}
}
