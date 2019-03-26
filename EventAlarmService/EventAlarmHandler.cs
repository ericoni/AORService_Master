using FTN.Common.EventAlarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
    /// <summary>
    /// Service class used to implement alarm ack or event deletion.
    /// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class EventAlarmHandler // TODO left to be done
	{
		/// <summary>
		/// 
		/// </summary>
		private static volatile EventAlarmHandler instance;

		/// <summary>
		/// Lock object
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Singleton method
		/// </summary>
		public static EventAlarmHandler Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new EventAlarmHandler();
					}
				}

				return instance;
			}
		}

		public EventAlarmHandler()
		{
		}
	}
}
