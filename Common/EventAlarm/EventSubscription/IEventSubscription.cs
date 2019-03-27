using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm.EventSubscription
{
	[ServiceContract(CallbackContract = typeof(IEventSubscriptionCallback))]
	public interface IEventSubscription
	{
        [OperationContract(IsOneWay = true)]
        void SubscribeToAORAreas(HashSet<long> areas);
		[OperationContract]
		void UnsubscribeFromAORAreas();
	}
}
