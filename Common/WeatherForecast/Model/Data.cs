using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.WeatherForecast.Model
{
	public class Data
	{
		long time;
		string summary;
		double temperature;
		double humidity;
		double pressure;
		double windSpeed;
		double cloudCover;
		double visibility;
		long sunriseTime;
		long sunsetTime;

		public long Time
		{
			get
			{
				return time;
			}

			set
			{
				time = value;
			}
		}

		public string Summary
		{
			get
			{
				return summary;
			}

			set
			{
				summary = value;
			}
		}

		public double Humidity
		{
			get
			{
				return humidity;
			}

			set
			{
				humidity = value;
			}
		}

		public double Pressure
		{
			get
			{
				return pressure;
			}

			set
			{
				pressure = value;
			}
		}

		public double WindSpeed
		{
			get
			{
				return windSpeed;
			}

			set
			{
				windSpeed = value;
			}
		}

		public double CloudCover
		{
			get
			{
				return cloudCover;
			}

			set
			{
				cloudCover = value;
			}
		}

		public double Visibility
		{
			get
			{
				return visibility;
			}

			set
			{
				visibility = value;
			}
		}

		public double Temperature
		{
			get
			{
				return temperature;
			}

			set
			{
				temperature = value;
			}
		}

		public long SunriseTime
		{
			get
			{
				return sunriseTime;
			}

			set
			{
				sunriseTime = value;
			}
		}

		public long SunsetTime
		{
			get
			{
				return sunsetTime;
			}

			set
			{
				sunsetTime = value;
			}
		}

		public Data()
		{

		}


	}
}
