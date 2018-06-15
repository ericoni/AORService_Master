using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CalculationEngine.Interfaces;
using CommonCE;
using CalculationEngService;

namespace CalculationEngine.Services
{
    public class CalculationEngineForecastService : IDisposable
    {
        CalculationEngineForecast calculationEngineForecast = null;
        ServiceHost host = null;
        NetTcpBinding binding = null;
        string address = "net.tcp://localhost:10025/IForecastService";

        public CalculationEngineForecastService()
        {
            calculationEngineForecast = CalculationEngineForecast.Instance;
            binding = new NetTcpBinding();
            InitializeHosts();
        }

        private void InitializeHosts()
        {
            host = new ServiceHost(calculationEngineForecast);
        }

        public void Dispose()
        {
            CloseHosts();
            GC.SuppressFinalize(this);
        }

        private void CloseHosts()
        {
            if (host == null)
            {
                throw new Exception("Calculation Engine Forecast Service can not be closed because it is not initialized.");
            }

            host.Close();

            string message = "Calculation Engine Forecast Service is closed.";
            //CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
            Console.WriteLine("\n\n{0}", message);
        }

        public void Start()
        {
            StartHosts();
        }

        private void StartHosts()
        {
            if (host == null)
            {
                throw new Exception("Calculation Engine Forecast Service can not be opend because it is not initialized.");
            }

            string message = string.Empty;

            try
            {
                host.AddServiceEndpoint(typeof(IForecastService), binding, address);
                host.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            message = string.Format("The WCF service {0} is ready.", host.Description.Name);
            Console.WriteLine(message);

            message = "Endpoints:";
            Console.WriteLine(message);
            Console.WriteLine(address);

            message = "The Calculation Engine Forecast Service is started.";
            Console.WriteLine("{0}", message);

            Console.WriteLine("************************************************************************Calculation Engine Forecast started", message);
        }

    }
}
