using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using DERMSApp.Model;
using System.Windows;
using System.Windows.Input;
using FTN.Common.WeatherForecast.Model;
using WeatherForecastProxyNS;
using FTN.Common;
using System.Threading;
using FTN.Common.Services;
using System.ServiceModel;
using DERMSApp.Views;
using Adapter;
using FTN.Common.AORCachedModel;
using EventCommon;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// Uvezan je sa tabular view, nije logicno ali tako je.U sustini glavni view model.
	/// </summary>
	public class EntireNetworkViewModel : ViewModelBase, IDeltaNotifyCallback, IDERMSEventSubscriptionCallback
	{
		#region Fields
		//readonly ReadOnlyCollection<GeographicalRegionViewModel> _regions;
		private ObservableCollection<TableSMItem> _ders;
		private List<NetworkRootViewModel> _roots;
		CIMAdapter adapter = new CIMAdapter();
		private BindableBase historyDataChartVM;
		private BindableBase generationForecastVM;
		private BindableBase eventsVM;
		private BindableBase alarmsVM;
		private WeatherInfo weather;
		private string weatherIcon;
		private ObservableCollection<TableSMItem> dersToSend = null;
		private TableSMItem derToSend = null;
		private string timeStamp;
		private List<AORCachedArea> aorAreas;

		//WeatherForecastProxy weatherProxy = new WeatherForecastProxy(); //// to do vrati weather

		#region Visibility
		/// <summary>
		/// Visibility of Tabular Data
		/// </summary>
		private Visibility showData;
		private Visibility showCharts;
		private Visibility showForecast;
		private Visibility showEvents;
		private Visibility showAlarms;

		private Visibility weatherWidgetVisible;
		#endregion Visibility
		#region Commands
		/// <summary>
		/// Simple property to hold the 'ShowTableCommand' - when executed
		/// it will change the current view to the 'Table Data'
		/// </summary>
		public ICommand ShowTableCommand { get; private set; }

		/// <summary>
		/// Simple property to hold the 'ShowChartCommand' - when executed
		/// it will change the current view to the 'History Chart'
		/// </summary>
		public ICommand ShowChartCommand { get; private set; }

		public ICommand ShowEventsCommand { get; private set; }

		public ICommand ShowAlarmsCommand { get; private set; }


		/// <summary>
		/// Simple property to hold the 'ShowForecastCommand' - when executed
		/// it will change the current view to the 'Forecast'
		/// </summary>
		public ICommand ShowForecastCommand { get; private set; }

		public ICommand FilterCommand { get; private set; }
		public ICommand SearchCommand { get; private set; }
		#endregion Commands
		public List<string> TypesForFilter { get; set; }

		private string filterButton;
		private string filterType;
		private bool isFilterEnabled;

		private string searchButton;
		private string searchName;
		private bool isSearchEnabled;

		private List<TableSMItem> tempList = new List<TableSMItem>();

		private double activeMinimum;
		private double activeMaximum;
		private double activeValue;

		private double reactiveMinimum;
		private double reactiveMaximum;
		private double reactiveValue;

		private double activeShareSun;

		private RDAdapter rdAdapter = new RDAdapter();

		private Visibility gaugesVisibility;

		private long selectedGid;
		#endregion Fields
		public EntireNetworkViewModel(List<AORCachedArea> aorAreas)
		{
			this.aorAreas = aorAreas;
			_ders = new ObservableCollection<TableSMItem>();
			_roots = new List<NetworkRootViewModel>();
			_roots.Add(new NetworkRootViewModel(_ders, aorAreas));

			SetAllVisibilitiesToCollapsed();
			ShowData = Visibility.Visible;
			WeatherWidgetVisible = Visibility.Hidden;

			InitializeCommands();
			SubscribeToEverything();

			EventsVM = new EventsViewModel();//to do bice prebaceno odmah da se instancira, da cuva podatke o events 25.7.

			SubscribeForEvents();// to do vratiti se i konvertovati areas u stringove

			dersToSend = null;
			derToSend = null;
			selectedGid = -1;

			InitializeFilters();
			ConnectToCalculationEngine();
		}

		private void InitializeCommands()
		{
			ShowTableCommand = new RelayCommand(() => ExecuteShowTableCommand());
			ShowChartCommand = new RelayCommand(() => ExecuteShowChartCommand());
			ShowEventsCommand = new RelayCommand(() => ExecuteShowEventsCommand());
			ShowAlarmsCommand = new RelayCommand(() => ExecuteShowAlarmsCommand());

			FilterCommand = new RelayCommand(() => ExecuteFilterCommand());
			SearchCommand = new RelayCommand(() => ExecuteSearchCommand());
			//ShowForecastCommand = new RelayCommand(() => ExecuteShowForecastCommand());
		}

		private void InitializeFilters()
		{
			TypesForFilter = new List<string>();
			TypesForFilter.Add("Sun");
			TypesForFilter.Add("Wind");
			FilterButton = "Filter";
			SearchButton = "Search";
			IsSearchEnabled = true;
			IsFilterEnabled = true;
		}

		private void SubscribeToEverything()
		{
			EventSystem.Subscribe<string>(ShowTable);
			EventSystem.Subscribe<long>(ObjectSelected);
			EventSystem.Subscribe<DateTime>(DisplayLastDateTime);
			EventSystem.Subscribe<ObservableCollection<TableSMItem>>(DisplayPowerAndFlexibility);
			EventSystem.Subscribe<TableSMItem>(DisplayPowerAndFlexibility);
			EventSystem.Subscribe<ForecastObjData>(ForecastForObject);
		}

		/// <summary>
		/// Subscribe for event from EventAlarmService.
		/// </summary>
		private void SubscribeForEvents()
		{
			//DERMSEventClientCallback callback = new DERMSEventClientCallback();
			IDERMSEventSubscription proxy = null;

			DuplexChannelFactory<IDERMSEventSubscription> factory = new DuplexChannelFactory<IDERMSEventSubscription>(
				new InstanceContext(this),
				new NetTcpBinding(),
				new EndpointAddress("net.tcp://localhost:10047/IDERMSEvent"));
			proxy = factory.CreateChannel();

			proxy.Subscribe(new List<string>(1) { "a" });//hardcoded subscribe
		}

		/// <summary>
		/// Optimize later.
		/// </summary>
		private void SetAllVisibilitiesToCollapsed()
		{
			ShowCharts = Visibility.Collapsed;
			ShowForecast = Visibility.Collapsed;
			ShowData = Visibility.Collapsed;
			ShowEvents = Visibility.Collapsed;
			ShowAlarms = Visibility.Collapsed;
		}

		#region Properties
		public List<NetworkRootViewModel> Roots // zasto je ovo lista?
		{
			get { return _roots; }
			set
			{
				_roots = value;
				RaisePropertyChanged("Roots");
			}
		}

		public ObservableCollection<TableSMItem> DERS
		{

			get { return _ders; }
			set
			{
				_ders = value;
				RaisePropertyChanged("DERS");
			}
		}

		public List<AORCachedArea> Areas
		{
			get { return aorAreas; }
			set
			{
				aorAreas = value;
				RaisePropertyChanged("Areas");
			}
		}

		public Visibility ShowCharts
		{
			get { return showCharts; }
			set
			{
				showCharts = value;
				RaisePropertyChanged("ShowCharts");
			}
		}

		public Visibility ShowData
		{
			get { return showData; }
			set
			{
				showData = value;
				RaisePropertyChanged("ShowData");
			}
		}

		public Visibility ShowEvents
		{
			get { return showEvents; }
			set
			{
				showEvents = value;
				RaisePropertyChanged("ShowEvents");
			}
		}

		public Visibility ShowAlarms
		{
			get { return showAlarms; }
			set
			{
				showAlarms = value;
				RaisePropertyChanged("ShowAlarms");
			}
		}


		public Visibility WeatherWidgetVisible
		{
			get
			{
				return weatherWidgetVisible;
			}

			set
			{
				weatherWidgetVisible = value;
				RaisePropertyChanged("WeatherWidgetVisible");
			}
		}

		public Visibility ShowForecast
		{
			get
			{
				return showForecast;
			}

			set
			{
				showForecast = value;
				RaisePropertyChanged("ShowForecast");
			}
		}

		public BindableBase HistoryDataChartVM
		{
			get { return historyDataChartVM; }
			set
			{
				if (historyDataChartVM == value)
					return;
				historyDataChartVM = value;
				RaisePropertyChanged("HistoryDataChartVM");
			}
		}

		public BindableBase EventsVM
		{
			get { return eventsVM; }
			set
			{
				if (eventsVM == value)
					return;
				eventsVM = value;
				RaisePropertyChanged("EventsVM");
			}
		}

		public BindableBase AlarmsVM
		{
			get { return alarmsVM; }
			set
			{
				if (alarmsVM == value)
					return;
				alarmsVM = value;
				RaisePropertyChanged("AlarmsVM");
			}
		}

		public WeatherInfo Weather
		{
			get { return weather; }
			set
			{
				weather = value;
				RaisePropertyChanged("Weather");
			}
		}

		public string WeatherIcon
		{
			get
			{
				return weatherIcon;
			}

			set
			{
				weatherIcon = value;
				RaisePropertyChanged("WeatherIcon");
			}
		}

		public BindableBase GenerationForecastVM
		{
			get
			{
				return generationForecastVM;
			}

			set
			{
				if (generationForecastVM == value)
					return;
				generationForecastVM = value;
				RaisePropertyChanged("GenerationForecastVM");
			}
		}

		public string TimeStamp
		{
			get
			{
				return timeStamp;
			}

			set
			{
				if (timeStamp == value)
					return;
				timeStamp = value;
				RaisePropertyChanged("TimeStamp");
			}
		}

		public string FilterButton
		{
			get
			{
				return filterButton;
			}

			set
			{
				filterButton = value;
				RaisePropertyChanged("FilterButton");
			}
		}

		public string FilterType
		{
			get
			{
				return filterType;
			}

			set
			{
				filterType = value;
				RaisePropertyChanged("FilterType");
			}
		}

		public string SearchButton
		{
			get
			{
				return searchButton;
			}

			set
			{
				searchButton = value;
				RaisePropertyChanged("SearchButton");
			}
		}

		public string SearchName
		{
			get
			{
				return searchName;
			}

			set
			{
				searchName = value;
				RaisePropertyChanged("SearchName");
			}
		}

		public bool IsFilterEnabled
		{
			get
			{
				return isFilterEnabled;
			}

			set
			{
				isFilterEnabled = value;
				RaisePropertyChanged("IsFilterEnabled");
			}
		}

		public bool IsSearchEnabled
		{
			get
			{
				return isSearchEnabled;
			}

			set
			{
				isSearchEnabled = value;
				RaisePropertyChanged("IsSearchEnabled");
			}
		}

		public double ActiveMinimum
		{
			get
			{
				return activeMinimum;
			}

			set
			{
				activeMinimum = value;
				RaisePropertyChanged("ActiveMinimum");
			}
		}

		public double ActiveMaximum
		{
			get
			{
				return activeMaximum;
			}

			set
			{
				activeMaximum = value;
				RaisePropertyChanged("ActiveMaximum");
			}
		}

		public double ActiveValue
		{
			get
			{
				return activeValue;
			}

			set
			{
				activeValue = value;
				RaisePropertyChanged("ActiveValue");
			}
		}

		public double ReactiveMinimum
		{
			get
			{
				return reactiveMinimum;
			}

			set
			{
				reactiveMinimum = value;
				RaisePropertyChanged("ReactiveMinimum");
			}
		}

		public double ReactiveMaximum
		{
			get
			{
				return reactiveMaximum;
			}

			set
			{
				reactiveMaximum = value;
				RaisePropertyChanged("ReactiveMaximum");
			}
		}

		public double ReactiveValue
		{
			get
			{
				return reactiveValue;
			}

			set
			{
				reactiveValue = value;
				RaisePropertyChanged("ReactiveValue");
			}
		}

		public double ActiveShareSun
		{
			get
			{
				return activeShareSun;
			}

			set
			{
				activeShareSun = value;
				RaisePropertyChanged("ActiveShareSun");
			}
		}

		public Visibility GaugesVisibility
		{
			get
			{
				return gaugesVisibility;
			}

			set
			{
				gaugesVisibility = value;
				RaisePropertyChanged("GaugesVisibility");
			}
		}

		private void ShowTable(string command)
		{
			if (command.Equals("ShowTable"))
			{
				ExecuteShowTableCommand();
			}
		}
		#endregion

		#region Private methods
		private void ExecuteFilterCommand()
		{
			if (tempList.Count == 0)
			{
				foreach (TableSMItem item in DERS)
				{
					tempList.Add(item);
				}
				DERS.Clear();
				foreach (TableSMItem item in tempList)
				{
					if (FilterType.Equals("Sun"))
					{
						if (item.Der.FuelType == FTN.Common.FuelType.Sun)
						{
							DERS.Add(item);
						}
					}
					else if (FilterType.Equals("Wind"))
					{
						if (item.Der.FuelType == FTN.Common.FuelType.Wind)
						{
							DERS.Add(item);
						}
					}
					else
					{

					}
				}
				FilterButton = "Cancel Filter";
				IsSearchEnabled = false;
			}
			else
			{
				DERS.Clear();
				foreach (TableSMItem item in tempList)
				{
					DERS.Add(item);
				}
				tempList.Clear();
				FilterButton = "Filter";
				IsSearchEnabled = true;
			}

			ShowGauges();
		}

		private void ExecuteSearchCommand()
		{
			if (tempList.Count == 0)
			{
				foreach (TableSMItem item in DERS)
				{
					tempList.Add(item);
				}
				DERS.Clear();
				foreach (TableSMItem item in tempList)
				{
					if (item.Der.Name.Contains(SearchName) || item.Der.Name.ToLower().Contains(SearchName))
					{
						DERS.Add(item);
					}
				}
				SearchButton = "Cancel Search";
				IsFilterEnabled = false;
			}
			else
			{
				DERS.Clear();
				foreach (TableSMItem item in tempList)
				{
					DERS.Add(item);
				}
				tempList.Clear();
				SearchButton = "Search";
				IsFilterEnabled = true;
			}

			ShowGauges();
		}

		/// <summary>
		/// Set the visibility of DataGrid to 'Visible'
		/// </summary>
		private void ExecuteShowTableCommand()
		{
			SetAllVisibilitiesToCollapsed();
			ShowData = Visibility.Visible;
		}

		/// <summary>
		/// Set the visibility of ContentControl to 'Visible'
		/// </summary>
		private void ExecuteShowChartCommand()
		{
			HistoryDataChartVM = new HistoryDataChartViewModel(selectedGid);
			SetAllVisibilitiesToCollapsed();
			ShowCharts = Visibility.Visible;
		}
		private void ExecuteShowEventsCommand()
		{
			//EventsVM = new EventsViewModel();//to do bice prebaceno odmah da se instancira, da cuva podatke o events 25.7.
			SetAllVisibilitiesToCollapsed();
			ShowEvents = Visibility.Visible;
		}

		private void ExecuteShowAlarmsCommand()
		{
			SetAllVisibilitiesToCollapsed();
			ShowAlarms = Visibility.Visible;
		}

		private void DisplayLastDateTime(DateTime lastDateTime)
		{
			TimeStamp = lastDateTime.ToString();
		}

		private void DisplayPowerAndFlexibility(ObservableCollection<TableSMItem> _forecastders)
		{
			dersToSend = _forecastders;
			derToSend = null;
		}

		private void DisplayPowerAndFlexibility(TableSMItem _forecastder)
		{
			derToSend = _forecastder;
			dersToSend = null;
		}

		/// <summary>
		/// Ovo se pozove kad se odradi EventSystem.Publish<ForecastObjData>. U forecast obj data sam dodao naziv regiona.
		/// </summary>
		/// <param name="d"></param>
		private void ForecastForObject(ForecastObjData d)
		{
			GenerationForecastVM = new GenerationForecastViewModel(d.Gid, d.Power, d.IsGroup, dersToSend, derToSend);
			EventSystem.Publish<bool>(true);
			SetAllVisibilitiesToCollapsed();
			ShowForecast = Visibility.Visible;
		}

		#endregion Private methods

		#region Public methods
		public void ObjectSelected(long gid) //// to do vrati weather
		{
			selectedGid = gid;

			//Ako se nalazimo na istorijskom dijagramo izmeni podatke... radi tree view
			if (ShowCharts == Visibility.Visible)
			{
				HistoryDataChartVM = new HistoryDataChartViewModel(selectedGid);
			}

			new Thread(() =>
			{
				if (gid != -1)
				{
					WeatherInfo tempWeather;
					try
					{
						//tempWeather = weatherProxy.Proxy.GetCurrentWeatherDataByGlobalId(gid); // to do vrati weather
						tempWeather = new WeatherInfo();
					}
					catch (Exception e)
					{
						throw e;
					}

					tempWeather.Currently.Temperature = Math.Round(tempWeather.Currently.Temperature, 2);
					tempWeather.Currently.WindSpeed = Math.Round(tempWeather.Currently.WindSpeed, 2);
					tempWeather.Currently.Humidity = Math.Round(tempWeather.Currently.Humidity, 2);
					tempWeather.Currently.Pressure = Math.Round(tempWeather.Currently.Pressure, 2);

					if (tempWeather.Currently.Summary.ToUpper().Equals("CLEAR"))
					{
						WeatherIcon = @"../Images/WeatherConditionsSunny.png";
					}
					else if (tempWeather.Currently.Summary.ToUpper().Equals("OVERCAST") || tempWeather.Currently.Summary.ToUpper().Contains("CLOUD"))
					{
						WeatherIcon = @"../Images/WeatherConditionsOvercast.png";
					}
					else if (tempWeather.Currently.Summary.ToUpper().Contains("RAIN") || tempWeather.Currently.Summary.ToUpper().Contains("DRIZZLE"))
					{
						WeatherIcon = @"../Images/WeatherConditionsRain.png";
					}
					else
					{
						WeatherIcon = @"../Images/WindIcon.png";
					}

					Weather = tempWeather;

					WeatherWidgetVisible = Visibility.Visible;

					new Thread(() => ShowGauges()).Start();

				}
				else
				{
					WeatherWidgetVisible = Visibility.Hidden;
					new Thread(() => ShowGauges()).Start();
				}
			}).Start();
		}

		public void ShowGauges()
		{
			Thread.Sleep(1000);

			int counter = 0;
			while (true)
			{
				if (DERS.Count != 0)
				{
					break;
				}
				else
				{
					if (counter == 5)
					{
						break;
					}

					counter++;
					Thread.Sleep(1000);
				}
			}

			ActiveMinimum = Math.Round(DERS.Sum(o => o.PDecrease), 2);
			ActiveMaximum = Math.Round(DERS.Sum(o => o.PIncrease), 2);
			ActiveValue = Math.Round(DERS.Sum(o => o.CurrentP), 2);

			ReactiveMinimum = Math.Round(DERS.Sum(o => o.QDecrease), 2);
			ReactiveMaximum = Math.Round(DERS.Sum(o => o.QIncrease), 2);
			ReactiveValue = Math.Round(DERS.Sum(o => o.CurrentQ), 2);

			double SunPower = DERS.Where(o => o.Der.FuelType.Equals(FuelType.Sun)).ToList().Sum(o => o.CurrentP);

			if (ActiveValue != 0)
			{
				ActiveShareSun = Math.Round((SunPower / ActiveValue) * 100, 2);
				if (ActiveShareSun > 100)
				{
					ActiveShareSun = 100;
				}
			}
			else
			{
				ActiveShareSun = 0;
			}

			GaugesVisibility = Visibility.Visible;
		}

		#region IDeltaNotifyCallback implementation
		public void Refresh()
		{
			Roots = new List<NetworkRootViewModel>() { new NetworkRootViewModel(_ders) };
			Roots[0].IsExpanded = false;

			var dialogBox = new DialogBox(new DialogBoxViewModel("Info!", true, "New delta applied.", 1));
			dialogBox.ShowDialog();
			//Roots.Add(new NetworkRootViewModel(_ders));
		}

		#endregion IDeltaNotifyCallback implementation

		DuplexChannelFactory<IDeltaNotify> factory = null;
		IDeltaNotify proxy = null;

		/// <summary>
		/// Ne znam od kad je zakomentarisano.
		/// </summary>
		public void ConnectToCalculationEngine()
		{
			//factory = new DuplexChannelFactory<IDeltaNotify>(
			//  new InstanceContext(this),
			//  new NetTcpBinding(),
			//  new EndpointAddress("net.tcp://localhost:10017/IDeltaNotify"));
			//proxy = factory.CreateChannel();
			//proxy.Register();
		}

		public void ReceiveEvent(Event e)
		{
			EventSystem.Publish<Event>(e);
		}

		#endregion Public methods
	}
}
