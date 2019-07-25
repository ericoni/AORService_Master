using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
	/// <summary>
	/// Event model for UI presentation. To do mozda izbaciti bindable base.
	/// </summary>
	public class EventModel : BindableBase
	{
		private string username;
		private string details;
		private string region;
		private DateTime systemTimestamp;
		private DateTime fieldTimestamp;

		public EventModel(string username, string details, string region, DateTime fieldTimestamp, DateTime systemTimestamp)
		{
			this.Username = username;
			this.Details = details;
			this.Region = region;
			this.FieldTimestamp = fieldTimestamp;
			this.SystemTimestamp = systemTimestamp;
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

		public DateTime SystemTimestamp
		{
			get { return systemTimestamp; }
			set
			{
				if (systemTimestamp != value)
				{
					systemTimestamp = value;
					OnPropertyChanged("SystemTimestamp");
				}
			}
		}

		public DateTime FieldTimestamp
		{
			get { return fieldTimestamp; }
			set
			{
				if (fieldTimestamp != value)
				{
					fieldTimestamp = value;
					OnPropertyChanged("FieldTimestamp");
				}
			}
		}
	}
}
