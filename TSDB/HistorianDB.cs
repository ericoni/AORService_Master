using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Services.NetworkModelService.DataModel.Meas;
using System.ServiceModel;
using TSDB.Helper;
using TSDB.Converter;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using TSDB.Access;
using Adapter;
using TSDB.Model;

namespace TSDB.Access
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class HistorianDB : IHistorianDB
	{
		/// <summary>
		/// Singleton instanca
		/// </summary>
		private static volatile HistorianDB instance;

		/// <summary>
		/// Objekat za lock
		/// </summary>
		private static object syncRoot = new Object();

		/// <summary>
		/// Helper za rad sa bazom
		/// </summary>
		private DBHelper dbHelper;

		/// <summary>
		/// Converter za konvetovanje AnalogValue <=> HistorianItem
		/// </summary>
		private IDBConverter dbConverter;

		private RDAdapter rdAdapter;

		/// <summary>
		/// Konstruktor bez parametara
		/// </summary>
		private HistorianDB()
		{
			dbHelper = new DBHelper();
			dbConverter = new DBConverter();
			rdAdapter = new RDAdapter();
		}

		/// <summary>
		/// Singleton instanca
		/// </summary>
		public static IHistorianDB Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
							instance = new HistorianDB();
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Upis analognih merenja u bazu podataka
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool WriteAnalogValue(List<AnalogValue> values)
		{
			// Lista koja treba da se sacuva u bazu podataka
			List<CollectItem> collectItems = new List<CollectItem>();

			try
			{
				// Vrsi se konverzija analognih vrednosti u historianItem
				collectItems = dbConverter.ConvertAnalogValueToHistorianItem<AnalogValue, CollectItem>(values);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return false;
			}

			try
			{
				// Dodaje se lista historianItem-a u context collect. Klasa Helper brine o Operation tipu i svemu ostalom
				dbHelper.AddRange(collectItems);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return false;
			}

			return true;
		}

		public List<AnalogValue> GetHourlyConsuption(long date, PowerType powerType, long gid = -1)
		{
			// Powratna vrenost u koju se konvertuje historian item
			List<AnalogValue> retValAV = new List<AnalogValue>();

			// Ako je vreme npr. 12h znaci dajem potrosnju za vreme 12-1h
			DateTime dateTime = new DateTime(date);
			DateTime startDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
			DateTime endDate = startDate.AddHours(1);

			retValAV = GetConsuption<FiveMinutesItem>(startDate.Ticks, endDate.Ticks, powerType, gid);

			return retValAV;
		}

		public List<AnalogValue> GetDailyConsuption(long date, PowerType powerType, long gid = -1)
		{
			// Powratna vrenost u koju se konvertuje historian item
			List<AnalogValue> retValAV = new List<AnalogValue>();

			// Konverzija vremena, odredjivanje pocetne i krajnje granice
			DateTime dateTime = new DateTime(date);
			DateTime startDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			DateTime endDate = startDate.AddDays(1);

			retValAV = GetConsuption<HourlyItem>(startDate.Ticks, endDate.Ticks, powerType, gid);

			return retValAV;
		}

		public List<AnalogValue> GetMonthlyConsuption(long date, PowerType powerType, long gid = -1)
		{
			// Powratna vrenost u koju se konvertuje historian item
			List<AnalogValue> retValAV = new List<AnalogValue>();

			// Konverzija vremena, odredjivanje pocetne i krajnje granice
			DateTime dateTime = new DateTime(date);
			DateTime startDate = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
			DateTime endDate = startDate.AddMonths(1);

			retValAV = GetConsuption<DailyItem>(startDate.Ticks, endDate.Ticks, powerType, gid);

			return retValAV;
		}

		public List<AnalogValue> GetAnnualConsuption(long date, PowerType powerType, long gid = -1)
		{
			// Powratna vrenost u koju se konvertuje historian item
			List<AnalogValue> retValAV = new List<AnalogValue>();

			// Konverzija vremena, odredjivanje pocetne i krajnje granice
			DateTime dateTime = new DateTime(date);
			DateTime endDate = dateTime.AddYears(1);

			retValAV = GetConsuption<MonthlyItem>(dateTime.Ticks, endDate.Ticks, powerType, gid);

			return retValAV;
		}


		private List<AnalogValue> GetConsuption<T>(long startDate, long endDate, PowerType powerType, long gid = -1) where T : HistorianItem
		{
			// Pomocna lista gde ce biti smesteni gid-ovi analognih signala
			List<long> ids = new List<long>();

			// Povratna vrednost kao historian item
			List<T> retValHI = new List<T>();

			// Povratna vrenost u koju se konvertuje historian item
			List<AnalogValue> retValAV = new List<AnalogValue>();

			try
			{
				if (gid != -1)
				{
					// Dobijanje svih analognih signala za zeljeni region, subregion, substation, der
					ids = rdAdapter.GetAnalogValuesGidForGidAndPowerType(gid, powerType);
				}
				else
				{
					// Ako je gid -1, tada se radi o citavoj mrezi
					ids = rdAdapter.GetAllAnalogValuesGidForPowerType(powerType);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}

			using (var access = new AccessDB())
			{
				foreach (long id in ids)
				{
					retValHI.AddRange(dbHelper.GetRangeByGid<T>(id, startDate, endDate));
				}
			}

			// Konverzija historian itema u analogne vrednosti
			retValAV = dbConverter.ConvertHistorianItemToAnalogValue<T, AnalogValue>(retValHI);

			// Ukoliko se ne radi o DER-u, tada mora da se racuna prosecna potrosnja za sve der-ove unutar regiona, subregiona...
			if (!((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid)).Equals(DMSType.SYNCMACHINE))
			{
				retValAV = SumConsuption(retValAV);
			}

			return retValAV;
		}

		private List<AnalogValue> SumConsuption(List<AnalogValue> values)
		{
			// Lista povratnih vrednosti sa prosecno izracunatim vrednostima za svaki vremenski trenutak
			List<AnalogValue> sum = new List<AnalogValue>();

			// Pomocni dictionary
			Dictionary<long, List<AnalogValue>> temp = new Dictionary<long, List<AnalogValue>>();

			foreach (AnalogValue value in values)
			{
				if (!temp.ContainsKey(value.Timestamp))
				{
					temp.Add(value.Timestamp, new List<AnalogValue>());
				}

				temp[value.Timestamp].Add(value);
			}

			foreach (List<AnalogValue> tempValue in temp.Values)
			{
				AnalogValue example = tempValue.FirstOrDefault();

				float sumValue = tempValue.Sum(o => o.Value);
				float sumIncrease = tempValue.Sum(o => o.PowIncrease);
				float sumDecrease = tempValue.Sum(o => o.PowDecrease);

				sum.Add(new AnalogValue(-1)
				{
					Value = sumValue,
					PowDecrease = sumDecrease,
					PowIncrease = sumIncrease,
					PowerType = example.PowerType,
					Timestamp = example.Timestamp
				});
			}

			return sum;
		}
	}
}
