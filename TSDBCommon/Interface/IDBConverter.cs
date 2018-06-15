using FTN.Services.NetworkModelService.DataModel.Meas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSDB.Model;

namespace TSDB.Converter
{
    public interface IDBConverter
    {
		/// <summary>
		/// Genericka metoda koja konvertuje Analogne vrednosti u sledece tipove podataka:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="FROM"> Sta se konvertuje </typeparam>
		/// <typeparam name="TO"> U sta se konvertje </typeparam>
		/// <param name="values"> Lista FROM item-a </param>
		/// <returns> Lista TO item-a </returns>
		List<TO> ConvertAnalogValueToHistorianItem<FROM, TO>(List<FROM> values) where FROM : AnalogValue where TO : HistorianItem;

		/// <summary>
		/// Genericka metoda koja konvertuje Historian item u Analog Value,
		/// tipovi Historian Item-a mogu biti sledeci tipovi podataka:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="FROM"> Sta se konvertuje </typeparam>
		/// <typeparam name="TO"> U sta se konvertje </typeparam>
		/// <param name="values"> Lista FROM item-a </param>
		/// <returns> Lista TO item-a </returns>
		List<TO> ConvertHistorianItemToAnalogValue<FROM, TO>(List<FROM> values) where FROM : HistorianItem where TO : AnalogValue;
	}
}
