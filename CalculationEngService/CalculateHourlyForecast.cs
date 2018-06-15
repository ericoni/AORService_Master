using FTN.Common.CalculationEngine.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CommonCE;
using FTN.Common.WeatherForecast.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Common;
using PowerValuesSimulator;
using FTN.Common.SCADA;
using System.Fabric;

namespace CalculateHourlyForecastService
{

    public class CalculateHourlyForecast : ICalculateHourlyForecast
    {
        private PowerCalculator powCalc = new PowerCalculator();

        public CalculateHourlyForecast()
        {

        }

        public ForecastObject CalculateHourlyForecastForDer(List<Data> hourlyDerData, SynchronousMachine der, long sunriseTime, long sunsetTime, AnalogValue modelDataActive, AnalogValue modelDataReactive)
        {
            ForecastObject forecastObj = new ForecastObject();

            forecastObj.DerGID = der.GlobalId;

            //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Calculate hourly forecast for one der.");

            if (modelDataActive == null)
            {
                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " ERROR - CalculateHourlyForecast.cs - Model data active object is null.");
                return null;
            }

            foreach (Data derHour in hourlyDerData)
            {
                switch (der.FuelType)
                {
                    case FuelType.Sun:
                        {
                            //get ders substation so you can get latitude and longitude
                            //need this to calculate sunrise and sunset time for calculation of active power for solar 
                            float activePowSolar = powCalc.GetActivePowerForSolarGenerator((float)derHour.Temperature, (float)derHour.CloudCover, der.NominalP, sunriseTime, sunsetTime);
                            float powIncrease = 0;
                            float powDecrease = 0;
                            float powIncreaseTemp = activePowSolar + der.DERFlexibilityP;
                            float powDecreaseTemp = activePowSolar - der.DERFlexibilityP;

                            if (powIncreaseTemp >= der.MaxP)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Solar panel: "+der.GlobalId+" active power increase value change is higher or equal to max active power. New active power increase value is set to max active power value.");
                                powIncrease = der.MaxP;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Solar panel: " + der.GlobalId + " active power increase value change is lower then max active power. New active power increase value is set regularly.");
                                powIncrease = powIncreaseTemp;
                            }

                            if (powDecreaseTemp <= der.MinP)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Solar panel: " + der.GlobalId + " active power decrease value change is lower or equal to min active power. New active power decrease value is set to min active power value.");
                                powDecrease = der.MinP;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Solar panel: " + der.GlobalId + " active power decrease value change is higher then min active power. New active power decrease value is set regularly.");
                                powDecrease = powDecreaseTemp;
                            }

                            forecastObj.HourlyP.Add(new AnalogValue(activePowSolar, derHour.Time, modelDataActive.GlobalId, modelDataActive.SynchronousMachine,
                                PowerType.Active, powIncrease, powDecrease));
                        }

                        break;
                    case FuelType.Wind:
                        {
                            float activePowWind = powCalc.GetActivePowerForWindGenerator(der.NominalP, (float)derHour.WindSpeed);
                            float reactivePowWind = activePowWind * 0.05f;
                            float powIncreaseActive = 0;
                            float powDecreaseActive = 0;
                            float powIncreaseActiveTemp = activePowWind + der.DERFlexibilityP;
                            float powDecreaseActiveTemp = activePowWind - der.DERFlexibilityP;
                            float powIncreaseReactive = 0;
                            float powDecreaseReactive = 0;
                            float powIncreaseReactiveTemp = reactivePowWind + der.DERFlexibilityQ;
                            float powDecreaseReactiveTemp = reactivePowWind - der.DERFlexibilityQ;

                            //Check for active power limit
                            if (powIncreaseActiveTemp >= der.MaxP)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " active power increase value change is higher or equal to max active power. New active power increase value is set to max active power value.");
                                powIncreaseActive = der.MaxP;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " active power increase value change is lower then max active power. New active power increase value is set regularly.");
                                powIncreaseActive = powIncreaseActiveTemp;
                            }

                            if (powDecreaseActiveTemp <= der.MinP)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " active power decrease value change is lower or equal to min active power. New active power decrease value is set to min active power value.");
                                powDecreaseActive = der.MinP;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " active power decrease value change is higher then min active power. New active power decrease value is set regularly.");
                                powDecreaseActive = powDecreaseActiveTemp;
                            }

                            //Check for reactive power limit
                            if (powIncreaseReactiveTemp >= der.MaxQ)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " reactive power increase value change is higher or equal to max reactive power. New reactive power increase value is set to max reactive power value.");
                                powIncreaseReactive = der.MaxQ;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " reactive power increase value change is lower then max reactive power. New reactive power increase value is set regularly.");
                                powIncreaseReactive = powIncreaseReactiveTemp;
                            }

                            if (powDecreaseReactiveTemp <= der.MinQ)
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " WARNING - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " reactive power decrease value change is lower or equal to min reactive power. New reactive power decrease value is set to min reactive power value.");
                                powDecreaseReactive = der.MinQ;
                            }
                            else
                            {
                                //LogHelper.Log(LogTarget.File, LogService.CalculateHourlyForecast, " INFO - CalculateHourlyForecast.cs - Wind turbine: " + der.GlobalId + " reactive power decrease value change is higher then min reactive power. New reactive power decrease value is set regularly.");
                                powDecreaseReactive = powDecreaseReactiveTemp;
                            }
                            forecastObj.HourlyP.Add(new AnalogValue(activePowWind, derHour.Time, modelDataActive.GlobalId, modelDataActive.SynchronousMachine,
                                PowerType.Active, powIncreaseActive, powDecreaseActive));

                            forecastObj.HourlyQ.Add(new AnalogValue(reactivePowWind, derHour.Time, modelDataReactive.GlobalId, modelDataReactive.SynchronousMachine,
                                PowerType.Reactive, powIncreaseReactive, powDecreaseReactive));
                        }

                        break;
                    default:
                        break;
                }

            }

            return forecastObj;
        }
    }
}
