﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp.Views
{
	/// <summary>
	/// Interaction logic for AORManagementWindow.xaml
	/// </summary>
	public partial class AORManagementWindow : UserControl
	{
		public AORManagementWindow()
		{
			InitializeComponent();
			try
			{
				this.DataContext = new TestApp.ViewModels.AORManipulationWinVM();
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
