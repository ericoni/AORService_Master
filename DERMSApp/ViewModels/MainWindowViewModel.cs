﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;
using SmartCacheLibrary;
using SmartCacheLibrary.Interfaces;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Meas;
using DERMSApp.Model;
using AORManagementProxyNS;
using FTN.Common.AORCachedModel;
using EventCommon;
using DERMSApp.Views;

namespace DERMSApp.ViewModels
{
	/// <summary>
	/// This is our MainWindowViewModel that is tied to the MainWindow via the 
	/// ViewModelLocator class.
	/// </summary>
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
	public class MainWindowViewModel : ViewModelBase//, ICacheServiceCallback
	{
		//list to store current scada values for each der, bind it to some table
		//private List<TableSMItem> _tableItemList = new List<TableSMItem>();

		/*private SynchronizationContext _uiSyncContext = null;
		DuplexChannelFactory<ICacheService> factory = null;
		ICacheService proxy = null;*/
		#region Fields
		private string textBoxUsername = string.Empty;
		private string textBoxPassword = string.Empty; // to do secure string
		private bool isUserAuthenticated = false;
		private bool isLoginGridVisible = false;
		private bool wrongCredentialsVisibility = false;
		private bool dataTemplatesVisibility = false;

		/// <summary>
		/// Proxy for AOR Management
		/// </summary>
		private AORManagementProxy aorManagementProxy = null;
		/// <summary>
		/// The current view.
		/// </summary>
		private ViewModelBase _currentViewModel;

		/// <summary>
		/// Static instance of one of the ViewModels.
		/// </summary>
		//readonly static TabularViewModel _tabularViewModel = new TabularViewModel();
		//readonly static EntireNetworkViewModel _tabularViewModel = new EntireNetworkViewModel(); // ovako je bilo 30.6.
		readonly static EntireNetworkViewModel _tabularViewModel; // ne kontam sto se zove tabular, kad je entire network u pitanju no dobro

		/// <summary>
		/// Static instance of one of the ViewModels.
		/// </summary>
		readonly static DeltaViewModel _deltaViewModel = new DeltaViewModel();

		/// <summary>
		/// Visibility of Network View
		/// </summary>
		private Visibility showNetwork;

		/// <summary>
		/// Visibility of Apply Delta View
		/// </summary>
		private Visibility showApplyDelta;

		/// <summary>
		/// Max number of tries to connect to Calculation Engine
		/// </summary>
		private const int maxTry = 5;
		#endregion

		/// <summary>
		/// The CurrentView property.  The setter is private since only this 
		/// class can change the view via a command.  If the View is changed,
		/// we need to raise a property changed event (via INPC).
		/// </summary>
		public ViewModelBase CurrentViewModel
		{
			get
			{
				return _currentViewModel;
			}
			set
			{
				if (_currentViewModel == value)
					return;
				_currentViewModel = value;
				RaisePropertyChanged("CurrentViewModel");
			}
		}

		/// <summary>
		/// Simple property to hold the 'FirstViewCommand' - when executed
		/// it will change the current view to the 'FirstView'
		/// </summary>
		public ICommand DeltaViewCommand { get; private set; }

		/// <summary>
		/// Simple property to hold the 'SecondViewCommand' - when executed
		/// it will change the current view to the 'SecondView'
		/// </summary>
		public ICommand SecondViewCommand { get; private set; }

		/// <summary>
		/// Simple property to hold the 'ButtonLoginCommand' - when executed it will send 
		/// login request to AORService
		/// </summary>
		public ICommand ButtonLoginOnClick { get; set; }
		public ICommand ShowAORBoardCommand { get; private set; }
		public ICommand ShowAlarmsCommand { get; private set; }
		public ICommand ShowAORSupervisionCommand { get; private set; }

		/// <summary>
		/// The Show Network property. 
		/// Changes if the Network Table and Tree View needs to be shown.
		/// </summary>
		public Visibility ShowNetwork
		{
			get { return showNetwork; }
			set
			{
				showNetwork = value;
				RaisePropertyChanged("ShowNetwork");
			}
		}

		/// <summary>
		/// The Show Network property. 
		/// Changes if the Apply Delta View needs to be shown.
		/// </summary>
		public Visibility ShowApplyDelta
		{
			get { return showApplyDelta; }
			set
			{
				showApplyDelta = value;
				RaisePropertyChanged("ShowApplyDelta");
			}
		}

		/*public List<TableSMItem> TableItemList
		{
			get { return _tableItemList; }
			set { _tableItemList = value; }
		}*/

