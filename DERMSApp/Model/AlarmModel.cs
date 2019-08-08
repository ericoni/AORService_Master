using AORCommon.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
	[Serializable]
	public class AlarmModel : BindableBase
	{
		private string details;
		private string region;
		private string substation;
		private string generatorType;
		private SeverityEnumeration severity;
		private DateTime fieldTimestamp;

		public AlarmModel(string details, string region, DateTime fieldTimestamp, SeverityEnumeration severity = SeverityEnumeration.Low, string substation = null, string generatorType = null)
		{
			this.Details = details;
			this.Region = region;
			this.FieldTimestamp = fieldTimestamp;
			this.Severity = severity;
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

		public SeverityEnumeration Severity
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

		public string Substation
		{
			get { return substation; }
			set
			{
				if (substation != value)
				{
					substation = value;
					OnPropertyChanged("Substation");
				}
			}
		}

		public string GeneratorType
		{
			get { return generatorType; }
			set
			{
				if (generatorType != value)
				{
					generatorType = value;
					OnPropertyChanged("GeneratorType");
				}
			}
		}
	}
}
