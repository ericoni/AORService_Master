using FTN.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSDB.Access;
using TSDB.Model;

namespace TSDB.Helper
{
    public interface IDBHelper
	{ 
		/// <summary>
		/// Genericka metod-a za dodavanje Item u bazu. Metodi mogu da se proslede sledeci tipovi:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Jedan od navedenih 6 tipova podataka</typeparam>
		/// <param name="values"> Lista vrednosti koje se dodaju </param>
		void AddRange<T>(List<T> values) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a pronalazenje svih vrednosti povezanih sa zeljenim globalnim identifikatorom, 
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <param name="gid"> Globalni identifikator na osnovu koga se pretrazuje</param>
		/// <returns> Lista povratnih vrednosti </returns>
		List<T> FindAllByGid<T>(long gid) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a pronalazenje svih vrednosti povezanih sa zeljenim PowerType, 
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <param name="powerType"> Tip snage (aktivna, reaktivna) </param>
		/// <returns> Lista povratnih vrednosti </returns>
		List<T> FindAllByPowerType<T>(PowerType powerType) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a pronalazenje svih vrednosti, u odredjenom vremenskom intervalu povezanih sa zeljenim globalnim identifikatorom
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <param name="gid"> Globalni identifikator </param>
		/// <param name="startTS"> Pocetak intervala </param>
		/// <param name="endTS"> Kraj intervala </param>
		/// <returns> Lista povratnih vrednosti </returns>
		List<T> GetRangeByGid<T>(long gid, long startTS, long endTS) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a pronalazenje svih vrednosti, u odredjenom vremenskom intervalu povezanih sa zeljenim PowerType-om
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <param name="powerType"> Tip snage (aktivna, reaktivna) </param>
		/// <param name="startTS"> Pocetak intervala </param>
		/// <param name="endTS"> Kraj intervala </param>
		/// <returns> Lista povratnih vrednosti </returns>
		List<T> GetRangeByPowerType<T>(PowerType powerType, long startTS, long endTS) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a koja vraca poslednji item ili null vrednost ako je tabela prazna
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <returns> Poslednji dodat item zeljenog tipa </returns>
		T LastOrDefault<T>() where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a koja vraca prvi item ili null vrednost ako je tabela prazna
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <returns> Poslednji dodat item zeljenog tipa </returns>
		T FirstOrDefault<T>() where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a koja vraca prvi item, sa zeljenim globalnim identifikatorom ili null vrednost ako je tabela prazna
		/// tipovi koji mogu biti povratna vrednost su sl:
		/// 1. CollectItem
		/// 2. FiveMinutesItem
		/// 3. HourlyItem
		/// 4. DailyItem
		/// 5. MonthlyItem
		/// 6. AnnualItem
		/// </summary>
		/// <typeparam name="T"> Tip koji se ocekuje kao povratna vrednost </typeparam>
		/// <param name="gid"> Globalni identifikator </param>
		/// <returns> Poslednji dodat item zeljenog tipa </returns>
		T FirstOrDefaultByGid<T>(long gid) where T : HistorianItem;

		/// <summary>
		/// Genericka metod-a koja odredjuje tip operacije, moguce operacije:
		/// 1. Insert
		/// 2. Update
		/// 3. Delete - nije podrzano
		/// </summary>
		/// <typeparam name="T"> Tip tabele koja se pretrazuje </typeparam>
		/// <param name="gid"> Globalni identifikatora </param>
		/// <returns> Operacija </returns>
		Operation GetOperation<T>(long gid) where T : HistorianItem;
    }
}
