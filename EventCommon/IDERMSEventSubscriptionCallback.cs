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
		[OperationContract(IsOneWay = true)] // IsOneWay valjda govori da li je neblokirajuci poziv
		void ReceiveEvent(string message);
	}
}
