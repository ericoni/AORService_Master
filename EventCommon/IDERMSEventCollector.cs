using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
	/// <summary>
	/// Method for event information collecting.
	/// </summary>
	[ServiceContract]
	public interface IDERMSEventCollector
	{
		[OperationContract]
		void SendEvent(Event e);
	}
}