		/// <summary>
		/// Default constructor.  We set the initial view-model to 'FirstViewModel'.
		/// We also associate the commands with their execution actions.
		/// </summary>
		public MainWindowViewModel()
		{
			DataTemplatesVisibility = false; // master projekat prikaz (sve ono sto nije login)
			LoginGridVisibility = true; //invert ova dva polja, ako podesavas app koji nema applied delta

			//aorManagementProxy = new AORManagementProxy(); // ugasi, ako prvi put podesavas app
			ButtonLoginOnClick = new RelayCommand(() => ButtonLoginOnClickExecute(), () => true);

			//CurrentViewModel = MainWindowViewModel._tabularViewModel; // ovako je bilo 30.6., sad je u else od ButtonLoginOnClickExecute()

			DeltaViewCommand = new RelayCommand(() => ExecuteDeltaViewCommand());
			SecondViewCommand = new RelayCommand(() => ExecuteSecondViewCommand());
			ShowAORBoardCommand = new RelayCommand(() => ExecuteShowAORBoardCommand());
			ShowAORSupervisionCommand = new RelayCommand(() => ExecuteShowAORSupervisionCommand());
			ShowAlarmsCommand = new RelayCommand(() => ExecuteShowAlarmsCommand());

			//ConnectToCalculationEngine();
			//ShowNetwork = Visibility.Visible; //bio koment
			//ShowApplyDelta = Visibility.Collapsed;

			ConnectCacheToCE();

			TextBoxUsernameText = "admin";//to do hardcoded for now
			TextBoxPasswordText = "a";
		}

		/// <summary>
		/// Method that does authentication and authorization. aorManagementProxy.Proxy.Login
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ButtonLoginOnClickExecute()
		{
			aorManagementProxy = new AORManagementProxy();
			var aorAreaObjects = aorManagementProxy.Proxy.GetAORAreaObjectsForUsername("admin");// to do implement again xD 15.8.
			//var aorAreaNames = aorManagementProxy.Proxy.GetAORAreasForUsername("admin");
			//var areasGroupsMapping = aorManagementProxy.Proxy.GetAORGroupsAreasMapping(aorAreaNames);

			CurrentViewModel = new EntireNetworkViewModel(aorAreaObjects); // to do jako je bitno vratiti ga ovde 24.7.
			//var aorAreasStrings = aorManagementProxy.Proxy.GetAORAreasForUsername(TextBoxUsernameText); //to do vratiti se vrati se ovde obavezno. Znaci prvo sub pa onda Login.
			//SubscribeEntireVMForEvents(aorAreasStrings);

			//var aorAreasRealLogin = aorManagementProxy.Proxy.Login(TextBoxUsernameText, TextBoxPasswordText);
			var aorAreas = new List<AORCachedArea>(1) { new AORCachedArea("aaa", "bbbb", null, null, null) }; //to do temp fix, samo kod ponovnog ucitavanja delte. Ne sjecam se tacno zasto. xD

			if (aorAreas.Count == 0)
			{
				IsUserAuthenticated = false;
				WrongCredentialsVisibility = true;
				return false;
			}
			else
			{
				IsUserAuthenticated = true;
				LoginGridVisibility = false;
				//CurrentViewModel = new EntireNetworkViewModel(aorAreas); // to do jako je bitno vratiti ga ovde 24.7.
				//CurrentViewModel = new EntireNetworkViewModel(new List<AORCachedArea>()); // to do jako je bitno vratiti ga ovde 24.7., 25.7.

				//CurrentViewModel = _tabularViewModel;
				DataTemplatesVisibility = true; // ovo ili probati sa onim event djavolima, sta je datatemplates jbt
				return true;
			}
		}

		private void ConnectCacheToCE()
		{
			CacheReceiver cacheReceiver = CacheReceiver.Instance;

			int tryCounter = 0;

			while (true)
			{
				try
				{
					cacheReceiver.ConnectToCalculationEngine();
					break;
				}
				catch (Exception)
				{
					tryCounter++;

					if (tryCounter.Equals(maxTry))
					{
						throw;
					}

					Thread.Sleep(2000);
				}
			}
		}

		/// <summary>
		/// Set the CurrentViewModel to 'FirstViewModel' (DeltaViewModel)
		/// </summary>
		private void ExecuteDeltaViewCommand()
		{
			CurrentViewModel = MainWindowViewModel._deltaViewModel;
			//ShowNetwork = Visibility.Collapsed;
			//ShowApplyDelta = Visibility.Visible;
		}

		/// <summary>
		/// Set the CurrentViewModel to 'SecondViewModel. Executes when user presses "Home" button then it makes a call to "EntireVM" to method ExecuteShowTableCommand().
		/// </summary>
		private void ExecuteSecondViewCommand()
		{
			CurrentViewModel = MainWindowViewModel._tabularViewModel;
			EventSystem.Publish<string>("ShowTable");
			//ShowNetwork = Visibility.Visible;
			//ShowApplyDelta = Visibility.Collapsed;
		}

