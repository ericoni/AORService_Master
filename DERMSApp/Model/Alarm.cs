using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
	public enum AlarmSeverity
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public class Alarm : BindableBase
	{
		private string username;
		private string details;
		private AlarmSeverity severity;
		private DateTime timestamp;

		public Alarm(string username, string details, AlarmSeverity severity = AlarmSeverity.Low)
		{
			this.Username = username;
			this.Details = details;
			this.Timestamp = timestamp;
			this.Severity = severity;
		}

		public string Username
		{
			get { return username; }
			set
			{
				if (username != value)
				{
					username = value;
					OnPropertyChanged("Username");
				}
			}
		}

		public string Details
		{
			get { return details; }
			set
			{
				if (details != value)
				{
					details = value;
					OnPropertyChanged("Details");
				}
			}
		}

		public AlarmSeverity Severity
		{
			get { return severity; }
			set
			{
				if (severity != value)
				{
					severity = value;
					OnPropertyChanged("Severity");
				}
			}
		}

		public DateTime Timestamp
		{
			get { return timestamp; }
			set
			{
				if (timestamp != value)
				{
					timestamp = value;
					OnPropertyChanged("Timestamp");
				}
			}
		}
	}
}
