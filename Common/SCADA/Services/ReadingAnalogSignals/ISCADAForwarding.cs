using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.SCADA.Services
{
	[ServiceContract]
	public interface ISCADAForwarding
	{
		[OperationContract]
		void GetAnalogScadaData(List<AnalogValue> values);
	}
}
