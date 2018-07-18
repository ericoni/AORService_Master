﻿using AORCommon.AORManagementContract;
using AORManagement;
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

namespace AORViewer.ViewModels
{
	public class LoginViewModel : ViewModelBase
	{
		private string textBoxUsername = string.Empty;
		private AORManagementProxy aorManagementProxy = null;
		/// <summary>
		/// The current view.
		/// </summary>
		private ViewModelBase _currentViewModel;
		public ICommand ButtonLoginOnClick { get; set; }
		public SecureString SecurePassword { private set; get; }

		public LoginViewModel()
		{
			InitGUIElements();
			aorManagementProxy = new AORManagementProxy();
			ButtonLoginOnClick = new RelayCommand(() => ButtonLoginOnClickExecute(), () => true);
		}

		private void InitGUIElements()
		{
			TextBoxUsernameText = string.Empty;
			
		}

		private void ButtonLoginOnClickExecute()
		{
			aorManagementProxy.Proxy.Login("aaaa", "bbbb");
		}

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
	}
}
