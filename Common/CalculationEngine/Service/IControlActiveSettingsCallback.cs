using FTN.Common.CE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.CE.Services
{
	/// <summary>
	/// Callback for <seealso cref="IControlActiveSettingsSubscriber"/>
	/// </summary>
	[ServiceContract]
	public interface IControlActiveSettingsCallback
	{
		[OperationContract]
		void ControlActiveSettings(List<CAS> signals);
	}
}
