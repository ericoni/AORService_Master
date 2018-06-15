using FTN.Common.SCADA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Common.CalculationEngine.Model;
using FTN.Common.RawConverter;
using MODBUSLibrary;
using FTN.Common.SCADA;
using FTN.Common.Logger;

namespace SCADAService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class SCADASetpoint : ISCADAReceiving // stari scadaReceiving
	{
		/// <summary>
		/// Singleton instanca
		/// </summary>
		private static volatile SCADASetpoint instance;

		private ModbusTcpClient client;
		private SCADAModel scadaModel;
		private const float EGU_MIN = -200;
		private const float EGU_MAX = 1500;

		private const float RAW_MIN = 0;
		private const float RAW_MAX = 4095;

		/// <summary>
		/// Objekat za lock
		/// </summary>
		private static object syncRoot = new Object();

		public SCADASetpoint()
		{
			scadaModel = SCADAModel.Instance;
			client = new ModbusTcpClient();
			client.Connect();
		}

		/// <summary>
		/// Metoda za Singleton
		/// </summary>
		public static SCADASetpoint Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new SCADASetpoint();
					}
				}

				return instance;
			}
		}

		public void ReceiveAndSetSetpoint(Setpoint setpoint)
		{
			foreach (var item in setpoint.PDistributionByAV)
			{
				AnalogValue av = scadaModel.AnalogValues.Where(o => o.GlobalId.Equals(item.Key)).FirstOrDefault();
				float value = RawValuesConverter.ConvertRange(item.Value, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX);
                //short value = floatToShort(RawValuesConverter.ConvertRange(item.Value, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX));
                //client.WriteSingleHoldingRegister(av.Address, value);
                LogHelper.Log(LogTarget.File, LogService.SCADASetpoint, " INFO - SCADASetpoint.cs - Receiving and setting setpoint.");
				client.WriteSingleHoldingRegister2(av.Address, value);
			}
		}
	}
}
