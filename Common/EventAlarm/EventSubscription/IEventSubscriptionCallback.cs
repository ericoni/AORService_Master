using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm.EventSubscription
{
	[ServiceContract]
	public interface IEventSubscriptionCallback
	{
		[OperationContract(IsOneWay = true)]
		void NotifyUser();
	}
}
