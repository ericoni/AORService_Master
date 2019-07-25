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

		public Event(string username, string details, string region, DateTime fieldTimestamp)
		{
			Username = username;
			Details = details;
			Region = region;
			FieldTimestamp = fieldTimestamp;
		}

		public string Username { get; set; }
		public string Details { get; set; }
		public string Region { get; set; }
		public DateTime FieldTimestamp { get; set; }
	}
}
