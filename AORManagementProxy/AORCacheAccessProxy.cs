﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	public class AORCacheAccessProxy
	{
		private const int maxTries = 10;
		private const int sleepTime = 3000;
		private AORCacheAccessChannel proxy;

		public AORCacheAccessProxy()
		{
			OpenChannel();
		}

		private void OpenChannel()
		{
			int tryCounter = 0;

			while (true)
			{
				if (tryCounter == maxTries)
					throw new Exception("AORCacheAccessProxy: Connection error.");

				try
				{
					proxy = new AORCacheAccessChannel();
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

		public AORCacheAccessChannel Proxy
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