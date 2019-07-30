using DERMSApp.ViewModels;
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

namespace DERMSApp.Views
{
	/// <summary>
	/// Interaction logic for AORBoardWindow.xaml
	/// </summary>
	public partial class AORBoardWindow : Window
	{
		public AORBoardWindow()
		{
			InitializeComponent();
			//this.DataContext = new AORBoardViewModel(treeViewAORBoard);
			this.DataContext = new AORBoardViewModel(new TreeView());
		}
	}
}
