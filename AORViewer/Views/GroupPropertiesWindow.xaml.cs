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
	/// Interaction logic for GroupPropertiesWindow.xaml
	/// </summary>
	public partial class GroupPropertiesWindow : Window
	{
		AORCachedGroup selectedGroup;

		public GroupPropertiesWindow(AORCachedGroup selectedGroup)
		{
			InitializeComponent();
			this.selectedGroup = selectedGroup;
			this.DataContext = selectedGroup;
		}
	}
}
