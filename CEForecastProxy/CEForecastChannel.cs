using CalculationEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CalculationEngine;
using FTN.Services.NetworkModelService.DataModel.Wires;
using CommonCE;

namespace CEForecastProxy
{
    public class CEForecastChannel : ClientBase<IForecastService>, IForecastService
    {
        public CEForecastChannel()
			: base("CEForecastEndpoint")
		{
        }

        public ForecastObject CalculateHourlyForecastForGroup(long groupGID)
        {
            return this.Channel.CalculateHourlyForecastForGroup(groupGID);
        }

		public ForecastObject HourlyForecastForDer(long derGID)
		{
			return this.Channel.HourlyForecastForDer(derGID);
		}
    }
}
