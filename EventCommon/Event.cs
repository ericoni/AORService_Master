using AORCommon.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
    /// <summary>
    /// If you need to create an event use this method and send it via proxy to EventService. Servisi koriste ovu klasu.
    /// </summary>
	[Serializable]
	public class Event
	{
		public Event() { }

		public Event(string username, string details, string region, DateTime fieldTimestamp, SeverityEnumeration severity = SeverityEnumeration.Low)
		{
			Username = username;
			Details = details;
			Region = region;
			FieldTimestamp = fieldTimestamp;
            Severity = severity;
		}

		public string Username { get; set; }
		public string Details { get; set; }
		public string Region { get; set; }
        public SeverityEnumeration Severity { get; set; }
        public DateTime FieldTimestamp { get; set; }
	}
}
