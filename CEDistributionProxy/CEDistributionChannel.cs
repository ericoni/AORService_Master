using CalculationEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Common.CalculationEngine.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;

namespace CEDistributionProxy
{
    public class CEDistributionChannel : ClientBase<ICEDistribution>, ICEDistribution
    {
        public CEDistributionChannel()
			: base("CEDistributionEndpoint")
		{
        }

        public Setpoint AvailableReserveDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues)
        {
            return this.Channel.AvailableReserveDistribution(gid, demandedValue, powerType, anaValues);
        }

		public bool CancelCommand(long gid, PowerType powerType)
		{
			return this.Channel.CancelCommand(gid, powerType);
		}

		public bool CommandExists(long gid, PowerType powerType)
        {
            return this.Channel.CommandExists(gid, powerType);
        }

        public bool DistributePowerClient(Command command)
        {
            return this.Channel.DistributePowerClient(command);
        }

        public SortedDictionary<long, float> GetApplaiedCommands(long gid, PowerType powerType)
        {
            return this.Channel.GetApplaiedCommands(gid, powerType);
        }

        public float GetAvailablePower(long gid, float demandedValue, List<AnalogValue> anaValues, PowerType powerType)
        {
            return this.Channel.GetAvailablePower(gid, demandedValue, anaValues, powerType);
        }

        public Setpoint NominalPowerDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues)
        {
            return this.Channel.NominalPowerDistribution(gid, demandedValue, powerType, anaValues);
        }
    }
}
