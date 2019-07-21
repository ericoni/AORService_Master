using CASSubscriberProxyNS;
using FTN.Common;
using ModBusSimulator.Callback;
using ModBusSimulatorService.Controller;
using ModBusSimulatorService.Model;
using NMSSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModBusSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Preplata na zeljene topike
            NMSSubscriber NMSProxy = new NMSSubscriber(SimulatorModel.Instance);
            NMSProxy.Subscribed(new List<DMSType> { DMSType.ANALOGVALUE, DMSType.DISCRETEVALUE, DMSType.SYNCMACHINE });

            // Inicijalizacija modela
            SimulatorModel.Instance.Initialization();

            ControlActiveSettingsCallback callback = new ControlActiveSettingsCallback();
            CASSubscriberProxy CASProxy = new CASSubscriberProxy(callback);
            CASProxy.Subscribed();

            // Inicijalizacija kontrolera za simulator
            SimulatorController simulator = new SimulatorController();
            simulator.StartSimulator();

            Console.ReadKey();
        }
    }
}
