using Adapter;
using FTN.Common;
using FTN.Common.WeatherForecast.Model;
using FTN.Common.WeatherForecast.Service;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WeatherForecastService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class WeatherForcast : IWeatherForecast
	{

		/// <summary>
		/// Kljuc sa kojm se pristupa sajtu openweathermap
		/// </summary>
		private const string appId = "b717bb2a3ab839b54c5784935b13892f"; // ovo je stara  b717bb2a3ab839b54c5784935b13892f       076e0fe79255d818ac0dcc5404821365

		/// <summary>
		/// Adapter za komunikaciju sam NMS servisom
		/// </summary>
		private INetworkModelClient rdAdapter = new RDAdapter();

		/// <summary>
		/// Za svaki substation trenutna vremenska prognoza
		/// </summary>
		private Dictionary<long, WeatherInfo> currentWeather = new Dictionary<long, WeatherInfo>();

		/// <summary>
		/// Lock objekat
		/// </summary>
		private static readonly object lockCW = new object();

		/// <summary>
		/// Za svaki substation forecest
		/// </summary>
		private Dictionary<long, WeatherInfo> forecastWeather = new Dictionary<long, WeatherInfo>();

		/// <summary>
		/// Lock objekat
		/// </summary>
		private static readonly object lockFW = new object();

		/// <summary>
		/// Singleton instanca
		/// </summary>
		private static WeatherForcast instance;

		/// <summary>
		/// Lock objekat
		/// </summary>
		private static readonly object syncRoot = new object();

		/// <summary>
		/// Get/Set for RDAdapter
		/// </summary>
		public INetworkModelClient RdAdapter
		{
			get
			{
				return rdAdapter;
			}

			set
			{
				rdAdapter = value;
			}
		}

		/// <summary>
		/// Metoda za Singleton
		/// </summary>
		public static WeatherForcast Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new WeatherForcast();
					}
				}

				return instance;
			}
		}

		public  WeatherForcast()
		{
			List<Substation> substations = rdAdapter.GetAllSubstation();

			Thread currentThread = new Thread(() => CurrentWeatherJob(substations));
			Thread forecastThread = new Thread(() => ForecastWeatherJob(substations));

			forecastThread.Start();
			currentThread.Start();
		}

		private void CurrentWeatherJob(List<Substation> substations)
		{
			while (true)
			{
				lock (lockCW)
				{
					foreach (Substation substation in substations)
					{
						WeatherInfo weatherInfo = GetCurrentWeatherDataByLatLon(substation.Latitude, substation.Longitude);

						if (!currentWeather.ContainsKey(substation.GlobalId))
						{
							currentWeather.Add(substation.GlobalId, weatherInfo);
						}
						else
						{
							currentWeather[substation.GlobalId] = weatherInfo;
						}
					}
				}

				Thread.Sleep(60 * 60 * 1000);
            }
		}

		private void ForecastWeatherJob(List<Substation> substations)
		{
			while (true)
			{
				lock (lockFW)
				{
					foreach (Substation substation in substations)
					{
						WeatherInfo weatherInfo = Get7DayPerHourForecastByLatLon(substation.Latitude, substation.Longitude);

						if (!forecastWeather.ContainsKey(substation.GlobalId))
						{
							forecastWeather.Add(substation.GlobalId, weatherInfo);
						}
						else
						{
							forecastWeather[substation.GlobalId] = weatherInfo;
						}
					}
				}

				Thread.Sleep(2 * 60 * 60 * 1000);
            }
		}

		/// <summary>
		/// You can call by city name or city name and country code. API responds with a list of results that match a searching word.
		/// </summary>
		/// <param name="gid"Global id"</param>
		public WeatherInfo GetCurrentWeatherDataByGlobalId(long gid)
		{
			// Povratna vrednost
			WeatherInfo weatherInfoRetVal = new WeatherInfo();

			// Lista substationa za koje se dobija vremenska prognoza
			List<Substation> substations = new List<Substation>();

			// Pomocna lista
			List<WeatherInfo> temp = new List<WeatherInfo>();

			// Pomocna lista
			List<Data> datas = new List<Data>();

			// Iz gid-a se dobija DMSType i na osnovu toga se dalje pribavljaju substation-i
			DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid);

			switch (type)
			{
				case DMSType.REGION:
					// Ukoliko je gid od regiona, moraju se dobaviti svi sub region-i zatim od njih svi substation-i
					List<SubGeographicalRegion> subRegions = RdAdapter.GetSubRegionsForRegion(gid);

					// Prolazi se kroz sve subRegion-e i dobavljaju se svi substation-i
					foreach (SubGeographicalRegion subRegion in subRegions)
					{
						substations.AddRange(RdAdapter.GetSubstationsForSubRegion(subRegion.GlobalId));
					}

					// Prolazi se kroz substation-e i za svaki se trazi vremenska prognoza
					foreach (Substation substation in substations)
					{
						lock (lockCW)
						{
                            WeatherInfo info = null;
                            currentWeather.TryGetValue(substation.GlobalId, out info);

                            if (info == null)
                            {
                                info = GetCurrentWeatherDataByLatLon(substation.Latitude, substation.Longitude);

                                currentWeather.Add(substation.GlobalId, info);
                            }

                            temp.Add(info);
						}
					}

					// Racuna se prosecna vremenska prognoza za region i vraca se kao povratna vrednost
					foreach (WeatherInfo weatherInfo in temp)
					{
						datas.Add(weatherInfo.Currently);
					}
					weatherInfoRetVal.Currently = CalculateAverageData(datas);

					break;

				case DMSType.SUBREGION:
					// Ukoliko se radi o subregion-u, tada se za taj subregion dobavljaju svi substation-i
					substations.AddRange(RdAdapter.GetSubstationsForSubRegion(gid));

					// Prolazi se kroz substation-e i za svaki se trazi vremenska prognoza
					foreach (Substation substation in substations)
					{
                        lock (lockCW)
                        {
                            WeatherInfo info = null;
                            currentWeather.TryGetValue(substation.GlobalId, out info);

                            if (info == null)
                            {
                                info = GetCurrentWeatherDataByLatLon(substation.Latitude, substation.Longitude);

                                currentWeather.Add(substation.GlobalId, info);
                            }
                            temp.Add(info);

                        }
					}

					// Racuna se prosecna vremenska prognoza za region i vraca se kao povratna vrednost
					foreach (WeatherInfo weatherInfo in temp)
					{
						datas.Add(weatherInfo.Currently);
					}
					weatherInfoRetVal.Currently = CalculateAverageData(datas);

					break;
				case DMSType.SUBSTATION:
					// Ukoliko se radi o substation-u, dobavi se odredjeni substation i za njega se trazi vremenska prognoza
					Substation tempSubstation = RdAdapter.GetSubstation(gid);

					lock (lockCW)
					{
                        weatherInfoRetVal = null;
                        currentWeather.TryGetValue(tempSubstation.GlobalId, out weatherInfoRetVal);

                        if (weatherInfoRetVal == null)
                        {
                            weatherInfoRetVal = GetCurrentWeatherDataByLatLon(tempSubstation.Latitude, tempSubstation.Longitude);

                            currentWeather.Add(tempSubstation.GlobalId, weatherInfoRetVal);
                        }
                    }

					break;

				case DMSType.SYNCMACHINE:
					// Ukoliko se radi o sinhronom masini, dobavim tu sinhronu masinu, i vidim u kom substationu se nalazi, 
					// zatim zatrazim vremensku prognozu za taj substation
					SynchronousMachine sm = rdAdapter.GetSyncMachineByGid(gid);

					lock (lockCW)
					{
                        weatherInfoRetVal = null;
                        currentWeather.TryGetValue(sm.EquipmentContainer, out weatherInfoRetVal);

                        if (weatherInfoRetVal == null)
                        {
                            tempSubstation = RdAdapter.GetSubstation(sm.EquipmentContainer);
                            weatherInfoRetVal = GetCurrentWeatherDataByLatLon(tempSubstation.Latitude, tempSubstation.Longitude);

                            currentWeather.Add(tempSubstation.GlobalId, weatherInfoRetVal);
                        }
                    }

					break;
			}

			return weatherInfoRetVal;
		}

		/// <summary>
		/// You can call by city name or city name and country code. API responds with a list of results that match a searching word.
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		public WeatherInfo GetCurrentWeatherDataByLatLon(float lat, float lon)
		{
			//WeatherInfo weatherInfo = null;

			//string url = string.Format("https://api.darksky.net/forecast/{0}/{1},{2}?units=si&exclude=flags,alerts,minutely,hourly", appId, lat, lon);
			//using (WebClient client = new WebClient())
			//{

			//    string json = client.DownloadString(url);
			//    weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
			//}

			//weatherInfo.Currently.Time = UnixTimeStampToDateTime(weatherInfo.Currently.Time).Ticks;
			//weatherInfo.Daily.Data.FirstOrDefault().Time = UnixTimeStampToDateTime(weatherInfo.Daily.Data.FirstOrDefault().Time).Ticks;
			//weatherInfo.Daily.Data.FirstOrDefault().SunriseTime = UnixTimeStampToDateTime(weatherInfo.Daily.Data.FirstOrDefault().SunriseTime).Ticks;
			//weatherInfo.Daily.Data.FirstOrDefault().SunsetTime = UnixTimeStampToDateTime(weatherInfo.Daily.Data.FirstOrDefault().SunsetTime).Ticks;

			//return weatherInfo; //vrati se ovamo
			return new WeatherInfo();
		}

		public WeatherInfo Get7DayPerHourForecastByGid(long gid)
		{
			// Povratna vrednost
			WeatherInfo weatherInfoRetVal = new WeatherInfo();

			// Lista substationa za koje se dobija vremenska prognoza
			List<Substation> substations = new List<Substation>();

			// Pomocna lista
			List<WeatherInfo> temp = new List<WeatherInfo>();

			// Iz gid-a se dobija DMSType i na osnovu toga se dalje pribavljaju substation-i
			DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid);

			switch (type)
			{
				case DMSType.REGION:
					// Ukoliko je gid od regiona, moraju se dobaviti svi sub region-i zatim od njih svi substation-i
					List<SubGeographicalRegion> subRegions = RdAdapter.GetSubRegionsForRegion(gid);

					// Prolazi se kroz sve subRegion-e i dobavljaju se svi substation-i
					foreach (SubGeographicalRegion subRegion in subRegions)
					{
						substations.AddRange(RdAdapter.GetSubstationsForSubRegion(subRegion.GlobalId));
					}

					// Prolazi se kroz substation-e i za svaki se trazi predvidjanje vremenske prognoze
					foreach (Substation substation in substations)
					{
						lock (lockFW)
						{
                            WeatherInfo info = null;
                            forecastWeather.TryGetValue(substation.GlobalId, out info);

                            if (info == null)
                            {
                                info = Get7DayPerHourForecastByLatLon(substation.Latitude, substation.Longitude);

                                forecastWeather.Add(substation.GlobalId, info);
                            }

                            temp.Add(info);
                        }
					}

					// Prolazi se 168 puta i za svaki substation za svaki sat se radi proracun prosecne prognoze
					for (int i = 0; i < temp.FirstOrDefault().Hourly.Data.Count; i++)
					{
						List<Data> section = new List<Data>();

						foreach (WeatherInfo weatherInfo in temp)
						{
							section.Add(weatherInfo.Hourly.Data[i]);
						}

						weatherInfoRetVal.Hourly.Data.Add(CalculateAverageData(section));
					}
					break;

				case DMSType.SUBREGION:
					// Ukoliko se radi o subregion-u, tada se za taj subregion dobavljaju svi substation-i
					substations.AddRange(RdAdapter.GetSubstationsForSubRegion(gid));

					// Prolazi se kroz substation-e i za svaki se trazi vremenska prognoza
					foreach (Substation substation in substations)
					{
						lock (lockFW)
						{
                            WeatherInfo info = null;
                            forecastWeather.TryGetValue(substation.GlobalId, out info);

                            if (info == null)
                            {
                                info = Get7DayPerHourForecastByLatLon(substation.Latitude, substation.Longitude);

                                forecastWeather.Add(substation.GlobalId, info);
                            }
                            temp.Add(info);
                        }
					}

					// Prolazi se 168 puta i za svaki substation za svaki sat se radi proracun prosecne prognoze
					for (int i = 0; i < temp.FirstOrDefault().Hourly.Data.Count; i++)
					{
						List<Data> section = new List<Data>();

						foreach (WeatherInfo weatherInfo in temp)
						{
							section.Add(weatherInfo.Hourly.Data[i]);
						}

						weatherInfoRetVal.Hourly.Data.Add(CalculateAverageData(section));
					}
					break;

				case DMSType.SUBSTATION:
					// Ukoliko se radi o substation-u, dobavi se odredjeni substation i za njega se trazi vremenska prognoza
					Substation tempSubstation = RdAdapter.GetSubstation(gid);
					//weatherInfoRetVal = Get7DayPerHourForecastByLatLon(tempSubstation.Latitude, tempSubstation.Longitude);

					lock (lockFW)
					{
                        weatherInfoRetVal = null;
                        forecastWeather.TryGetValue(tempSubstation.GlobalId, out weatherInfoRetVal);

                        if (weatherInfoRetVal == null)
                        {
                            weatherInfoRetVal = Get7DayPerHourForecastByLatLon(tempSubstation.Latitude, tempSubstation.Longitude);

                            forecastWeather.Add(tempSubstation.GlobalId, weatherInfoRetVal);
                        }
                    }

					break;

				case DMSType.SYNCMACHINE:
					// Ukoliko se radi o sinhronom masini, dobavim tu sinhronu masinu, i vidim u kom substationu se nalazi, 
					// zatim zatrazim vremensku prognozu za taj substation
					SynchronousMachine sm = rdAdapter.GetSyncMachineByGid(gid);

					lock (lockFW)
					{
                        weatherInfoRetVal = null;
                        forecastWeather.TryGetValue(sm.EquipmentContainer, out weatherInfoRetVal);

                        if (weatherInfoRetVal == null)
                        {
                            tempSubstation = RdAdapter.GetSubstation(sm.EquipmentContainer);
                            weatherInfoRetVal = Get7DayPerHourForecastByLatLon(tempSubstation.Latitude, tempSubstation.Longitude);

                            forecastWeather.Add(tempSubstation.GlobalId, weatherInfoRetVal);
                        }
                    }

					break;
			}

			return weatherInfoRetVal;
		}

		public WeatherInfo Get7DayPerHourForecastByLatLon(float lat, float lon)
		{
            //WeatherInfo weatherInfo = null;

            //string url = string.Format("https://api.darksky.net/forecast/{0}/{1},{2}?extend=hourly&units=si&exclude=flags,alerts,minutely,currently", appId, lat, lon);
            //using (WebClient client = new WebClient())
            //{

            //    string json = client.DownloadString(url);
            //    weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
            //}

            //foreach (Data data in weatherInfo.Hourly.Data)
            //{
            //    data.Time = UnixTimeStampToDateTime(data.Time).Ticks;
            //}

            //return weatherInfo; //vrati se ovamo
			return new WeatherInfo();
		}

		/// <summary>
		/// Metoda za racunanje prosecne vremenske prognoze na nivou regiona ili subregion-a
		/// </summary>
		/// <param name="weatherInfos">Lista vremenskih prognoza</param>
		/// <returns></returns>
		private Data CalculateAverageData(List<Data> datas)
		{
			// Povratna vrednost metode
			Data average = new Data();

			// Dictionary u kome se racuna koje je vreme najcesce u regionu i ono se proglasava za vremne u regionu
			Dictionary<string, int> tempWeather = new Dictionary<string, int>();

			average.CloudCover = datas.Average(o => o.CloudCover);
			average.Humidity = datas.Average(o => o.Humidity);
			average.Pressure = datas.Average(o => o.Pressure);
			average.Temperature = datas.Average(o => o.Temperature);
			average.Time = datas.FirstOrDefault().Time;
			average.Visibility = datas.Average(o => o.Visibility);
			average.WindSpeed = datas.Average(o => o.WindSpeed);


			// Vreme koje je pretezno za region ili subregion, odnosno vreme koje vazi za najveci broj substation-a
			foreach (Data data in datas)
			{
				if (!tempWeather.ContainsKey(data.Summary))
				{
					tempWeather.Add(data.Summary, 0);
				}

				tempWeather[data.Summary]++;
			}

			// Vreme koje je pretezno za region ili subregion, odnosno vreme koje vazi za najveci broj substation-a
			average.Summary = tempWeather.FirstOrDefault(x => x.Value == tempWeather.Values.Max()).Key;

			return average;
		}

		public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}
	}
}
