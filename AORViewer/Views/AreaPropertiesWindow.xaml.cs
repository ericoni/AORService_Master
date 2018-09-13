using AORViewer.ViewModels;
using FTN.Common.AORCachedModel;
using System;
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
using System.Windows.Shapes;

namespace AORViewer.Views
{
	/// <summary>
	/// Interaction logic for AreaPropertiesWindow.xaml
	/// </summary>
	public partial class AreaPropertiesWindow : Window
	{
		AORCachedArea selectedArea;

		//public AreaPropertiesWindow(AORCachedArea selectedArea)
		//{
		//	InitializeComponent();
		//	this.selectedArea = selectedArea;
		//	this.DataContext = selectedArea;
		//}

		//public AreaPropertiesWindow()
		//{
		//	InitializeComponent();
		//	//this.selectedArea = selectedArea;
		//	//this.DataContext = mw;
		//}
		public AreaPropertiesWindow(AORVMainWindowViewModel mainVM)
		{
			InitializeComponent();
			this.DataContext = mainVM;
			//this.selectedArea = selectedArea;
			//this.DataContext = mw;
		}
	}
}
