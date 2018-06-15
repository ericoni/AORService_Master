using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TSDB.Access
{
	[ServiceContract]
	public interface IHistorianDB
	{
		/// <summary>
		/// Metoda za upis liste analognih merenja u bazu podataka
		/// </summary>
		/// <param name="values"> Lista analognih merenja</param>
		/// <returns></returns>
		[OperationContract]
		bool WriteAnalogValue(List<AnalogValue> values);

        [OperationContract]
        List<AnalogValue> GetHourlyConsuption(long date, PowerType powerType, long gid = -1);

        [OperationContract]
        List<AnalogValue> GetDailyConsuption(long date, PowerType powerType, long gid = -1);

        [OperationContract]
        List<AnalogValue> GetMonthlyConsuption(long date, PowerType powerType, long gid = -1);

        [OperationContract]
        List<AnalogValue> GetAnnualConsuption(long date, PowerType powerType, long gid = -1);

    }
}
