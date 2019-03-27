using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm
{
	public interface IEventDistributionCallback
	{
		[OperationContract]
		void Refresh();
	}
}
