using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm
{
	[ServiceContract]
	public interface IEventDistributionCallback
	{
		[OperationContract]
		void Refresh();
	}
}
