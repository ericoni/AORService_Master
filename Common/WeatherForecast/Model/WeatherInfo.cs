using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.WeatherForecast.Model
{
	[Serializable]
	[DataContract]
	public class WeatherInfo
	{
		private Data currently;
		private Hourly hourly;
		private Daily daily;

		[DataMember]
		public Hourly Hourly
		{
			get
			{
				return hourly;
			}

			set
			{
				hourly = value;
			}
		}

		[DataMember]
		public Data Currently
		{
			get
			{
				return currently;
			}

			set
			{
				currently = value;
			}
		}

		[DataMember]
		public Daily Daily
		{
			get
			{
				return daily;
			}

			set
			{
				daily = value;
			}
		}

		public WeatherInfo()
		{
			Hourly = new Hourly();
			currently = new Data(667);
			Daily = new Daily();
		}
	}
}
