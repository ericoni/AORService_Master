using System;
using System.Collections.Generic;
using System.Linq;
using FTN.Common;
using TSDB.Access;
using TSDB.Model;

namespace TSDB.Helper
{
    public class DBHelper : IDBHelper
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
		public void AddRange<T>(List<T> values) where T : HistorianItem
		{
			int i = 0;

			// Ukoliko je parametar null ili je lista prazna, ne izvrsava se metoda
			if (values == null || values.Count == 0)
			{
				Console.WriteLine(DateTime.Now + ": List of collect items is null or empty.");
				return;
			}

			// Postavlja se operacija, da li se radi o update ili insert
			foreach (HistorianItem value in values)
			{
				value.Operation = GetOperation<T>(value.GlobalId);
			}

			using (AccessDB access = new AccessDB())
			{
				// Dodavanje u bazu
				access.Set<T>().AddRange(values);

				// Snimanje promena
				i = access.SaveChanges();
			}

			// Greska je ukoliko "i" nije pozitivan broj
			if (i <= 0)
			{
				throw new Exception(DateTime.Now + ": Saving failed.");
			}
		}

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
		public List<T> FindAllByGid<T>(long gid) where T : HistorianItem
		{
			// Lista povratnih vrednosti
			List<T> values = new List<T>();

			using (AccessDB access = new AccessDB())
			{
				// Upit u tabelu tipa T
				values = access.Set<T>().Where(o => o.GlobalId.Equals(gid)).ToList();
			}

			return values;	
		}

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
		public List<T> FindAllByPowerType<T>(PowerType powerType) where T : HistorianItem
		{
			// Povratna vrednost
			List<T> values = new List<T>();

			using (AccessDB access = new AccessDB())
			{
				// Upit u tabelu tipa T
				values = access.Set<T>().Where(o => ((short)o.Type).Equals((short)powerType)).ToList();
			}

			return values;
		}

		/// <summary>
		/// Genericka metod-a koja odredjuje tip operacije, moguce operacije:
		/// 1. Insert
		/// 2. Update
		/// 3. Delete - nije podrzano
		/// </summary>
		/// <typeparam name="T"> Tip tabele koja se pretrazuje </typeparam>
		/// <param name="gid"> Globalni identifikatora </param>
		/// <returns> Operacija </returns>
		public Operation GetOperation<T>(long gid) where T : HistorianItem
		{
			// Prvi item u tabeli za dati gid
			T item = null;

			// Pretrazuje se da li postoji entitet sa zeljenim identifikatorom
			item = this.FirstOrDefaultByGid<T>(gid);

			// Ako je item == null to znaci da ne postoji u bazi, i tada se radi o insertu
			if (item == null)
			{
				return Operation.Insert;
			}

			// Ako vrednost nije null, onda entitet postoji i radi se o update
			return Operation.Update;
		}

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
		public List<T> GetRangeByGid<T>(long gid, long startTS, long endTS) where T : HistorianItem
		{
			// Povratna vrednost
			List<T> values = new List<T>();

			using (AccessDB access = new AccessDB())
			{
				// Upit za dobijanje svih item-a sa datim gid-om u datom intervalu
				values = access.Set<T>().Where(o => o.GlobalId.Equals(gid) && o.Timestamp >= startTS && o.Timestamp <= endTS).ToList();
			}

			return values;
		}

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
		public List<T> GetRangeByPowerType<T>(PowerType powerType, long startTS, long endTS) where T : HistorianItem
		{
			// Povratna vrednost
			List<T> values = new List<T>();

			using (AccessDB access = new AccessDB())
			{
				// Upit za dobijanje svih item-a sa datim powerType-om u datom intervalu
				values = access.Set<T>().Where(o => ((short)o.Type).Equals((short)powerType) && o.Timestamp >= startTS && o.Timestamp <= endTS).ToList();
			}

			return values;
		}

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
		public T LastOrDefault<T>() where T : HistorianItem
		{
			// Povratna vrednost
			T value = null;

			using (AccessDB access = new AccessDB())
			{
				// Upit za poslednji item u tabeli
				value = access.Set<T>().Where(o => o.Timestamp.Equals(access.Set<T>().Max(ob => ob.Timestamp))).FirstOrDefault();
			}

			return value;
		}

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
		public T FirstOrDefault<T>() where T : HistorianItem
		{
			// Povratna vrednost
			T value = null;

			using (AccessDB access = new AccessDB())
			{
				// Upit za prvi item u tabeli
				value = access.Set<T>().FirstOrDefault();
			}

			return value;
		}

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
		public T FirstOrDefaultByGid<T>(long gid) where T : HistorianItem
		{
			// Povratna vrednost
			T value = null;

			using (AccessDB access = new AccessDB())
			{
				// Prvi item datog gid-a
				value = access.Set<T>().FirstOrDefault(o => o.GlobalId.Equals(gid));
			}

			return value;
		}
	}
}
