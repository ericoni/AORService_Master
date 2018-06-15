using FTN.Common.WeatherForecast.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.WeatherForecast.Model;

namespace WeatherForecastProxyNS
{
    public class WeatherForecastChannel : ClientBase<IWeatherForecast>, IWeatherForecast
    {
        public WeatherForecastChannel()
            : base("WeatherForecastEndpoint")
        {
        }

        public WeatherInfo Get7DayPerHourForecastByGid(long gid)
        {
            return this.Channel.Get7DayPerHourForecastByGid(gid);
        }

        public WeatherInfo Get7DayPerHourForecastByLatLon(float lat, float lon)
        {
            return this.Channel.Get7DayPerHourForecastByLatLon(lat, lon);
        }

        public WeatherInfo GetCurrentWeatherDataByGlobalId(long gid)
        {
            return this.Channel.GetCurrentWeatherDataByGlobalId(gid);
        }

        public WeatherInfo GetCurrentWeatherDataByLatLon(float lat, float lon)
        {
            return this.Channel.GetCurrentWeatherDataByLatLon(lat, lon);
        }
    }
}
