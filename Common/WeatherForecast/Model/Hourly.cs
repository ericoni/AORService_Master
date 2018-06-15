using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.WeatherForecast.Model
{
	public class Hourly
	{
		private List<Data> data;

		public List<Data> Data
		{
			get
			{
				return data;
			}

			set
			{
				data = value;
			}
		}

		public Hourly()
		{
			Data = new List<Data>();
		}
	}
}
