using Adapter;
using DERMSApp.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TSDBProxyNS;

namespace DERMSApp.ViewModels
{
    public class HistoryDataChartViewModel : BindableBase
    {
        TSDBProxy proxy = new TSDBProxy();

        public ICommand ShowChartCommand { get; private set; }

        private bool isHourlyChecked;

        private bool isDailyChecked;

        private bool isMonthlyChecked;

        private DateTime selectedDate;

        private int year;

        private string month;

        private bool hoursChosen;
 
        public List<int> Years { get; set; }

        public List<string> Months { get; set; }

        #region Active chart data
        private string[] historianDataActive_X;
        private SeriesCollection historianDataActive_Y;

        public SeriesCollection HistorianDataActive_Y
        {
            get
            {
                return historianDataActive_Y;
            }

            set
            {
                historianDataActive_Y = value;
                OnPropertyChanged("HistorianDataActive_Y");
            }
        }

        public string[] HistorianDataActive_X
        {
            get
            {
                return historianDataActive_X;
            }

            set
            {
                historianDataActive_X = value;
                OnPropertyChanged("HistorianDataActive_X");
            }
        }
        #endregion

        #region Reaktive chart data
        private string[] historianDataReactive_X;
        private SeriesCollection historianDataReactive_Y;

        public SeriesCollection HistorianDataReactive_Y
        {
            get
            {
                return historianDataReactive_Y;
            }

            set
            {
                historianDataReactive_Y = value;
                OnPropertyChanged("HistorianDataReactive_Y");
            }
        }

        public string[] HistorianDataReactive_X
        {
            get
            {
                return historianDataReactive_X;
            }

            set
            {
                historianDataReactive_X = value;
                OnPropertyChanged("HistorianDataReactive_X");
            }
        }
        #endregion

        private RDAdapter rdAdapter = new RDAdapter();

        private long selectedDER;

        private string selectedObject;

        private string timeReference;

        private Visibility hourlyChosen;

        private Visibility dailyChosen;

        private Visibility monthlyChosen;

        public HistoryDataChartViewModel()
        {
        }

        public HistoryDataChartViewModel(long selectedObjectGid)
        {
            //New active chart data
            HistorianDataActive_Y = new SeriesCollection();
            HistorianDataActive_Y.Add(
                new LineSeries
                {
                    Values = new ChartValues<float>(),
                    LineSmoothness = 1,
                    Stroke = Brushes.Blue
                }
            );

            //New reactive chart data
            HistorianDataReactive_Y = new SeriesCollection();
            HistorianDataReactive_Y.Add(
                new LineSeries
                {
                    Values = new ChartValues<float>(),
                    LineSmoothness = 1,
                    Stroke = Brushes.Red
                }
            );

            InitGUIElements();
            SelectedDER = selectedObjectGid;
            ShowChartCommand = new RelayCommand(() => ShowChartCommandExecute(), () => true);
           

            if (selectedObjectGid == -1)
            {
                SelectedObject = "Entire network";
            }
            else
            {
                switch (((DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(selectedObjectGid)))
                {
                    case DMSType.REGION:
                        SelectedObject = rdAdapter.GetRegionByGid(selectedObjectGid).Name;
                        break;
                    case DMSType.SUBREGION:
                        SelectedObject = rdAdapter.GetSubRegionByGid(selectedObjectGid).Name;
                        break;
                    case DMSType.SUBSTATION:
                        SelectedObject = rdAdapter.GetSubstation(selectedObjectGid).Name;
                        break;
                    case DMSType.SYNCMACHINE:
                        SelectedObject = rdAdapter.GetSyncMachineByGid(selectedObjectGid).Name;
                        break;
                }
            }

            Year = DateTime.Now.Year;
            Month = Months[DateTime.Now.Month - 1];
        }

        private void InitGUIElements()
        {
            Years = new List<int>();
            for (int i = 2000; i <= DateTime.Now.Year; i++)
            {
                Years.Add(i);
            }

            Months = new List<string>();
            string[] monthsArray = new DateTimeFormatInfo().MonthNames;
            for (int i = 0; i < monthsArray.Count(); i++)
            {
                Months.Add(monthsArray[i]);
            }

            IsHourlyChecked = true;
            IsDailyChecked = false;
            IsMonthlyChecked = false;
            SelectedDate = DateTime.Now;
        }

        private void ShowChartCommandExecute()
        {
            new Thread(() => ShowChartForActivePower()).Start();
            new Thread(() => ShowChartForReactivePower()).Start();
        }

        private void ShowChartForActivePower()
        {
            List<AnalogValue> ChartDataActive = new List<AnalogValue>();

            HistorianDataActive_Y[0].Values.Clear();

            DateTime chosenTime;

            if (IsHourlyChecked)
            {
                TimeReference = SelectedDate.ToShortDateString();
                chosenTime = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, 0, 0, 0);
                GetHourGraphData(chosenTime, SelectedDER, PowerType.Active, out ChartDataActive);
            }
            else if(IsDailyChecked)
            {
                TimeReference = Month + ", " + Year.ToString();
                chosenTime = new DateTime(Year, DateTime.ParseExact(Month, "MMMM", CultureInfo.CurrentCulture).Month, 1);
                GetDailyGraphData(chosenTime, SelectedDER, PowerType.Active, out ChartDataActive);
            }
            else
            {
                TimeReference = Year.ToString();
                chosenTime = new DateTime(Year, 1, 1);
                GetMonthlyGraphData(chosenTime, SelectedDER, PowerType.Active, out ChartDataActive);
            }

            if (ChartDataActive.Count > 0)
            {
                HistorianDataActive_X = new string[ChartDataActive.Count];
                int x_a_counter = 0;
                foreach (AnalogValue v in ChartDataActive)
                {
                    HistorianDataActive_Y[0].Values.Add(v.Value);
                    if (IsHourlyChecked)
                    {
                        HistorianDataActive_X[x_a_counter++] = new DateTime(v.Timestamp).ToShortTimeString();
                    }
                    else if (IsDailyChecked)
                    {
                        HistorianDataActive_X[x_a_counter++] = new DateTime(v.Timestamp).ToShortDateString();
                    }
                    else
                    {
                        HistorianDataActive_X[x_a_counter++] = Months[new DateTime(v.Timestamp).Month - 1];
                    }
                }
            }
        }

