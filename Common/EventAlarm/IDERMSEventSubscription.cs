using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm
{
	[ServiceContract(CallbackContract = typeof(IDERMSEventSubscriptionCallback))]
	public interface IDERMSEventSubscription
	{
		[OperationContract]
		void Subscribe(List<long> gids);
		[OperationContract]
		void Unsubscribe();
	}
}
