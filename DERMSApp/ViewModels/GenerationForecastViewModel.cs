using CEDistributionProxy;
using CEForecastProxy;
using CommonCE;
using DERMSApp.Model;
using DERMSApp.Views;
using EventAlarmService;
using EventCollectorProxyNS;
using EventCommon;
using FTN.Common;
using FTN.Common.CalculationEngine.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using TSDBProxyNS;

namespace DERMSApp.ViewModels
{
	public class GenerationForecastViewModel : BindableBase
	{
		private long selectedDER;
		private string setPoint;
		private int selectedTime;
		private bool active;
		private string power;
		private bool isNominalChecked;
		private bool isReserveChecked;
		private PowerType powerType;
		private bool group;
		private string currentDate;

		private string powerOfObject;
		private string increaseForObject;
		private string decreaseForObject;
		private ObservableCollection<TableSMItem> DERS;
		private TableSMItem _der;

		TSDBProxy proxy = new TSDBProxy();
		ICEDistributionProxy CommandProxy = new ICEDistributionProxy();
		IForecastServiceProxy ForecastProxy = new IForecastServiceProxy();

		public ICommand IncreaseCommandValue { get; private set; }

		public ICommand DecreaseCommandValue { get; private set; }

		public ICommand ExecuteSetpointCommand { get; private set; }

		public ICommand StopSetpointCommand { get; private set; }

		public List<int> Durations { get; set; }

		private Command newCommand;

		public DateTime CurrentDateTime { get; set; }
		public DateTime MinDate { get; set; }
		public DateTime MaxDate { get; set; }

		private string meteringUnit;

		private SortedDictionary<long, float> Commands;

		bool executeEnabled;
		bool stopEnabled;

		#region Chart property

		private SeriesCollection forecastSeries_Y;
		public SeriesCollection ForecastSeries_Y
		{
			get
			{
				return forecastSeries_Y;
			}

			set
			{
				forecastSeries_Y = value;
				OnPropertyChanged("ForecastSeries_Y");
			}
		}

		private string[] forecastData_X;
		public string[] ForecastData_X
		{
			get
			{
				return forecastData_X;
			}

			set
			{
				forecastData_X = value;
				OnPropertyChanged("ForecastData_X");
			}
		}
		#endregion

		public GenerationForecastViewModel()
		{
		}

		public GenerationForecastViewModel(long selectedObjectGid, bool _active, bool isGroup, ObservableCollection<TableSMItem> _forecastDERS, TableSMItem _forecastDER)
		{
			#region Chart initialize
			ForecastSeries_Y = new SeriesCollection();

			ForecastSeries_Y.Add(
					new LineSeries
					{
						Title = "Max Increase",
						Values = new ChartValues<float>(),
						PointGeometry = DefaultGeometries.Circle,
						Opacity = 0,
						Stroke = Brushes.Green,
						Fill = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0)),
						LineSmoothness = 1
					});

			ForecastSeries_Y.Add(
					new LineSeries
					{
						Title = "Actual Power",
						Values = new ChartValues<float>(),
						PointGeometry = DefaultGeometries.Circle,
						Opacity = 0,
						Stroke = Brushes.Yellow,
						Fill = new SolidColorBrush(Color.FromArgb(190, 255, 0, 0)),
						LineSmoothness = 1

					});

			ForecastSeries_Y.Add(
					 new LineSeries
					 {
						 Title = "Max Decrease",
						 Values = new ChartValues<float>(),
						 PointGeometry = DefaultGeometries.Circle,
						 Opacity = 0,
						 Stroke = Brushes.Red,
						 Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#282828")),
						 LineSmoothness = 1
					 });
			#endregion Chart initialize

			SelectedDER = selectedObjectGid;
			SelectedTime = 1;
			IsNominalChecked = true;

			active = _active;
			if(_active)
			{
				meteringUnit = "kW";
			}
			else
			{
				meteringUnit = "kVAr";
			}

