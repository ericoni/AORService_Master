using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AORViewer.ViewModels
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
	public class MainWindowViewModel : ViewModelBase
	{
		/// <summary>
		/// The current view.
		/// </summary>
		private ViewModelBase _currentViewModel;
		/// <summary>
		/// Static instance of one of the ViewModels.
		/// </summary>
		readonly static LoginViewModel _loginViewModel = new LoginViewModel();


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
		/// Default constructor.  We set the initial view-model to 'FirstViewModel'.
		/// We also associate the commands with their execution actions.
		/// </summary>
		public MainWindowViewModel()
		{
			CurrentViewModel = MainWindowViewModel._loginViewModel;
		}

	}
}
