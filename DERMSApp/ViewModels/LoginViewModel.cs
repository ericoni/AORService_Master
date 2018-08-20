using AORManagement;
using DERMSApp.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DERMSApp.ViewModels
{
	public class LoginViewModel : ViewModelBase
	{
		private string textBoxUsername = string.Empty;
		private string textBoxPassword = string.Empty; // vratiti se za implementaciju secure string-a ili nesto sa sertifikatima
		//private AORManagementProxy aorManagementProxy = null; //vrati se
		private bool isUserAuthenticated = false;
		/// <summary>
		/// The current view.
		/// </summary>
		public ICommand ButtonLoginOnClick { get; set; }
		//public SecureString SecurePassword { set; get; }

		public LoginViewModel()
		{
			InitGUIElements();
			//aorManagementProxy = new AORManagementProxy();
			//ButtonLoginOnClick = new RelayCommand(() => ButtonLoginOnClickExecute(), () => true);
		}

		private void InitGUIElements()
		{
			
		}

		//public bool ButtonLoginOnClickExecute() //vrati se
		//{
		//	if (aorManagementProxy.Proxy.Login(TextBoxUsernameText, TextBoxPasswordText))
		//	{
		//		IsUserAuthenticated = true;

		//		return true;
		//	}
		//	else
		//	{
		//		IsUserAuthenticated = false;
		//		return false;
		//	}
		//}

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

	}
}