        private void ShowChartForReactivePower()
        {
            List<AnalogValue> ChartDataReactive = new List<AnalogValue>();

            HistorianDataReactive_Y[0].Values.Clear();

            DateTime chosenTime;

            if (IsHourlyChecked)
            {
                TimeReference = SelectedDate.ToShortDateString();
                chosenTime = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, 0, 0, 0);
                GetHourGraphData(chosenTime, SelectedDER, PowerType.Reactive, out ChartDataReactive);
            }
            else if (IsDailyChecked)
            {
                TimeReference = Month + ", " + Year.ToString();
                chosenTime = new DateTime(Year, DateTime.ParseExact(Month, "MMMM", CultureInfo.CurrentCulture).Month, 1);
                GetDailyGraphData(chosenTime, SelectedDER, PowerType.Reactive, out ChartDataReactive);
            }
            else
            {
                TimeReference = Year.ToString();
                chosenTime = new DateTime(Year, 1, 1);
                GetMonthlyGraphData(chosenTime, SelectedDER, PowerType.Reactive, out ChartDataReactive);
            }

            if (ChartDataReactive.Count > 0)
            {
                HistorianDataReactive_X = new string[ChartDataReactive.Count];
                int x_r_counter = 0;
                foreach (AnalogValue v in ChartDataReactive)
                {
                    HistorianDataReactive_Y[0].Values.Add(v.Value);
                    if (IsHourlyChecked)
                    {
                        HistorianDataReactive_X[x_r_counter++] = new DateTime(v.Timestamp).ToShortTimeString();
                    }
                    else if (IsDailyChecked)
                    {
                        HistorianDataReactive_X[x_r_counter++] = new DateTime(v.Timestamp).ToShortDateString();
                    }
                    else
                    {
                        HistorianDataReactive_X[x_r_counter++] = Months[new DateTime(v.Timestamp).Month - 1];
                    }
                }
            }
        }