		private void SubscribeEntireVMForEvents(List<string> areaNames)
		{
			IDERMSEventSubscription proxy = null;

			DuplexChannelFactory<IDERMSEventSubscription> factory = new DuplexChannelFactory<IDERMSEventSubscription>(
				new InstanceContext(this),
				new NetTcpBinding(),
				new EndpointAddress("net.tcp://localhost:10047/IDERMSEvent"));
			proxy = factory.CreateChannel();

			proxy.Subscribe(areaNames);
		}


		private void ExecuteShowAORBoardCommand()
		{
			AORBoardWindow aorBoardWindow = new AORBoardWindow();
			try
			{
				aorBoardWindow.Show();
			}
			catch (Exception e)
			{
				throw;
			}
		}

		private void ExecuteShowAlarmsCommand()
		{
			EventSystem.Publish("ShowAlarmsEventSystem");
		}
		private void ExecuteShowAORSupervisionCommand()
		{
			AORSupervisionWindow aorSupervisionBoard = new AORSupervisionWindow();
			try
			{
				aorSupervisionBoard.Show();
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public string TextBoxUsernameText
		{
			get
			{
				return textBoxUsername;
			}
			set
			{
				if (textBoxUsername == value)
					return;
				textBoxUsername = value;
				RaisePropertyChanged("TextBoxUsernameText");
			}
		}

		public string TextBoxPasswordText
		{
			get
			{
				return textBoxPassword;
			}
			set
			{
				if (textBoxPassword == value)
					return;
				textBoxPassword = value;
				RaisePropertyChanged("TextBoxPasswordText");
			}
		}

		public bool IsUserAuthenticated
		{
			get
			{
				return isUserAuthenticated;
			}
			private set
			{
				isUserAuthenticated = value;
				RaisePropertyChanged("IsUserAuthenticated");
			}
		}

		public bool LoginGridVisibility
		{
			get
			{
				return 	isLoginGridVisible;
			}
			set
			{
				isLoginGridVisible = value;
				RaisePropertyChanged("LoginGridVisibility");
			}
		}

		public bool DataTemplatesVisibility
		{
			get
			{
				return dataTemplatesVisibility;
			}
			set
			{
				dataTemplatesVisibility = value;
				RaisePropertyChanged("DataTemplatesVisibility");
			}
		}

		public bool WrongCredentialsVisibility
		{
			get
			{
				return wrongCredentialsVisibility;
			}
			set
			{
				wrongCredentialsVisibility = value;
				RaisePropertyChanged("WrongCredentialsVisibility");
			}
		}


		#region Master commented code

		/// <summary>
		/// Set the CurrentViewModel to 'ThirdViewModel'
		/// </summary>
		/*private void ExecuteThirdViewCommand()
		{
			ShowNetwork = Visibility.Collapsed;
			ShowApplyDelta = Visibility.Collapsed;
		}*/

		/// <summary>
		/// 
		/// </summary>
		/*private void ConnectToCalculationEngine()
		{
			// Capture the UI synchronization context
			_uiSyncContext = SynchronizationContext.Current;
			// The client callback interface must be hosted for the server to invoke the callback
			// Open a connection to the message service via the proxy 

			factory = new DuplexChannelFactory<ICacheService>(
			  new InstanceContext(this),
			  new NetTcpBinding(),
			  new EndpointAddress("net.tcp://localhost:10012/ICacheService"));
			proxy = factory.CreateChannel();
			proxy.Register("");
			
			//put a button window closing so that we can unsubscribe from ds
			
		}


		/// <summary>
		/// Client callback method
		/// </summary>
		/// <param name="arg_Name"></param>
		/// <param name="arg_Message"></param>
		public void NotifyUserOfCache(string arg_Name, Cache arg_Message)
		{
			// The UI thread won't be handling the callback, but it is the only one allowed to update the controls.  
			// So, we will dispatch the UI update back to the UI sync context.
			SendOrPostCallback callback =
				delegate (object state)
				{
					//receive cache data... put it in some list and set it to datagrid
	
					ConvertAnalogValuesToTableItemValues(arg_Message);
				};

			_uiSyncContext.Post(callback, arg_Name);
		}

		//Implement onClosing window so we can unredister and close channel

		public void ConvertAnalogValuesToTableItemValues(Cache cache)
		{
			if (cache.CacheList[0] != null)
			{
				List<Object> cacheObjects = cache.CacheList[0].Measurements;
				DateTime timestamp = cache.CacheList[0].Timestamp;
				this._tableItemList.Clear();

				var groups = cacheObjects.GroupBy(c => ((AnalogValue)c).SynchronousMachine).Where(g => g.Skip(1).Any());

				//in each group should be two values, one for P and one for Q of the same SynchronousMachine
				foreach (var group in groups)
				{
					TableSMItem tableItem = new TableSMItem(timestamp, group.ToList());
					this._tableItemList.Add(tableItem);
				}
				

			}
			
		}*/
		#endregion

	}
}
