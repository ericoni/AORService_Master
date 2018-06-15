using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Services.NetworkModelService.DataModel.Meas;
using TSDB.Model;
using TSDB.Helper;

namespace TSDB.Converter
{
	/// <summary>
	/// Klasa za konvetovanje AnalogValue <=> HistorianItem
	/// </summary>
	public class DBConverter : IDBConverter
	{
		/// <summary>
		/// Helper pri radu sa bazom
		/// </summary>
		private DBHelper dbHelper;

		/// <summary>
		/// Konstruktor bez parametara
		/// </summary>
		public DBConverter()
		{
			dbHelper = new DBHelper();
		}

		/// <summary>
		/// Genericka metoda koja podrzava sledece tipove podataka:
		/// 1. AnalogValue
		/// 2. CollectItem
		/// 3. FiveMinutesItem
		/// 4. HourlyItem
		/// 5. DailyItem
		/// 6. MonthlyItem
		/// 7. AnnualItem
		/// </summary>
		/// <typeparam name="FROM"> Sta se konvertuje </typeparam>
		/// <typeparam name="TO"> U sta se konvertje </typeparam>
		/// <param name="values"> Lista FROM item-a </param>
		/// <returns> Lista TO item-a </returns>
		public List<TO> ConvertAnalogValueToHistorianItem<FROM, TO>(List<FROM> values)
			where FROM : AnalogValue
			where TO : HistorianItem
		{
			// Povratna vrednost
			List<TO> retValues = new List<TO>();

			// Ako je parametar null onda je greska
			if( values == null)
			{
				throw new Exception("Can not convert null values...");
			}

			foreach(FROM value in values)
			{
				// Pravljenje novog objekta
				TO item = (TO)Activator.CreateInstance(typeof(TO), new object[] { value.Value, value.Timestamp, value.GlobalId, value.PowerType, value.PowIncrease, value.PowDecrease });

				// Dodavanje u listu povratnih vrednosti
				retValues.Add(item);
			}

			return retValues;
		}

		public List<TO> ConvertHistorianItemToAnalogValue<FROM, TO>(List<FROM> values)
			where FROM : HistorianItem
			where TO : AnalogValue
		{

			// Povratna vrednost
			List<TO> retValues = new List<TO>();

			// Ako je parametar null onda je greska
			if (values == null)
			{
				throw new Exception("Can not convert null values...");
			}

			foreach (FROM value in values)
			{
				// Pravljenje novog objekta
				TO item = (TO)Activator.CreateInstance(typeof(TO), new object[] { value.Value, value.Timestamp, value.GlobalId, value.Type, value.Increase, value.Decrease });

				// Dodavanje u listu povratnih vrednosti
				retValues.Add(item);
			}

			return retValues;
		}
	}
}
