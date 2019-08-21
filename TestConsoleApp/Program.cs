using ActiveAORCache.Helpers;
using Adapter;
using AORCommon.Principal;
using AORManagementProxyNS;
using EventAlarmService;
using EventCollectorProxyNS;
using EventCommon;
using FTN.Common.AORCachedModel;
using FTN.Common.AORHelpers;
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
			//DERMSEventClientCallback callback = new DERMSEventClientCallback();
			//IDERMSEventSubscription proxy = null;

			//DuplexChannelFactory<IDERMSEventSubscription> factory = new DuplexChannelFactory<IDERMSEventSubscription>(
			//	new InstanceContext(callback),
			//	new NetTcpBinding(),
			//	new EndpointAddress("net.tcp://localhost:10047/IDERMSEvent"));
			//proxy = factory.CreateChannel();

			//proxy.Subscribe(new List<long>(1) { 7 });//hardcoded subscribe

			//AORManagementProxy aorManagementProxy = new AORManagementProxy();// prvo subscribe pa posle login
			//var areas = aorManagementProxy.Proxy.Login("state", "a");

			//bool isSelected = aorManagementProxy.Proxy.SelectAreaForView("West-Area", true);
			//bool isSelected = aorManagementProxy.Proxy.SelectAreaForControl("West-Area", true);

			//var areas = aorManagementProxy.Proxy.Login("state", "a");

			RDAdapter rdAdapter = new RDAdapter();
			AORCachedGroup aorGroup = null;
			List<AORCachedGroup> aorGroups = new List<AORCachedGroup>(20);
			HashSet<AORCachedSyncMachine> aorSMachinesHash = new HashSet<AORCachedSyncMachine>();
			List<string> smNames = new List<string>(20);
			var nmsAorGroups = rdAdapter.GetAORGroups();

			if (nmsAorGroups == null)
			{
				return;
			}

			foreach (var nmsGroup in nmsAorGroups)
			{
				var syncMachines = rdAdapter.GetSyncMachinesForAreaGroupGid(new List<long>() { nmsGroup.GlobalId });

				var aorSyncMachines = NMSModelAORConverter.ConvertSyncMachinesFromNMS(syncMachines);

				foreach (var sm in aorSyncMachines)
				{
					aorSMachinesHash.Add(sm);
				}

				aorGroup = NMSModelAORConverter.ConvertAORGroupFromNMS(nmsGroup);
				aorGroup.SynchronousMachines = aorSyncMachines;
				aorGroups.Add(aorGroup);
			}

			//var perms = AORCacheConfigurations.GetPermissionsForUser("admin");
			//var p = Thread.CurrentPrincipal as IMyPrincipal;
			//var p2 = Thread.CurrentPrincipal;

			//aorManagementProxy.Proxy.Test(null);

			Console.WriteLine("Prosao main..");
			Console.Read();
		}

		static void SendEvent()
		{
			Event e = new Event("a", "New command for ", "regionXX", DateTime.Now);
			EventCollectorProxy eventCollectorProxy = new EventCollectorProxy(); //to do izbaciti ga posle u konstruktor
			eventCollectorProxy.Proxy.SendEvent(e);
			//DERMSEventSubscription.Instance.NotifyClients("a", e);//ne koristiti ga ovako, nego uvijek pucamo notifikaciju na event-e
		}

		private void ClientRegularChannelTest()
		{
			//ChannelFactory<IDERMSEventCollector> factory2 = new ChannelFactory<IDERMSEventCollector>(
			//  new NetTcpBinding(),
			//  new EndpointAddress("net.tcp://localhost:10048/IDERMSEventCollector"));
			//IDERMSEventCollector kanal = factory2.CreateChannel();

			//Thread.Sleep(3000);
			//kanal.SendEvent(new Event { Details = "Nesto" });
		}

		static void ClientDuplexChannelTest()
		{
			DERMSEventClientCallback callback = new DERMSEventClientCallback();
			IDERMSEventSubscription proxy = null;

			DuplexChannelFactory<IDERMSEventSubscription> factory = new DuplexChannelFactory<IDERMSEventSubscription>(
				new InstanceContext(callback),
				new NetTcpBinding(),
				new EndpointAddress("net.tcp://localhost:10047/IDERMSEvent"));
			proxy = factory.CreateChannel();

			//proxy.Subscribe(new List<long>(1) { 7 });
		}

		private void SendEventTest()
		{
			EventCollectorProxy eventProxy = new EventCollectorProxy();
			eventProxy.Proxy.SendEvent(new Event { Details = "Nesto" });
		}

		private void RandomTesting()
		{
			//var a = AORCacheConfigurations.GetAORAreasForUsername("marko.markovic");

			// AORCacheConfigurations.SelectAreaForView("West-Area", false);

			///var c = AORCacheConfigurations.GetPermissionsForArea("West-Area"); 

			//AORManagementProxy aorManagementProxy = new AORManagementProxy();
			//var areas = aorManagementProxy.Proxy.Login("testUsername", "a");
			//aorManagementProxy.Proxy.Test();	

			//RDAdapter rdAdapter = new RDAdapter();
			//var a = rdAdapter.GetSyncMachinesByGids(new List<long>() { 12884901889 });
		}
	}
}
