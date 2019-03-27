using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.SCADA.Services;
using FTN.Common.CalculationEngine.Model;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Services.NetworkModelService.DataModel.Meas;
using SmartCacheLibrary;

using Newtonsoft.Json;
using FTN.Common.SCADA;
using TSDBProxyNS;
using FTN.Common.Logger;
using System.Diagnostics;

namespace CalculationEngService
{
	/// <summary>
	/// Implementing <see cref="ISCADAForwarding"/> callback interface.
	/// </summary>
    public class SCADADataColector : ISCADAForwarding
    {
        TSDBProxy proxyTSDB = null;

        CalculationEngineModel model = CalculationEngineModel.Instance;

        public SCADADataColector()
        {
            proxyTSDB = new TSDBProxy();
        }

		public void GetAnalogScadaData(List<AnalogValue> values)
		{
			if (values == null)
			{
				Trace.WriteLine("NULL GetAnalogScadaData method received empty values!");
				return;
			}

			Console.WriteLine("\n****");

			List<object> measurements = new List<object>();

			foreach (AnalogValue item in values)
			{
				// Koliko komanda uvecava vrednost
				float commandValue = 0;

				// Lista komandi za analogni signal
				List<Command> commands = new List<Command>();

				// Pribavljanje liste
				model.AppliedCommands.TryGetValue(item.GlobalId, out commands);

				// DER
				SynchronousMachine sm = model.DersOriginal.Where(o => o.GlobalId.Equals(item.SynchronousMachine)).FirstOrDefault();

				if (sm == null)
				{
					LogHelper.Log(LogTarget.File, LogService.CEDataCollector, " WARNING - SCADADataCollector.cs - Synchronous machine with " + item.SynchronousMachine + " global id does not exist.");
					continue;
				}

				if (commands != null)
				{
					Command command = commands.Where(o => o.StartTime <= DateTime.Now.Ticks &&
														  o.EndTime > DateTime.Now.Ticks).FirstOrDefault();

					if (command != null)
					{
						commandValue = command.DemandedPower;
					}
				}

				if (item.PowerType == FTN.Common.PowerType.Active)
				{
					item.PowIncrease = item.Value + sm.DERFlexibilityP - commandValue;
					item.PowDecrease = item.Value - sm.DERFlexibilityP - commandValue;

					if (item.PowIncrease > sm.MaxP)
					{
						item.PowIncrease = sm.MaxP;
					}

					if (item.PowDecrease < sm.MinP)
					{
						item.PowDecrease = sm.MinP;
					}
				}
				else
				{
					item.PowIncrease = item.Value + sm.DERFlexibilityQ - commandValue;
					item.PowDecrease = item.Value - sm.DERFlexibilityQ - commandValue;

					if (item.PowIncrease > sm.MaxQ)
					{
						item.PowIncrease = sm.MaxQ;
					}

					if (item.PowDecrease < sm.MinQ)
					{
						item.PowDecrease = sm.MinQ;
					}

					//item.Value += commandValue;
				}

				measurements.Add(item);
			}

			//Sent to TSDB
			proxyTSDB.Proxy.WriteAnalogValue(values);

			//when data from SCADA is received, publish it to all clients
			SmartCache.Instance.Cache.CacheList.Clear();
			SmartCache.Instance.Cache.CacheList.Add(new CacheObject(DateTime.Now, measurements));
			SmartCache.Instance.ReceiveCache("", null);
		}
	}
}
