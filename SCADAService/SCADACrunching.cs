using Adapter;
using FTN.Common;
using FTN.Common.Logger;
using FTN.Common.RawConverter;
using FTN.Common.SCADA;
using FTN.Services.NetworkModelService.DataModel.Wires;
using MODBUSLibrary;
using SCADA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCADAService
{
	public class SCADACrunching // stari scadaService
	{
		/// <summary>
		/// Adapter for GDA queries
		/// </summary>
		private RDAdapter adapter;
		private ModbusTcpClient client;
		private DateTime timestamp;
		private SCADAModel scadaModel;
		private const float EGU_MIN = -200;
		private const float EGU_MAX = 1500;

		private const float RAW_MIN = 0;
		private const float RAW_MAX = 4095;

		private static object syncRoot = new Object();
		private static object obj = new object();

		public SCADACrunching()
		{
			scadaModel = SCADAModel.Instance;

			timestamp = new DateTime();
			adapter = new RDAdapter();
			//apsClient = new APSClient(APSCommon.MessageType.AnalogValue_List);

			client = new ModbusTcpClient();
			client.Connect();

			Thread taskA = new Thread(() => DataAcquisition());

			taskA.Start();
		}

		private void DataAcquisition()
		{
			while (true)
			{
				LogHelper.Log(LogTarget.File, LogService.SCADACrunching, " INFO - SCADACrunching.cs - Data acquisition, new measurement ciclus.");
				timestamp = DateTime.Now;

				lock (SCADAModel.Instance.Lock2PC)
				{
					foreach (var analogValue in scadaModel.AnalogValues)
					{
						var value = client.ReadHoldingRegisters2(analogValue.Address, 1);

						SynchronousMachine syncMachine = adapter.GetSyncMachineByGid(analogValue.SynchronousMachine);

						if (syncMachine == null)
						{
							LogHelper.Log(LogTarget.File, LogService.SCADACrunching, " ERROR - SCADACrunching.cs - Data acquisition, Synchronous machine is not assigned to analog value.");
							throw new Exception("Synchronous machine is not assigned to analog value.");
						}

						analogValue.Value = RawValuesConverter.ConvertRange(value[0], RAW_MIN, RAW_MAX, EGU_MIN, EGU_MAX);

						analogValue.Timestamp = timestamp.Ticks;
					}

					SCADASubscriber.Instance.NotifySubscribers(DMSType.ANALOGVALUE);

				}
					 
				Thread.Sleep(3000);
			}
		}
	}
}
