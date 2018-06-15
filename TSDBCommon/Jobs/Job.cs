using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSDB.Access;
using TSDB.Converter;
using TSDB.Helper;
using TSDB.Model;

namespace TSDB.Jobs
{
	public abstract class Job
	{
		#region Semapthore
		// Sinhronizacija Five Minutes Job-a i Hourly Job-a
		private static Semaphore _FMJ_HJ_Semapthore = new Semaphore(0, 1);

		// Sinhronizacija Hourly Job-a i Daily Job-a
		private static Semaphore _HJ_DJ_Semapthore = new Semaphore(0, 1);

		// Sinhronizacija Daily Job-a i Monthly Job-a
		private static Semaphore _DJ_MJ_Semapthore = new Semaphore(0, 1);

		// Sinhronizacija Monthly Job-a i Annual job-a
		private static Semaphore _MJ_YJ_Semapthore = new Semaphore(0, 1);

		// Database helper
		private IDBHelper dbHelper = new DBHelper();

		// Database converter
		private IDBConverter dbConverter = new DBConverter();

		/// <summary>
		/// Get/Set
		/// </summary>
		public static Semaphore FMJ_HJ_Semapthore
		{
			get
			{
				return _FMJ_HJ_Semapthore;
			}

			set
			{
				_FMJ_HJ_Semapthore = value;
			}
		}

		/// <summary>
		/// Get/Set
		/// </summary>
		public static Semaphore HJ_DJ_Semapthore
		{
			get
			{
				return _HJ_DJ_Semapthore;
			}

			set
			{
				_HJ_DJ_Semapthore = value;
			}
		}

		/// <summary>
		/// Get/Set
		/// </summary>
		public static Semaphore DJ_MJ_Semapthore
		{
			get
			{
				return _DJ_MJ_Semapthore;
			}

			set
			{
				_DJ_MJ_Semapthore = value;
			}
		}

		/// <summary>
		/// Get/Set
		/// </summary>
		public static Semaphore MJ_YJ_Semapthore
		{
			get
			{
				return _MJ_YJ_Semapthore;
			}

			set
			{
				_MJ_YJ_Semapthore = value;
			}
		}
		#endregion Semapthore

		/// <summary>
		/// Proravcunava pocetak intervala
		/// </summary>
		/// <returns></returns>
		public abstract DateTime CalculateStartInterval(DateTime time);

		/// <summary>
		/// Proracunava kraj intervala
		/// </summary>
		/// <returns></returns>
		public abstract DateTime CalculateEndInterval(DateTime startTime);

		/// <summary>
		///  Thread koji je razlicit od Job-a do job-a
		/// </summary>
		public abstract void Collector();

		/// <summary>
		/// Vrsi inicijalizaciju tabela pri pokretanju TSDB servisa
		/// </summary>
		/// <typeparam name="READ"></typeparam>
		/// <typeparam name="WRITE"></typeparam>
		public void TableInitializer<READ, WRITE>() where READ : HistorianItem where WRITE : HistorianItem
		{
			Console.WriteLine("Start with {0} initialization...", typeof(WRITE));

			// Poslednji upis u tabelu u koju se zapisu vrednosti
			HistorianItem write_table_lastItem = null;

			// Prvi upis u tabelu iz koje se cita
			HistorianItem read_table_firstItem = null;

			// Poslednji upis u tabelu iz koje se cita
			HistorianItem read_table_lastItem = null;

			// Pocetak intervala
			DateTime startTime = DateTime.Now;

			// Kraj intervala
			DateTime endTime = DateTime.Now;

			// Trenutno vreme
			DateTime current = DateTime.Now;

			// Uzimanje poslednjeg itema iza Five Minutes tabele
			using (AccessDB access = new AccessDB())
			{
				// Poslednji item tabele u koju trebamo da pisemo
				write_table_lastItem = dbHelper.LastOrDefault<WRITE>();

				// Prvi item Collect tabele
				read_table_firstItem = dbHelper.FirstOrDefault<READ>();

				// Poslednji item Collect tabele
				read_table_lastItem = dbHelper.LastOrDefault<READ>();
			}

			// Ako je tabela u koju se pise prazna 
			if (write_table_lastItem == null)
			{
				// I ako je tabela iz koje se cita prazna, onda je kraj
				if (read_table_firstItem == null)
				{
					return;
				}

				// U suprotonom krece se od pocetka tabele iz koje se cita i odredjuje se pocetak intervala
				startTime = CalculateStartInterval(new DateTime(read_table_firstItem.Timestamp));
				
				// Racuna se kraj intervala
				endTime = CalculateEndInterval(startTime);

			}
			// Ako tabela u koju se pise nije prazna
			else
			{
				// Odredjuje se pocetak intervala, na osnovu poslednjeg dodatog itema u tabelu u kojoj se pise
				startTime = CalculateEndInterval(new DateTime(write_table_lastItem.Timestamp));

				// Odredjuje se kraj intervala
				endTime = CalculateEndInterval(startTime);
			}

			while (true)
			{
				// Proverava se da li trenutno vreme upada u interval, ako upada onda Job i nije trebao da se izvrsi i zavrsava
				if (current >= startTime && current <= endTime)
				{
					Console.WriteLine("Finished with {0} initialization...", typeof(WRITE));
					return;
				}


				if (startTime.Ticks > read_table_lastItem.Timestamp)
				{
					Console.WriteLine("Finished with {0} initialization...", typeof(WRITE));
					return;
				}


				// U suprotnom radi se proracun i snimanje u bazu
				CalculateAndStore<READ, WRITE>(startTime, endTime);

				// Odredjuje se novi pocetni trenutak
				startTime = endTime;

				// Odredjuje se kraj
				endTime = CalculateEndInterval(startTime);
			}
		}


