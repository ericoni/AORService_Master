using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagement
{
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
				if (tryCounter == maxTries)
					throw new Exception("AORManagementProxy: Connection error.");

				try
				{
					proxy = new AORManagementChannel();
					proxy.Open();
					break;
				}
				catch (Exception e)
				{
					tryCounter++;
					Thread.Sleep(sleepTime);
					Trace.Write("Exception trace je:" + e.StackTrace);
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
