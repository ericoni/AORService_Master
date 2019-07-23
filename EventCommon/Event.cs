using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
	[Serializable]
	public class Event
	{
		public Event() { }

		public Event(string username, string details, DateTime timestamp)
		{
			Username = username;
			Details = details;
			Timestamp = timestamp;
		}

		public string Username { get; set; }
		public string Details { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
