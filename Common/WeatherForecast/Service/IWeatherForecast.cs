using FTN.Common.WeatherForecast.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.WeatherForecast.Service
{
	[ServiceContract]
	public interface IWeatherForecast
	{
		/// <summary>
		/// You can call by city name or city name and country code. API responds with a list of results that match a searching word.
		/// </summary>
		/// <param name="gid">Global ID</param>
		/// <returns></returns>
		[OperationContract]
		WeatherInfo GetCurrentWeatherDataByGlobalId(long gid);

		/// <summary>
		/// You can call by city name or city name and country code. API responds with a list of results that match a searching word.
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		/// <returns></returns>
		[OperationContract]
		WeatherInfo GetCurrentWeatherDataByLatLon(float lat, float lon);

		/// <summary>
		/// 16 day forecasts is available at any location or city. 
		/// </summary>
		/// <param name="gid"></param>
		/// <returns></returns>
		[OperationContract]
		WeatherInfo Get7DayPerHourForecastByGid(long gid);

		/// <summary>
		/// 16 day forecasts is available at any location or city. 
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		/// <returns></returns>
		[OperationContract]
		WeatherInfo Get7DayPerHourForecastByLatLon(float lat, float lon);
	}
}
