﻿using AORManagementProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AORManagementProxyNS
{
	public class AOREventAlarmProxy // TODO temp bice samo za evente
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
				if (tryCounter == maxTries)
					throw new Exception("AOREventAlarmProxy: Connection error.");

				try
				{
					proxy = new AOREventAlarmChannel();
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
