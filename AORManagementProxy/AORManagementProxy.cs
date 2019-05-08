using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	/// <summary>
	/// Logivanje korisnika i AOR management prozor.
	/// </summary>
	public class AORManagementProxy
	{
		private const int maxTries = 5;
		private const int sleepTime = 3000;
		private AORManagementChannel proxy;

		public AORManagementProxy()
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
					proxy = new AORManagementChannel();
					proxy.Open();
					break;
				}
				catch (Exception e)
				{
					tryCounter++;

					if (tryCounter == maxTries)
						throw e;

					Thread.Sleep(sleepTime);
					Trace.Write("Exception trace is:" + e.StackTrace);
				}
			}
		}

		public AORManagementChannel Proxy
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
