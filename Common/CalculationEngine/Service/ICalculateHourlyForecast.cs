using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Services.NetworkModelService.DataModel.Core;
using CalculationEngine;
using CommonCE;
using FTN.Common.WeatherForecast.Model;

namespace FTN.Common.CalculationEngine.Service
{
    [ServiceContract]
    public interface ICalculateHourlyForecast
    {
        [OperationContract]
        ForecastObject CalculateHourlyForecastForDer(List<Data> hourlyDerData, SynchronousMachine der,  long sunriseTime, long sunsetTime, AnalogValue modelDataActive, AnalogValue modelDataReactive);
    }
}
