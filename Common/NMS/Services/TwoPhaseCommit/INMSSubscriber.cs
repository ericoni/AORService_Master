
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace FTN.Common.Services
{
	/// <summary>
	/// Has callback <seealso cref="ITwoPhaseCommit"/>.
	/// </summary>
	[ServiceContract(CallbackContract = typeof(ITwoPhaseCommit))]
	public interface INMSSubscriber
	{
		[OperationContract]
		void Subscribed(List<DMSType> topics);
		[OperationContract]
		void Unsubscribed(List<DMSType> topics);
	}
}
