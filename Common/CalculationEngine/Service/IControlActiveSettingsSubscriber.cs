using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.CE.Services
{
	[ServiceContract(CallbackContract = typeof(IControlActiveSettingsCallback))]
	public interface IControlActiveSettingsSubscriber
	{
		[OperationContract]
		void Subscribed();
		[OperationContract]
		void Unsubscribed();
	}
}
