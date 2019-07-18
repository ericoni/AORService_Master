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
		private string details;
		private string region;
		private DateTime timestamp;

		public Event(string username, string details, string region)
		{
			this.Username = username;
			this.Details = details;
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
