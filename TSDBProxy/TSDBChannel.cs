using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Meas;
using TSDB.Access;
using System.Threading;

namespace TSDBProxyNS
{
	public class TSDBChannel : ClientBase<IHistorianDB>, IHistorianDB
	{
		public TSDBChannel() : base("TSDBEndpoint")
		{
		}

		public List<AnalogValue> GetAnnualConsuption(long date, PowerType powerType, long gid = -1)
		{
			return this.Channel.GetAnnualConsuption(date, powerType, gid);
		}

		public List<AnalogValue> GetDailyConsuption(long date, PowerType powerType, long gid = -1)
		{
			return this.Channel.GetDailyConsuption(date, powerType, gid);
		}

		public List<AnalogValue> GetHourlyConsuption(long date, PowerType powerType, long gid = -1)
		{
			return this.Channel.GetHourlyConsuption(date, powerType, gid);
		}

		public List<AnalogValue> GetMonthlyConsuption(long date, PowerType powerType, long gid = -1)
		{
			return this.Channel.GetMonthlyConsuption(date, powerType, gid);
		}

		public bool WriteAnalogValue(List<AnalogValue> values)
		{
			return this.Channel.WriteAnalogValue(values);
		}
	}
}
