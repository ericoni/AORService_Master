using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace FTN.ServiceContracts
{
	[ServiceContract]
	public interface IScadaGDAContract
	{
		[OperationContract]
		List<AnalogValue> GetAnalogValues();
	}
}
