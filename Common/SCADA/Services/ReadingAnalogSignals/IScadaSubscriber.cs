using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.SCADA.Services
{
	/// <summary>
	/// Service contract with <seealso cref="ISCADAForwarding"></seealso> callback./>
	/// </summary>
	[ServiceContract(CallbackContract = typeof(ISCADAForwarding))]
	public interface ISCADASubscriber
	{
		[OperationContract]
		void Subscribed(List<DMSType> topics);
		[OperationContract]
		void Unsubscribed(List<DMSType> topics);
	}
}
