using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagementProxy
{
	public class AORViewerCommProxy
	{
		private const int maxTries = 10;
		private const int sleepTime = 3000;
		private AORViewerCommChannel proxy;

		public AORViewerCommProxy()
		{
			OpenChannel();
		}

		private void OpenChannel()
		{
			int tryCounter = 0;

			while (true)
			{
				if (tryCounter == maxTries)
					throw new Exception("AORViewerCommProxy: Connection error.");

				try
				{
					proxy = new AORViewerCommChannel();
					proxy.Open();
					break;
				}
				catch (Exception e)
				{
					tryCounter++;
					Thread.Sleep(sleepTime);
					Trace.Write("Exception trace :" + e.StackTrace);
				}
			}
		}

		public AORViewerCommChannel Proxy
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
