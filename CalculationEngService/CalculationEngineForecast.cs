using Adapter;
using CalculateHourlyForecastService;
using CommonCE;
using FTN.Common;
using FTN.Common.CalculationEngine.Model;
using FTN.Common.Logger;
using FTN.Common.SCADA;
using FTN.Common.WeatherForecast.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using PowerValuesSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeatherForecastProxyNS;

namespace CalculationEngService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class CalculationEngineForecast : IForecastService
	{
		/// <summary>
		/// Calculation engine model
		/// </summary>
		private CalculationEngineModel model;
		
		/// <summary>
		/// Power calculator
		/// </summary>
		private PowerCalculator powCalc;

		/// <summary>
		/// Weather forecast proxy
		/// </summary>
		private WeatherForecastProxy wfProxy = null;

		/// <summary>
		/// Proxy for CalculateHorulyForecastClient
		/// </summary>
		private CalculateHourlyForecast chfProxy = null;

		/// <summary>
		/// Function to get random number
		/// </summary>
		private static readonly Random random = new Random();

		/// <summary>
		/// Lock object
		/// </summary>
		private static readonly object syncLock = new object();

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static volatile CalculationEngineForecast instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Adapter for NMS data
		/// </summary>
		private RDAdapter adapter = new RDAdapter();

		private static object lockObj = new object();

		/// <summary>
		/// Constructor 
		/// </summary>
		public CalculationEngineForecast()
		{
			wfProxy = new WeatherForecastProxy();
			chfProxy = new CalculateHourlyForecast();
			model = CalculationEngineModel.Instance;
			powCalc = new PowerCalculator();
			Thread newThread = new Thread(CalculatePerHour);
			newThread.Start();
		}

		/// <summary>
		/// Singleton method
		/// </summary>
		public static CalculationEngineForecast Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new CalculationEngineForecast();
					}
				}

				return instance;
			}
		}

		public void CalculatePerHour()
		{
			//while (true) //vrati se
			//{
			//    lock (CalculationEngineModel.Instance.Lock2PC)
			//    {
			//        CalculateForecastForDers();
			//    }

			//    CalculationEngineModel.Instance.TwoPhaseCommitSemaptore.WaitOne(1000 * 60 * 60 * 2);
			//}
		}

		/// <summary>
		/// Calculate forecast for all ders in model
		/// </summary>
		public void CalculateForecastForDers()
		{
			model.ForecastListOfDers.Clear();
			LogHelper.Log(LogTarget.File, LogService.CalculationEngineForecast, " INFO - CalculationEngineForecast.cs - Calculate forecast for all ders in model.");

			List<Task<ForecastObject>> tasks = new List<Task<ForecastObject>>();

			foreach (SynchronousMachine der in model.DersOriginal)
			{
				Task<ForecastObject> task = new Task<ForecastObject>(() =>
			   {
				   WeatherInfo wi = wfProxy.Proxy.Get7DayPerHourForecastByGid(der.GlobalId);
				   List<Data> hourlyData = wi.Hourly.Data;
				   long sunriseTime = wi.Daily.Data.FirstOrDefault().SunriseTime;
				   long sunsetTime = wi.Daily.Data.FirstOrDefault().SunsetTime;
				   ForecastObject forecastObj = null;

				   AnalogValue modelDataActive = CalculationEngineModel.Instance.AnalogPointsOriginal.Where(o => o.SynchronousMachine.Equals(der.GlobalId) &&
																												 o.PowerType.Equals(PowerType.Active)).FirstOrDefault();
				   AnalogValue modelDataReactive = CalculationEngineModel.Instance.AnalogPointsOriginal.Where(o => o.SynchronousMachine.Equals(der.GlobalId) &&
																												   o.PowerType.Equals(PowerType.Reactive)).FirstOrDefault();

				   chfProxy = new CalculateHourlyForecast();
				   forecastObj = chfProxy.CalculateHourlyForecastForDer(hourlyData, der, sunriseTime, sunsetTime, modelDataActive, modelDataReactive);


				   return forecastObj;
			   });

				tasks.Add(task);
			}

			foreach(Task<ForecastObject> task in tasks)
			{
				task.Start();
			}

			foreach(Task<ForecastObject> task in tasks)
			{
				task.Wait();

				ForecastObject forecastObj = task.Result;

				if (forecastObj != null)
				{
					model.ForecastListOfDers.Add(forecastObj);
				}
			}
		}

		/// <summary>
		/// Calculates forecast for certain der object
		/// </summary>
		/// <param name="der"></param>
		/// <returns>Forecast object for certain der</returns>
		public ForecastObject HourlyForecastForDer(long derGID)
		{
			LogHelper.Log(LogTarget.File, LogService.CalculationEngineForecast, " INFO - CalculationEngineForecast.cs - Get hourly forecast for: "+derGID);
			SynchronousMachine der = model.DersOriginal.Where(o => o.GlobalId.Equals(derGID)).FirstOrDefault();
			ForecastObject retValue = new ForecastObject();

			lock (lockObj)
			{
				ForecastObject forecast = model.ForecastListOfDers.Where(o => o.DerGID.Equals(der.GlobalId)).FirstOrDefault();

				DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
				DateTime end = start.AddDays(1);
				start = start.AddHours(DateTime.Now.Hour);

				retValue.DerGID = forecast.DerGID;
				retValue.HourlyP = forecast.HourlyP.Where(o => o.Timestamp >= start.Ticks && o.Timestamp <= end.Ticks).ToList();
				retValue.HourlyQ = forecast.HourlyQ.Where(o => o.Timestamp >= start.Ticks && o.Timestamp <= end.Ticks).ToList();

				retValue.HourlyP = CopyList(retValue.HourlyP);
				retValue.HourlyQ = CopyList(retValue.HourlyQ);

				UpdateForecastDataDer(retValue);
			}

			return retValue;
		}


		/// <summary>
		/// Calculates forecast for ders group 
		/// <param name="hourlyData"></param>
		/// <param name="groupGID"></param>
		/// <returns></returns>
		public ForecastObject CalculateHourlyForecastForGroup(long groupGID)
		{
			LogHelper.Log(LogTarget.File, LogService.CalculationEngineForecast, " INFO - CalculationEngineForecast.cs - Get hourly forecast for group: " + groupGID);
			List<ForecastObject> derFObjects = new List<ForecastObject>();
			ForecastObject forecastObj = new ForecastObject();

			DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
			DateTime end = start.AddDays(1);
			start = start.AddHours(DateTime.Now.Hour);

			forecastObj.DerGID = groupGID;

			//Return forecast object for each der in group and add it to list of der forecast objects
			foreach (SynchronousMachine der in adapter.GetDERs(groupGID))
			{
				ForecastObject temp = model.ForecastListOfDers.Where(o => o.DerGID.Equals(der.GlobalId)).FirstOrDefault();

				ForecastObject retValue = new ForecastObject();

				retValue.DerGID = temp.DerGID;
				retValue.HourlyP = temp.HourlyP.Where(o => o.Timestamp >= start.Ticks && o.Timestamp <= end.Ticks).ToList();
				retValue.HourlyQ = temp.HourlyQ.Where(o => o.Timestamp >= start.Ticks && o.Timestamp <= end.Ticks).ToList();

				retValue.HourlyP = CopyList(retValue.HourlyP);
				retValue.HourlyQ = CopyList(retValue.HourlyQ);

				UpdateForecastDataDer(retValue);

				derFObjects.Add(retValue);
			}

		   // UpdateForecastData(derFObjects);

			//fill out lists of forecast object of group
			for (int k = 0; k < derFObjects.FirstOrDefault().HourlyP.Count; k++)
			{
				forecastObj.HourlyP.Add(new AnalogValue(-1));
				forecastObj.HourlyQ.Add(new AnalogValue(-1));
			}

			//Go through each object in list and sum their power values
			foreach (ForecastObject item in derFObjects)
			{
				for (int i = 0; i < item.HourlyP.Count; i++)
				{
					forecastObj.HourlyP[i].Value += item.HourlyP[i].Value;
					forecastObj.HourlyP[i].PowIncrease += item.HourlyP[i].PowIncrease;
					forecastObj.HourlyP[i].PowDecrease += item.HourlyP[i].PowDecrease;

					//Solar der HourlyQ list should be empty, so we need to skip it
					if (item.HourlyQ.Count > 0)
					{
						forecastObj.HourlyQ[i].Value += item.HourlyQ[i].Value;
						forecastObj.HourlyQ[i].PowIncrease += item.HourlyQ[i].PowIncrease;
						forecastObj.HourlyQ[i].PowDecrease += item.HourlyQ[i].PowDecrease;
					}
				}
			}

			//Go through each power value in group forecast object and devide it with count of ders to get average power value
			for (int j = 0; j < forecastObj.HourlyP.Count; j++)
			{ 
				//set time for each element in power lists
				//use first element in der object list beause all object have same time values
				forecastObj.HourlyP[j].Timestamp = derFObjects[0].HourlyP[j].Timestamp;
				forecastObj.HourlyQ[j].Timestamp = derFObjects[0].HourlyP[j].Timestamp;
			}

			return forecastObj;
		}

		public void UpdateForecastData(List<ForecastObject> derFObjects)
		{
			LogHelper.Log(LogTarget.File, LogService.CalculationEngineForecast, " INFO - CalculationEngineForecast.cs - Update forecast data...");
			foreach (var item in derFObjects)
			{
				UpdateForecastDataDer(item);
			}
		}

		private List<AnalogValue> CopyList(List<AnalogValue> mesurement)
		{
			List<AnalogValue> copy = new List<AnalogValue>();

			foreach (AnalogValue av in mesurement)
			{
				AnalogValue copyAv = new AnalogValue(av.GlobalId);
				copyAv.Mrid = av.Mrid;
				copyAv.Name = av.Name;
				copyAv.Description = av.Description;
				copyAv.Address = av.Address;
				copyAv.Value = av.Value;
				copyAv.Timestamp = av.Timestamp;
				copyAv.PowerType = av.PowerType;
				copyAv.SynchronousMachine = av.SynchronousMachine;
				copyAv.PowIncrease = av.PowIncrease;
				copyAv.PowDecrease = av.PowDecrease;

				copy.Add(copyAv);
			}

			return copy;
		}

		public void UpdateForecastDataDer(ForecastObject foDer)
		{
			AnalogValue analogActive = foDer.HourlyP.FirstOrDefault();
			SynchronousMachine der = model.DersOriginal.Where(o => o.GlobalId.Equals(foDer.DerGID)).FirstOrDefault();
			LogHelper.Log(LogTarget.File, LogService.CalculationEngineForecast, " INFO - CalculationEngineForecast.cs - Update forecast data for : " + der.GlobalId);

			if (analogActive != null)
			{
				long analogActiveGId = foDer.HourlyP.FirstOrDefault().GlobalId;

				//get Command list for analog values 
				if (CalculationEngineModel.Instance.AppliedCommands.ContainsKey(analogActiveGId))
				{
					List<Command> comActive = CalculationEngineModel.Instance.AppliedCommands[analogActiveGId];

					if (comActive.Count > 0)
					{
						foreach (var activeAnalogForecastValue in foDer.HourlyP)
						{
							foreach (var activeAnalogCommandValue in comActive)
							{
								//ako je start komande zakacio vreme prognozirane vrednosti, moramo uzeti u obzir taj setpoint
								if ((activeAnalogForecastValue.Timestamp >= activeAnalogCommandValue.StartTime) && (activeAnalogForecastValue.Timestamp < activeAnalogCommandValue.EndTime))
								{
									var newIncrease = activeAnalogForecastValue.Value + der.DERFlexibilityP;
									var newDecrease = activeAnalogForecastValue.Value - der.DERFlexibilityP;
								   
									activeAnalogForecastValue.PowIncrease = newIncrease;
																	 
									activeAnalogForecastValue.PowDecrease = newDecrease;

									activeAnalogForecastValue.Value += activeAnalogCommandValue.DemandedPower;                                   
								}
							}
						}
					}
				}
			}
			AnalogValue analogReactive = foDer.HourlyQ.FirstOrDefault();

			if (analogReactive != null)
			{
				long analogReactiveGId = foDer.HourlyQ[0].GlobalId;
				//get Command list for analog values 
				if (CalculationEngineModel.Instance.AppliedCommands.ContainsKey(analogReactiveGId))
				{
					List<Command> comReactive = CalculationEngineModel.Instance.AppliedCommands[analogReactiveGId];

					//if der is solar, HourlyQ list should be empty
					if (foDer.HourlyQ.Count == 0)
					{
						return;
					}

					if (comReactive.Count > 0)
					{
						foreach (var reactiveAnalogForecastValue in foDer.HourlyQ)
						{
							foreach (var reactiveAnalogCommandValue in comReactive)
							{
								//ako je start komande zakacio vreme prognozirane vrednosti, moramo uzeti u obzir taj setpoint
								if ((reactiveAnalogForecastValue.Timestamp >= reactiveAnalogCommandValue.StartTime) && (reactiveAnalogForecastValue.Timestamp < reactiveAnalogCommandValue.EndTime))
								{
									var newIncreaseReactive = reactiveAnalogForecastValue.Value + der.DERFlexibilityQ;
									var newDecreaseReactive = reactiveAnalogForecastValue.Value - der.DERFlexibilityQ;

									reactiveAnalogForecastValue.PowIncrease = newIncreaseReactive;
																 
									reactiveAnalogForecastValue.PowDecrease = newDecreaseReactive;
									
									reactiveAnalogForecastValue.Value += reactiveAnalogCommandValue.DemandedPower;                                   
								}
							}
						}
					}
				}
			}
		}
	}
}
