using FTN.Common;
using FTN.Common.CalculationEngine.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine.Interfaces
{
	[ServiceContract]
	public interface ICEDistribution
	{
        [OperationContract]
		Setpoint NominalPowerDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues);

        [OperationContract]
        Setpoint AvailableReserveDistribution(long gid, float demandedValue, PowerType powerType, List<AnalogValue> anaValues);

        [OperationContract]
        float GetAvailablePower(long gid, float demandedValue, List<AnalogValue> anaValues, PowerType powerType);

        [OperationContract]
        bool DistributePowerClient(Command command);

        [OperationContract]
        bool CommandExists(long gid, PowerType powerType);

		[OperationContract]
		bool CancelCommand(long gid, PowerType powerType);

        [OperationContract]
        SortedDictionary<long, float> GetApplaiedCommands(long gid, PowerType powerType);

	}
}
