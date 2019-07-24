using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
	/// <summary>
	/// Implements <seealso cref="IDERMSEventSubscriptionCallback"/> interface. Uspio sam da ga pogodim 24.7. posle fixa(prazna lista subscribera) 
	/// </summary>
	public class DERMSEventClientCallback : IDERMSEventSubscriptionCallback
	{
		public void ReceiveEvent(Event e)
		{
		}
	}
}
	