using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using SmartCacheLibrary.Interfaces;
using FTN.Services.NetworkModelService.DataModel.Meas;

namespace SmartCacheLibrary.Services
{
    public class UpdateCacheService : IDisposable
    {

        SmartCache smartCache = null;
        ServiceHost host = null;
        NetTcpBinding binding = null;
        string address = "net.tcp://localhost:10012/ICacheService";

        public UpdateCacheService()
        {
            smartCache = SmartCache.Instance;
            binding = new NetTcpBinding();
            InitializeHosts();
        }

        private void InitializeHosts()
        {
            host = new ServiceHost(smartCache);
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
                throw new Exception("Smart Cache Services can not be closed because it is not initialized.");
            }

            host.Close();

            string message = "The Smart Cache Service is closed.";
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
                throw new Exception("Smart cache can not be opend because it is not initialized.");
            }

            string message = string.Empty;

            try
            {
                host.AddServiceEndpoint(typeof(ICacheService), binding, address);
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

            message = "The Smart Cache Service is started.";
            Console.WriteLine("{0}", message);

            Console.WriteLine("************************************************************************Smart Cache started", message);
        }

    }
}
