using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.CalculationEngine.Model;

namespace FTN.Common.SCADA.Services
{
	[ServiceContract]
	public interface ISCADAReceiving
	{
		[OperationContract]
		void ReceiveAndSetSetpoint(Setpoint setpoint);
	}
}