			group = isGroup;
			InitGUIElements();
			IncreaseCommandValue = new RelayCommand(() => IncreaseCommandValueExecute(), () => true);
			DecreaseCommandValue = new RelayCommand(() => DecreaseCommandValueExecute(), () => true);
			ExecuteSetpointCommand = new RelayCommand(() => ExecuteSetpointCommandExecute(), () => true);
			StopSetpointCommand = new RelayCommand(() => StopSetpointCommandExecute(), () => true);
			EventSystem.Subscribe<string>(NullifyCommand);

			DisplayPowerAndFlexibility(_forecastDERS, _forecastDER);
			LoadChartData();
			SortedDictionary<long, float> commands = CommandProxy.Proxy.GetApplaiedCommands(SelectedDER, powerType);
			CommandTimeLineData(commands, SelectedDER + "commands.xml");

		}

		private void InitGUIElements()
		{
			if (active)
			{
				Power = "Active Power";
				powerType = PowerType.Active;
			}
			else
			{
				Power = "Reactive Power";
				powerType = PowerType.Reactive;
			}
			SetPoint = "0";
			Durations = new List<int>();
			for (int i = 1; i < (24 - DateTime.Now.Hour); i++)
			{
				Durations.Add(i);
			}
			CurrentDate = String.Format("Date: {0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

			DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
			CurrentDateTime = DateTime.Now;
			MinDate = startTime.AddDays(-1);
			MaxDate = startTime.AddDays(1);

			bool commandExist = CommandProxy.Proxy.CommandExists(SelectedDER, powerType);
			ExecuteEnabled = !commandExist;
			StopEnabled = commandExist;
		}

		private void DisplayPowerAndFlexibility(ObservableCollection<TableSMItem> DERS, TableSMItem _der)
		{
			this.DERS = DERS;
			this._der = _der;

			new Thread(() =>
			{
				if (DERS != null)
				{
					float sumActualPower = 0;
					float sumIncrease = 0;
					float sumDecrease = 0;

					foreach (TableSMItem der in DERS)
					{
						if (powerType == PowerType.Active)
						{
							sumActualPower += der.CurrentP;
							sumIncrease += der.PIncrease;
							sumDecrease += der.PDecrease;
						}
						else
						{
							sumActualPower += der.CurrentQ;
							sumIncrease += der.QIncrease;
							sumDecrease += der.QDecrease;
						}
					}

					PowerOfObject = Math.Round(sumActualPower, 2).ToString();
					IncreaseForObject = Math.Round(sumIncrease - sumActualPower, 2).ToString();
					DecreaseForObject = Math.Abs(Math.Round(sumActualPower - sumDecrease, 2)).ToString();
				}
				if (_der != null)
				{
					if (powerType == PowerType.Active)
					{
						PowerOfObject = Math.Round(_der.CurrentP, 2).ToString();
						IncreaseForObject = Math.Round(_der.PIncrease - _der.CurrentP, 2).ToString();
						DecreaseForObject = Math.Abs(Math.Round(_der.CurrentP - _der.PDecrease, 2)).ToString();
					}
					else
					{
						PowerOfObject = Math.Round(_der.CurrentQ, 2).ToString();
						IncreaseForObject = Math.Round(_der.QIncrease - _der.CurrentQ, 2).ToString();
						DecreaseForObject = Math.Abs(Math.Round(_der.CurrentQ - _der.QDecrease, 2)).ToString();
					}
				}
			}).Start();
		}

		#region Load Chart Data
		private void LoadChartData()
		{
			new Thread(() => LoadChartDataThread()).Start();
		}

		private void LoadChartDataThread()
		{
			return;// to do weather forecast chart data vrati se 
			ForecastSeries_Y[0].Values.Clear();
			ForecastSeries_Y[1].Values.Clear();
			ForecastSeries_Y[2].Values.Clear();

			List<AnalogValue> past = new List<AnalogValue>();
			List<AnalogValue> forecast = new List<AnalogValue>();
			ForecastObject forecastGraph = new ForecastObject();

			DateTime dateTime = DateTime.Now;
			DateTime startDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			DateTime endDate = startDate.AddDays(1);

			if (SelectedDER == -1)
			{
				if (active)
				{
					past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Active);
				}
				else
				{
					past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Reactive);
				}
			}
			else
			{
				if (group)
				{
					forecastGraph = ForecastProxy.Proxy.CalculateHourlyForecastForGroup(SelectedDER);
					if (active)
					{
						past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Active, SelectedDER);
						forecast = forecastGraph.HourlyP;
					}
					else
					{
						past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Reactive, SelectedDER);
						forecast = forecastGraph.HourlyQ;
					}
				}
				else
				{
					forecastGraph = ForecastProxy.Proxy.HourlyForecastForDer(SelectedDER);
					if (active)
					{
						past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Active, SelectedDER);
						forecast = forecastGraph.HourlyP;
					}
					else
					{
						past = proxy.Proxy.GetDailyConsuption(DateTime.Now.Ticks, FTN.Common.PowerType.Reactive, SelectedDER);
						forecast = forecastGraph.HourlyQ;
					}
				}
			}

			ForecastData_X = new string[past.Count + forecast.Count];
			int x_counter = 0;

			foreach (AnalogValue av in past)
			{
				if (av.Timestamp < DateTime.Now.Ticks)
				{
					DateTime Time = new DateTime(av.Timestamp);

					ForecastData_X[x_counter++] = Time.ToShortTimeString();
					ForecastSeries_Y[0].Values.Add(av.PowIncrease);
					ForecastSeries_Y[1].Values.Add(av.Value);
					ForecastSeries_Y[2].Values.Add(av.PowDecrease);
				}
			}

			DateTime current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

			ForecastData_X[x_counter++] = current.ToShortTimeString();
			ForecastSeries_Y[0].Values.Add(float.Parse(PowerOfObject) + float.Parse(IncreaseForObject));
			ForecastSeries_Y[1].Values.Add(float.Parse(PowerOfObject));
			ForecastSeries_Y[2].Values.Add(float.Parse(PowerOfObject) - float.Parse(DecreaseForObject));

			foreach (AnalogValue av in forecast)
			{
				if ((av.Timestamp > current.Ticks) && (av.Timestamp < endDate.Ticks))
				{
					DateTime Time = new DateTime(av.Timestamp);

					ForecastData_X[x_counter++] = Time.ToShortTimeString();
					ForecastSeries_Y[0].Values.Add(av.PowIncrease);
					ForecastSeries_Y[1].Values.Add(av.Value);
					ForecastSeries_Y[2].Values.Add(av.PowDecrease);
				}
			}
		}
		#endregion

		private void StopSetpointCommandExecute()
		{
		  
			NullifyCommand("Nullify");
		}

		private void ExecuteSetpointCommandExecute()
		{
			float result = 0;
			DialogBox dBox = null;

			if (!float.TryParse(SetPoint, out result))
			{
				dBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Delta value must be a number.", 3));
				ShowCenteredDialog(dBox);
				return;
			}
			else
			{
				if (result == 0)
				{
					dBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Delta value must be a either positive or negative number.", 3));
					ShowCenteredDialog(dBox);
					return;
				}
				else if (float.Parse(DecreaseForObject) < Math.Abs(result) && result < 0)
				{
					dBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Delta value can not have value greater than maximum decrease.", 3));
					ShowCenteredDialog(dBox);
					return;
				}
				else if (float.Parse(IncreaseForObject) < Math.Abs(result) && result > 0)
				{
					dBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Delta value can not have value greater than maximum increase.", 3));
					ShowCenteredDialog(dBox);
					return;
				}	
			
				switch (IsNominalChecked)
				{
					case true:
						newCommand = new Command(SelectedDER, float.Parse(SetPoint), powerType, SelectedTime, OptimizationType.NominalPower);
						break;
					case false:
						newCommand = new Command(SelectedDER, float.Parse(SetPoint), powerType, SelectedTime, OptimizationType.DerFlexibility);
						break;
					default:
						break;
				}

				CheckAndExecuteCommand(newCommand);
			}
		}

		private void ShowCenteredDialog(DialogBox dialogBox)
		{
			dialogBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			dialogBox.ShowDialog();
		}

		/// <summary>
		/// Ova se metoda pozove kad se radi "Execute". To do to do vrati se ovde za event. bitno
		/// </summary>
		/// <param name="newCommand"></param>
		private void CheckAndExecuteCommand(Command newCommand)
		{
			if (CommandProxy.Proxy.CommandExists(SelectedDER, powerType))
			{
				var dialogBox = new DialogBox(new DialogBoxViewModel("Warning!", true, "Previous command for this object needs to be nullified before adding a new one!", 2));
				dialogBox.ShowDialog();
			}
			else
			{
				Semaphore localSemapthore = new Semaphore(0, 1);

				//to do odavde sam prvobitno hito da posaljem event
				new Thread(() =>
				{
					if (CommandProxy.Proxy.DistributePowerClient(newCommand))
					{
						bool commandExist = CommandProxy.Proxy.CommandExists(SelectedDER, powerType);
						ExecuteEnabled = !commandExist;
						StopEnabled = commandExist;

						localSemapthore.Release();

						//Thread.Sleep(2000);
						//LoadChartData();
						//DisplayPowerAndFlexibility(this.DERS, this._der);
					}
					else
					{
						localSemapthore.Release();

						var dialogBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Error when executing command!", 3));
						dialogBox.ShowDialog();
					}
				}).Start();

				localSemapthore.WaitOne(10000);

				DateTime current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

				SortedDictionary<long, float> commands = CommandProxy.Proxy.GetApplaiedCommands(SelectedDER, powerType);

				IncreaseForObject = (Math.Round(float.Parse(IncreaseForObject) - commands[current.Ticks], 2)).ToString();
				DecreaseForObject = (Math.Round(float.Parse(DecreaseForObject) + commands[current.Ticks], 2)).ToString();
			 
				PowerOfObject = (Math.Round((float.Parse(PowerOfObject) + commands[current.Ticks]), 2)).ToString();

				LoadChartData();

				CommandTimeLineData(commands, SelectedDER + "commands.xml");
			}
		}

		private void NullifyCommand(string toNullify)
		{
			bool result = false;
			SortedDictionary<long, float> commands = CommandProxy.Proxy.GetApplaiedCommands(SelectedDER, powerType);

			if (toNullify.Equals("Nullify"))
			{
				Semaphore localSemapthore = new Semaphore(0, 1);

				new Thread(() =>
				{
					result = CommandProxy.Proxy.CancelCommand(SelectedDER, powerType);

					localSemapthore.Release();

					if (result == false)
					{
						var dialogBox = new DialogBox(new DialogBoxViewModel("Error!", true, "Error when nullifying command!", 3));
						dialogBox.ShowDialog();

						bool commandExist = CommandProxy.Proxy.CommandExists(SelectedDER, powerType);
						ExecuteEnabled = !commandExist;
						StopEnabled = commandExist;
					}

				   
				   // DisplayPowerAndFlexibility(this.DERS, this._der);
				}).Start();

				localSemapthore.WaitOne(10000);

				if (result)
				{
					DateTime current = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

					IncreaseForObject = Math.Round((float.Parse(IncreaseForObject) + commands[current.Ticks]), 2).ToString();
					DecreaseForObject = Math.Round((float.Parse(DecreaseForObject) - commands[current.Ticks]), 2).ToString();

					PowerOfObject = Math.Round((float.Parse(PowerOfObject) - commands[current.Ticks]), 2).ToString();

					LoadChartData();

					bool commandExist = CommandProxy.Proxy.CommandExists(SelectedDER, powerType);
					ExecuteEnabled = !commandExist;
					StopEnabled = commandExist;
				}

				SortedDictionary<long, float> newcommands = CommandProxy.Proxy.GetApplaiedCommands(SelectedDER, powerType);
				CommandTimeLineData(newcommands, SelectedDER + "commands.xml");
			}
		}

		private void DecreaseCommandValueExecute()
		{
			if (float.Parse(SetPoint) > 0)
			{
				SetPoint = (float.Parse(SetPoint) - 1).ToString();
			}
		}

		private void IncreaseCommandValueExecute()
		{
			SetPoint = (float.Parse(SetPoint) + 1).ToString();
		}

		//SelectedDER+"commands.xml"
		private void CommandTimeLineData(SortedDictionary<long, float> commands, string fileName)
		{
			DateTime dateTime;

			if (commands == null)
			{
				CommonTrace.WriteTrace(true,"Commands are null in GenerationForocastViewModel");
				return;
			}

			Commands = commands;

			using (XmlWriter writer = XmlWriter.Create(fileName))
			{
				writer.WriteStartElement("data");
				foreach (KeyValuePair<long, float> kvp in Commands)
				{
					dateTime = new DateTime(kvp.Key);
					writer.WriteStartElement("event");
					writer.WriteAttributeString("start", new DateTime(kvp.Key).ToString());
					writer.WriteAttributeString("title", String.Format("Demanded power: {0}{1}, {2}", kvp.Value, meteringUnit, new DateTime(kvp.Key).ToShortTimeString()));
					if (CurrentDateTime >= dateTime && CurrentDateTime < dateTime.AddHours(1))
					{
						writer.WriteAttributeString("color", "Red");
					}
					else if (dateTime < CurrentDateTime)
					{
						writer.WriteAttributeString("color", "Grey");
					}
					else if (dateTime > CurrentDateTime)
					{
						writer.WriteAttributeString("color", "RoyalBlue");
					}
					writer.WriteString(String.Format("Element with global ID: {0}, Demanded power: {1}"+ meteringUnit, SelectedDER, kvp.Value));
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				writer.Flush();
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			var declarations = doc.ChildNodes.OfType<XmlNode>().Where(x => x.NodeType == XmlNodeType.XmlDeclaration).ToList();

			declarations.ForEach(x => doc.RemoveChild(x));

			doc.Save(fileName);

			EventSystem.Publish<XmlDocument>(doc);
		}

		#region Properties
		public long SelectedDER
		{
			get
			{
				return selectedDER;
			}

			set
			{
				selectedDER = value;
				OnPropertyChanged("SelectedDER");
			}
		}

		public string SetPoint
		{
			get
			{
				return setPoint;
			}

			set
			{
				setPoint = value;
				OnPropertyChanged("SetPoint");
			}
		}

		public int SelectedTime
		{
			get
			{
				return selectedTime;
			}

			set
			{
				selectedTime = value;
				OnPropertyChanged("SelectedTime");
			}
		}

		public string Power
		{
			get
			{
				return power;
			}

			set
			{
				power = value;
				OnPropertyChanged("Power");
			}
		}

		public bool IsNominalChecked
		{
			get
			{
				return isNominalChecked;
			}

			set
			{
				if (isNominalChecked != value)
				{
					isNominalChecked = value;
					OnPropertyChanged("IsNominalChecked");
				}
			}
		}

		public bool IsReserveChecked
		{
			get
			{
				return isReserveChecked;
			}

			set
			{
				if (isReserveChecked != value)
				{
					isReserveChecked = value;
					OnPropertyChanged("IsReserveChecked");
				}
			}
		}

		public string PowerOfObject
		{
			get
			{
				return powerOfObject;
			}

			set
			{
				powerOfObject = value;
				OnPropertyChanged("PowerOfObject");
			}
		}

		public string IncreaseForObject
		{
			get
			{
				return increaseForObject;
			}

			set
			{
				increaseForObject = value;
				OnPropertyChanged("IncreaseForObject");
			}
		}

		public string DecreaseForObject
		{
			get
			{
				return decreaseForObject;
			}

			set
			{
				decreaseForObject = value;
				OnPropertyChanged("DecreaseForObject");
			}
		}

		public string CurrentDate
		{
			get
			{
				return currentDate;
			}

			set
			{
				currentDate = value;
				OnPropertyChanged("CurrentDate");
			}
		}

		public bool ExecuteEnabled
		{
			get
			{
				return executeEnabled;
			}

			set
			{
				executeEnabled = value;
				OnPropertyChanged("ExecuteEnabled");
			}
		}

		public bool StopEnabled
		{
			get
			{
				return stopEnabled;
			}

			set
			{
				stopEnabled = value;
				OnPropertyChanged("StopEnabled");
			}
		}

		#endregion Properties
	}
}

