using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.WeatherForecast.Model
{
	public class Daily
	{
		private List<Data> data;

		public Daily()
		{
			data = new List<Model.Data>();
		}

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
	}
}
