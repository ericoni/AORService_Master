using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	public class AOREventAlarmProxy // TODO dodati i za alarme
	{
		private AOREventAlarmChannel proxy;
		private const int maxTries = 10;
		private const int sleepTime = 3000;

		public AOREventAlarmProxy()
		{
			OpenChannel();
		}

		private void OpenChannel()
		{
			int tryCounter = 0;

			while (true)
			{
				try
				{
					proxy = new AOREventAlarmChannel();
					proxy.Open();
					break;
				}
				catch (Exception e)
				{
					tryCounter++;

					if (tryCounter == maxTries)
						throw e;

					Thread.Sleep(sleepTime);
					Trace.Write("Exception trace :" + e.StackTrace);
				}
			}
		}

		public AOREventAlarmChannel Proxy
		{
			get
			{
				if (proxy.State != System.ServiceModel.CommunicationState.Opened)
				{
					OpenChannel();
				}

				return proxy;
			}
		}
	}
}
