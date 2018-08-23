//using AORCommon.AORContract;
//using AORManagement;
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.CommandWpf;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Security;
//using System.ServiceModel;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Input;

//namespace AORViewer.ViewModels
//{
//	public class LoginViewModel : ViewModelBase
//	{
//		private string textBoxUsername = string.Empty;
//		private string textBoxPassword = string.Empty; // vratiti se za implementaciju secure string-a ili nesto sa sertifikatima
//		//private AORManagementProxy aorManagementProxy = null; //vrati se
//		/// <summary>
//		/// The current view.
//		/// </summary>
//		private ViewModelBase _currentViewModel;
//		public ICommand ButtonLoginOnClick { get; set; }
//		//public SecureString SecurePassword { set; get; }

//		public LoginViewModel()
//		{
//			InitGUIElements();
//			//aorManagementProxy = new AORManagementProxy();
//			//ButtonLoginOnClick = new RelayCommand(() => ButtonLoginOnClickExecute(), () => true); //vrati se i gore
//		}

//		private void InitGUIElements()
//		{
//			TextBoxUsernameText = string.Empty;
			
//		}

//		//private void ButtonLoginOnClickExecute()
//		//{
//		//	if (!TextBoxUsernameText.Equals(string.Empty))
//		//		if (aorManagementProxy.Proxy.Login(TextBoxUsernameText, TextBoxPasswordText))
//		//			MessageBox.Show("Success");
//		//		else
//		//			MessageBox.Show("Fail");
//		//}

//		/// <summary>
//		/// The CurrentView property.  The setter is private since only this 
//		/// class can change the view via a command.  If the View is changed,
//		/// we need to raise a property changed event (via INPC).
//		/// </summary>
//		public ViewModelBase CurrentViewModel
//		{
//			get
//			{
//				return _currentViewModel;
//			}
//			set
//			{
//				if (_currentViewModel == value)
//					return;
//				_currentViewModel = value;
//				RaisePropertyChanged("CurrentViewModel");
//			}
//		}

//		public string TextBoxUsernameText
//		{
//			get
//			{
//				return textBoxUsername;
//			}
//			set
//			{
//				if (textBoxUsername == value)
//					return;
//				textBoxUsername = value;
//				RaisePropertyChanged("TextBoxUsernameText");
//			}
//		}

//		public string TextBoxPasswordText
//		{
//			get
//			{
//				return textBoxPassword;
//			}
//			set
//			{
//				if (textBoxPassword == value)
//					return;
//				textBoxPassword = value;
//				RaisePropertyChanged("TextBoxPasswordText");
//			}
//		}
//	}
//}
