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

namespace CommonCE
{
    [ServiceContract]
	public interface IForecastService
	{
		[OperationContract]
		ForecastObject HourlyForecastForDer(long derGID);

		[OperationContract]
		ForecastObject CalculateHourlyForecastForGroup(long groupGID);
	}
}
