using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
	public class Event : BindableBase
	{
		private string username;
		private string message;
		private string region;
		private DateTime timestamp;

		public Event(string username, string message, string region)
		{
			this.Username = username;
			this.Message = message;
			this.Region = region;
			this.Timestamp = timestamp;
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

		public string Message
		{
			get { return message; }
			set
			{
				if (message != value)
				{
					message = value;
					OnPropertyChanged("Message");
				}
			}
		}

		public string Region
		{
			get { return region; }
			set
			{
				if (region != value)
				{
					region = value;
					OnPropertyChanged("Region");
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