        private void GetHourGraphData(DateTime chosenTime, long gid, PowerType type, out List<AnalogValue> active)
        {
            if (gid == -1)
            {
               active = proxy.Proxy.GetDailyConsuption(chosenTime.Ticks, type);
            }
            else
            {
               active = proxy.Proxy.GetDailyConsuption(chosenTime.Ticks, type, gid);
            }
        }

        private void GetDailyGraphData(DateTime chosenTime, long gid, PowerType type, out List<AnalogValue> active)
        {
            if (gid == -1)
            {
               active = proxy.Proxy.GetMonthlyConsuption(chosenTime.Ticks, type);
            }
            else
            {
                active = proxy.Proxy.GetMonthlyConsuption(chosenTime.Ticks, type, gid);
            }
        }

        private void GetMonthlyGraphData(DateTime chosenTime, long gid, PowerType type, out List<AnalogValue> active)
        {
            if (gid == -1)
            {
                active = proxy.Proxy.GetAnnualConsuption(chosenTime.Ticks, type);
            }
            else
            {
                active = proxy.Proxy.GetAnnualConsuption(chosenTime.Ticks, type, gid);
            }
        }

        public int Year
        {
            get
            {
                return year;
            }

            set
            {
                if (year != value)
                {
                    year = value;
                    OnPropertyChanged("Year");
                }
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }

            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                }
            }
        }

        public long SelectedDER
        {
            get
            {
                return selectedDER;
            }

            set
            {
                if (selectedDER != value)
                {
                    selectedDER = value;
                    OnPropertyChanged("SelectedDER");
                }
            }
        }

        public bool IsHourlyChecked
        {
            get
            {
                return isHourlyChecked;
            }

            set
            {
                if (isHourlyChecked != value)
                {
                    isHourlyChecked = value;
                    OnPropertyChanged("IsHourlyChecked");
                }
                if (isHourlyChecked)
                {
                    HourlyChosen = Visibility.Visible;
                    DailyChosen = Visibility.Collapsed;
                    MonthlyChosen = Visibility.Collapsed;
                }
            }
        }

        public bool IsDailyChecked
        {
            get
            {
                return isDailyChecked;
            }

            set
            {
                if (isDailyChecked != value)
                {
                    isDailyChecked = value;
                    OnPropertyChanged("IsDailyChecked");
                }
                if (isDailyChecked)
                {
                    HourlyChosen = Visibility.Collapsed;
                    DailyChosen = Visibility.Visible;
                    MonthlyChosen = Visibility.Collapsed;
                }
            }
        }

        public bool HoursChosen
        {
            get
            {
                return hoursChosen;
            }

            set
            {
                if (hoursChosen != value)
                {
                    hoursChosen = value;
                    OnPropertyChanged("HoursChosen");
                }
            }
        }

        public bool IsMonthlyChecked
        {
            get
            {
                return isMonthlyChecked;
            }

            set
            {
                if (isMonthlyChecked != value)
                {
                    isMonthlyChecked = value;
                    OnPropertyChanged("IsMonthlyChecked");
                }
                if (isMonthlyChecked)
                {
                    HourlyChosen = Visibility.Collapsed;
                    DailyChosen = Visibility.Collapsed;
                    MonthlyChosen = Visibility.Visible;
                }
            }
        }


        public string SelectedObject
        {
            get
            {
                return selectedObject;
            }

            set
            {
                selectedObject = value;
                OnPropertyChanged("SelectedObject");
            }
        }

        public Visibility HourlyChosen
        {
            get
            {
                return hourlyChosen;
            }

            set
            {
                hourlyChosen = value;
                OnPropertyChanged("HourlyChosen");
            }
        }

        public Visibility DailyChosen
        {
            get
            {
                return dailyChosen;
            }

            set
            {
                dailyChosen = value;
                OnPropertyChanged("DailyChosen");
            }
        }

        public Visibility MonthlyChosen
        {
            get
            {
                return monthlyChosen;
            }

            set
            {
                monthlyChosen = value;
                OnPropertyChanged("MonthlyChosen");
            }
        }

        public string Month
        {
            get
            {
                return month;
            }

            set
            {
                month = value;
                OnPropertyChanged("Month");
            }
        }

        public string TimeReference
        {
            get
            {
                return timeReference;
            }

            set
            {
                timeReference = value;
                OnPropertyChanged("TimeReference");
            }
        }
    }
}