		/// <summary>
		/// Racuna prosecnu potrosnju u intervalu odredjenom funkcijama CalculateStartInterval, CalculateEndInterval
		/// za aktivnu i reaktivnu snagu, pri tom vrsi skladistenje u odgovarajucu tabelu tabelu, 
		/// takodje metoda vrsi konverziju podataka izmedju dve tabele
		/// </summary>
		/// <typeparam name="READ"> Tip podatka tabele iz koje se citaju podaci </typeparam>
		/// <typeparam name="WRITE"> Tip podatka tabele u koju se upisuju podaci </typeparam>
		/// <param name="startTime"> Pocetak intervala </param>
		/// <param name="endTime"> Kraj intervala </param>
		public void CalculateAndStore<READ, WRITE>(DateTime startTime, DateTime endTime) where READ : HistorianItem where WRITE : HistorianItem
		{
			// Liste potrebne za proracun
			List<READ> activeValues = null;
			List<READ> reactiveValues = null;

			// Tabele za smestanje prosecnih vrednosti
			List<WRITE> activeAverageValues = null;
			List<WRITE> reactiveAverageValues = null;

			using (AccessDB access = new AccessDB())
			{
				activeValues = dbHelper.GetRangeByPowerType<READ>(PowerType.Active, startTime.Ticks, endTime.Ticks);
				reactiveValues = dbHelper.GetRangeByPowerType<READ>(PowerType.Reactive, startTime.Ticks, endTime.Ticks);

				activeAverageValues = Average<READ, WRITE>(activeValues, startTime.Ticks);
				reactiveAverageValues = Average<READ, WRITE>(reactiveValues, startTime.Ticks);

				dbHelper.AddRange<WRITE>(activeAverageValues);
				dbHelper.AddRange<WRITE>(reactiveAverageValues);
			}
		}

		/// <summary>
		/// Metoda koja je zaduzena za proracun prosecnih vrednosti po gidu u datom intervalu
		/// </summary>
		/// <typeparam name="READ"> Tip podatka tabele iz koje se citaju podaci </typeparam>
		/// <typeparam name="WRITE"> Tip podatka tabele u koju se upisuju podaci </typeparam>
		/// <param name="values"> Lista od koje se racuna prosecna vrednost </param>
		/// <param name="timestamp"> Za koji vremenski trenutak se racuna prosecna vrednost</param>
		/// <returns></returns>
		public List<WRITE> Average<READ, WRITE>(List<READ> values, long timestamp) where READ : HistorianItem where WRITE : HistorianItem
		{
			// Lista povratnih vrednosti sa prosecno izracunatim vrednostima za svaki globalni identifikator
			List<WRITE> average = new List<WRITE>();

			// Long je globalni identifikator i lista item-a vezanih za taj gid u odredjenom intervalu
			Dictionary<long, List<READ>> temp = new Dictionary<long, List<READ>>();

			// Prolazi se kroz listu vrednosti i vrsi se sortiranje po gid-u
			foreach (READ value in values)
			{
				if (!temp.ContainsKey(value.GlobalId))
				{
					temp.Add(value.GlobalId, new List<READ>());
				}

				temp[value.GlobalId].Add(value);
			}

			// Prolazi se kroz liste sortirane po gidu i vrsi se proracun srednje vrednosti
			foreach (List<READ> tempValue in temp.Values)
			{
				READ example = tempValue.FirstOrDefault();

				float averageValue = tempValue.Average(o => o.Value);
                float averageIncrease = tempValue.Average(o => o.Increase);
                float averageDecrease = tempValue.Average(o => o.Decrease);

				// Genericko kreiranje nove instance objekta i ubacivanje u litu povratnih vrednosti
				average.Add((WRITE)Activator.CreateInstance(typeof(WRITE), new object[] { averageValue, timestamp, example.GlobalId, example.Type, averageIncrease, averageDecrease }));
			}

			return average;
		}
	}
}
