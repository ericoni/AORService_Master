using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm
{
	/// <summary>
	/// Duplex communication to UI and manipulating events and alarms.
	/// </summary>
	[ServiceContract(CallbackContract = typeof(IEventDistributionCallback))]
	public interface IEventDistribution
	{
        [OperationContract(IsOneWay = true)]
        void TestEventDistribution();
	}
}
