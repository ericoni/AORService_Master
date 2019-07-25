using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
	/// <summary>
	/// Callback interface for <seealso cref="IDERMSEventSubscription"/>. Important note: there's no exception handling if client is closed.
	/// Need to take care of that later.
	/// </summary>
	[ServiceContract]
	public interface IDERMSEventSubscriptionCallback
	{
        /// <summary>
        /// IsOneWay valjda govori da li je neblokirajuci poziv.
        /// Enable the service to call back even when the concurrency mode is set to single-threaded,
        /// because there will not be any reply message to contend for the lock.
        /// WCF biblija : event-handling operations should have a void return type, should not have any outgoing parameters,
        /// and should be marked as one-way.
        /// </summary>
        [OperationContract(IsOneWay = true)] 
		void ReceiveEvent(Event e);
	}
}
