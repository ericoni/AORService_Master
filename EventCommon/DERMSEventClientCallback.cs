using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
    /// <summary>
    /// Implements <seealso cref="IDERMSEventSubscriptionCallback"/> interface.
    /// </summary>
	public class DERMSEventClientCallback : IDERMSEventSubscriptionCallback
	{
		public void ReceiveEvent(string message)
		{
		}
	}
}
	