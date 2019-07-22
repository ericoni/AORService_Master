﻿using ActiveAORCache.Helpers;
using Adapter;
using AORCommon.Principal;
using AORManagementProxyNS;
using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args) //ima neka fora, on kreira svoju NOVU bazu "UsersDatabase7xxxxx" umjesto da koristi onu postojecu.
        {
            //var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");

            // AORCacheConfigurations.SelectAreaForView("West-Area", false);

            ///var c = AORCacheConfigurations.GetPermissionsForArea("West-Area"); 

            //AORManagementProxy aorManagementProxy = new AORManagementProxy();
            //var areas = aorManagementProxy.Proxy.Login("testUsername", "a");
            //aorManagementProxy.Proxy.Test();	

            //RDAdapter rdAdapter = new RDAdapter();
            //var a = rdAdapter.GetSyncMachinesByGids(new List<long>() { 12884901889 });

            DERMSEventClientCallback callback = new DERMSEventClientCallback();
            IDERMSEventSubscription proxy = null;

            IDERMSEventCollector proxyEventCol = null;

            DuplexChannelFactory<IDERMSEventSubscription> factory = new DuplexChannelFactory<IDERMSEventSubscription>(
              new InstanceContext(callback),
              new NetTcpBinding(),
              new EndpointAddress("net.tcp://localhost:10047/IDERMSEvent"));
            proxy = factory.CreateChannel();

            proxy.Subscribe(new List<long>(1) { 7 });

            ChannelFactory<IDERMSEventCollector> factory2 = new ChannelFactory<IDERMSEventCollector>(
              new NetTcpBinding(),
              new EndpointAddress("net.tcp://localhost:10048/IDERMSEventCollector"));
            IDERMSEventCollector kanal = factory2.CreateChannel();

            Thread.Sleep(3000);
            kanal.SendEvent(new Event { Details = "None" });

            Console.Read();
        }
    }
}
