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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DERMSApp.ViewModels;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.ESI.SIMES.CIM.CIMAdapter;

namespace DERMSApp.Views
{
    /// <summary>
    /// Interaction logic for TabularView.xaml
    /// </summary>
    public partial class TabularView : UserControl
    {
		private CIMAdapter adapter = new CIMAdapter();
        public TabularView()
        {
            InitializeComponent();
			//GeographicalRegion[] regions = adapter.GetRegions().ToArray();
           // EntireNetworkViewModel viewModel = new EntireNetworkViewModel();
            //derTable.CanUserAddRows = false;
            //base.DataContext = viewModel;
        }

		private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
		{
			TreeViewItem item = sender as TreeViewItem;
			if (item != null)
			{
				item.Focus();
				e.Handled = true;
			}
		}
	}
}
