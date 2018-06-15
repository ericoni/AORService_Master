using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using SmartCacheLibrary;
using FTN.Common;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService.DataModel.Meas;
using TSDBProxyNS;
using NMSSub;
using FTN.Common.Services;
using CalculationEngine.Services;
using SCADASubscriberProxyNS;
using CalculationEngService;
using SmartCacheLibrary.Services;

namespace CalculationEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // Preplata na zeljene topike
            NMSSubscriber NMSProxy = new NMSSubscriber(CalculationEngineModel.Instance);
            NMSProxy.Subscribed(new List<DMSType> { DMSType.ANALOGVALUE, DMSType.DISCRETEVALUE, DMSType.SYNCMACHINE });

            // Inicijalizacija modela
            CalculationEngineModel.Instance.Initialization();

            try
            {

                //UpdateCacheService/////////////////////////////////////
                string message = "Starting UpdateCacheService...\nStarting CAS Subscribe service...\n";
                Console.WriteLine("\n{0}\n", message);
                Console.WriteLine("************************************************************************Starting services", message);

                UpdateCacheService sms = new UpdateCacheService();
                sms.Start();

                string message3 = "Starting CalculationEngineForecastService...\n";
                Console.WriteLine("\n{0}\n", message3);
                Console.WriteLine("************************************************************************Starting services", message3);
                //In constructor of CalculationEngineForecast we make Thread that calculates Forecast for each der in model and saves it to forecast object list in model
                CalculationEngineForecastService cefs = new CalculationEngineForecastService();
                cefs.Start();

                CASSubscriberService css = new CASSubscriberService();
                css.Start();

                string message2 = "Connecting to SCADAService...\n";
                Console.WriteLine("\n{0}\n", message2);
                Console.WriteLine("************************************************************************Connecting services", message2);

                string message4 = "Starting CalculationEngineDistributionService...\n";
                Console.WriteLine("\n{0}\n", message4);
                Console.WriteLine("************************************************************************Starting services", message4);
                CEDistributionService ceDs = new CEDistributionService();
                ceDs.Start();

                SCADADataColector colector = new SCADADataColector();
                SCADASubscriberProxy scadaProxy = new SCADASubscriberProxy(colector);
                scadaProxy.Subscribed(new List<DMSType> { DMSType.ANALOGVALUE });
                message = "Press <Enter> to stop the service.";

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("UpdateCacheService failed.");
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
