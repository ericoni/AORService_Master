using FTN.Services.NetworkModelService.DataModel.Meas;
using MODBUSLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Adapter;
using System.ServiceModel;
using FTN.ServiceContracts;
using FTN.Common;
using NMSSub;
using SCADAService;

namespace SCADA
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string message = "Starting SCADA Service...\nStarting SCADA Subscriber Service...\n Starting SCADA Receiving Service";
                CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);
                Console.WriteLine("\n{0}\n", message);
                Console.WriteLine("************************************************************************Starting services", message);

                SCADACrunching scadaService = new SCADACrunching();
                SCADASubscriberService scadaSubscriberService = new SCADASubscriberService();
                SCADASetpointService scadaReceivingService = new SCADASetpointService();

                scadaSubscriberService.Start();
                scadaReceivingService.Start();

                NMSSubscriber NMSProxy = new NMSSubscriber(SCADAModel.Instance);
                NMSProxy.Subscribed(new List<DMSType> { DMSType.ANALOGVALUE, DMSType.DISCRETEVALUE });

                Console.WriteLine("Press any key to stop data collecting...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Scada services failed.");
                Console.WriteLine(ex.StackTrace);
                CommonTrace.WriteTrace(CommonTrace.TraceError, ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, "Scada services failed.");
                CommonTrace.WriteTrace(CommonTrace.TraceError, ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}