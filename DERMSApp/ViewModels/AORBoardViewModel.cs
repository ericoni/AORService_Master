using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DERMSApp.ViewModels
{
	public class AORBoardViewModel : ViewModelBase
	{
		private AreaModel a1 = null;
		private AreaModel a2 = null;
		private AreaModel a3 = null;
		private AreaModel a4 = null;
		private AreaModel a5 = null;
		private AreaModel a6 = null;
		private AreaModel a7 = null;
		private AreaModel a8 = null;
		private AreaModel a9 = null;
		private AreaModel a10 = null;
		TreeView treeView = null;
		

		#region Commands
		public ICommand ExecuteCommand { get; private set; }
		#endregion

		private ObservableCollection<AreaModel> areas = null;
		public AORBoardViewModel(TreeView treeView)
		{
			a1 = new AreaModel("Backa-Area");
			a2 = new AreaModel("Low-Voltage-Backa-Area");
			a3 = new AreaModel("High-Voltage-Backa-Area");
			a4 = new AreaModel("a4");

			a5 = new AreaModel("Zrenjanin-Area");
			a6 = new AreaModel("Low-Voltage-Zrenjanin-Area");
			a7 = new AreaModel("High-Voltage-Zrenjanin-Area");

			a8 = new AreaModel("NoviBecej-Area");
			a9 = new AreaModel("Low-Voltage-NoviBecej-Area");
			a10 = new AreaModel("High-Voltage-NoviBecej-Area");

			a1.SubAreas.Add(a2);
			a1.SubAreas.Add(a3);

			a3.SubAreas.Add(a4);
			a3.SubAreas.Add(a7);    // kraj prva grana

			a5.SubAreas.Add(a6);
			a5.SubAreas.Add(a7);    // kraj druga grana

			a8.SubAreas.Add(a9);
			a8.SubAreas.Add(a10);

			var areasTemp = new List<AreaModel>();
			areasTemp.Add(a1);
			areasTemp.Add(a2);
			areasTemp.Add(a3);
			areasTemp.Add(a4);

			areasTemp.Add(a5);
			areasTemp.Add(a6);

			areasTemp.Add(a7);

			areasTemp.Add(a8);
			areasTemp.Add(a9);
			areasTemp.Add(a10);

			areasTemp = FilterTopAreaAors(areasTemp);//to do sada je without filter
			Areas = new ObservableCollection<AreaModel>(areasTemp);

			ExecuteCommand = new RelayCommand(() => ExecuteExecuteCommand());
			this.treeView = treeView;
		}

		public ObservableCollection<AreaModel> Areas
		{
			get { return areas; }
			set
			{
				//if (area != value)
				//{
				areas = value;
				RaisePropertyChanged("Areas");
			}
		}

		private List<string> TestZaTreeView()
		{
			List<string> selectedNames = new List<string>();

			foreach (var item in treeView.Items.OfType<AreaModel>())
			{
				GetSelected(item, ref selectedNames);
			}
			return selectedNames;
		}
		public void GetSelected(AreaModel node, ref List<string> s)
		{
			if (node.IsCheckedForView)
				s.Add(node.Name);

			foreach (AreaModel child in node.SubAreas)
				GetSelected(child, ref s);
		}

		private void ExecuteExecuteCommand()
		{
			TestZaTreeView();
		}
		#region Private Methods
		private List<AreaModel> FilterTopAreaAors(List<AreaModel> areas)
		{
			var tempSubAreas = new List<AreaModel>();
			var tempSubs = new List<string>(areas.Count);

			foreach (var area in areas)
			{
				foreach (var sub in area.SubAreas)
				{
					tempSubs.Add(sub.Name);
				}
			}

			return areas.Where(a => !tempSubs.Contains(a.Name)).ToList();
		}
		#endregion
	}
}
