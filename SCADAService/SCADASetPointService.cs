using FTN.Common;
using FTN.Common.SCADA.Services;
using SCADAService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class SCADASetpointService : IDisposable
    {
        SCADASetpoint scadaReceiving = null;
        ServiceHost host = null;
        NetTcpBinding binding = null;
        string address = "net.tcp://localhost:10014/ISCADAReceiving";

        public SCADASetpointService()
        {
            scadaReceiving = SCADASetpoint.Instance;
            binding = new NetTcpBinding();
            InitializeHosts();
        }

        private void InitializeHosts()
        {
            host = new ServiceHost(scadaReceiving);
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
                throw new Exception("SCADA Receiving Service can not be closed because it is not initialized.");
            }

            host.Close();

            string message = "The SCADA Receiving Service is closed.";
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
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
                throw new Exception("SCADA Receiving can not be opened because it is not initialized.");
            }

            string message = string.Empty;

            try
            {
                host.AddServiceEndpoint(typeof(ISCADAReceiving), binding, address);
                host.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            message = string.Format("The WCF service {0} is ready.", host.Description.Name);
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            message = "Endpoints:";
            Console.WriteLine(message);
            Console.WriteLine(address);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            message = "The SCADA Receiving Service is started.";
            Console.WriteLine("{0}", message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            Console.WriteLine("************************************************************************SCADA Receiving started", message);
        }
    }
}
