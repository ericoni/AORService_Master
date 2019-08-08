using AORCommon.Enumerations;
using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
	/// <summary>
	/// Event model for UI presentation. To do remove BindableBase vjereovatno. Ova se samo koristi za view modele.
    /// Mozda mi ne treba ova klasa uopste nego koristiti istu, ne znam zasto je ubacena. xD
	/// </summary>
	public class EventModel : BindableBase
	{
		private string username;
		private string details;
		private string region;
        private SeverityEnumeration severity;
		private DateTime systemTimestamp;
		private DateTime fieldTimestamp;

		public EventModel(string username, string details, string region, DateTime fieldTimestamp, DateTime systemTimestamp, 
            SeverityEnumeration severity = SeverityEnumeration.Low)
		{
			this.Username = username;
			this.Details = details;
			this.Region = region;
            this.Severity = severity;
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

        public  SeverityEnumeration Severity
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
