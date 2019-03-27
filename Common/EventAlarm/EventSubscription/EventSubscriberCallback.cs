using FTN.Common.EventAlarm.EventSubscription;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.EventAlarm.EventSubscription
{
	/// <summary>
	/// Callback class for <seealso cref="IEventSubscription"/> interface.
	/// </summary>
	//[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class EventSubscriberCallback : IEventSubscriptionCallback
	{
		public EventSubscriberCallback()
		{
		}

		public void NotifyUser()
		{
			int aaa = 55555;
			Trace.Write("zola zola zola NotifyUser has been invoked!");
		}
	}
}
