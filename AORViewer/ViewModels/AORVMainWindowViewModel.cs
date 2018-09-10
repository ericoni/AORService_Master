using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AORViewer.Model;
using FTN.Common.Model;
using AORManagementProxy;
using FTN.Common.AORModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace AORViewer.ViewModels
{
	public class AORVMainWindowViewModel : ViewModelBase
	{
		private List<LBModelBase> aorViewerList;
		private List<LBModelBase> aorViewerTempList;
		private LBModelBase selectedElement;
		private List<Permission> permissionList;
		private List<DNAAuthority> dnaList;
		private List<AORArea> aorAreas;
		private List<AORGroup> aorGroups;
		private AORViewerCommProxy aorViewCommProxy;
		public ICommand AORAreaPropertiesCommand { get; private set; }
		public ICommand AORAreaDeleteCommand { get; private set; }
		public ICommand AORGroupPropertiesCommand { get; private set; }
		public ICommand AORGroupDeleteCommand { get; private set; }

		public AORVMainWindowViewModel()
		{
			aorViewerTempList = new List<LBModelBase>(4)
			{ new LBModelBase(LBType.Permissions.ToString(), "Neki opis"), new LBModelBase(LBType.AOR_Groups.ToString(), "AOR GRUPE"),
				new LBModelBase(LBType.AOR_Areas.ToString(), "ARea"), new LBModelBase(LBType.DNA_Authorities.ToString(), "Dna nesto")};
			AORViewerList = aorViewerTempList;

			AORAreaPropertiesCommand = new RelayCommand(() => ExecuteAreaPropertiesCommand());
			AORAreaDeleteCommand = new RelayCommand(() => ExecuteAreaDeleteCommand());

			AORGroupPropertiesCommand = new RelayCommand(() => ExecuteGroupPropertiesCommand());
			AORGroupDeleteCommand = new RelayCommand(() => ExecuteGroupDeleteCommand());

			try
			{
				aorViewCommProxy = new AORViewerCommProxy();
				var pList = aorViewCommProxy.Proxy.GetAllPermissions();
				PermissionList = pList;

				var dnas = aorViewCommProxy.Proxy.GetAllDNAs();
				DNAList = dnas;

				var groups = aorViewCommProxy.Proxy.GetAORGroups();
				AORGroups = groups;

				var areas = aorViewCommProxy.Proxy.GetAORAreas();
				AORAreas = areas;
			}
			catch (Exception ex)
			{
				Console.WriteLine("AORVMainWindowViewModel Constructor failed: " + ex.StackTrace);
			}
		}
		public List<LBModelBase> AORViewerList
		{
			get
			{
				return aorViewerList;
			}

			set
			{
				aorViewerList = value;
				OnPropertyChanged("AORViewerList");
			}
		}

		public List<Permission> PermissionList
		{
			get
			{
				return permissionList;
			}

			set
			{
				permissionList = value;
				OnPropertyChanged("PermissionList");
			}
		}

		public List<AORArea> AORAreas
		{
			get
			{
				return aorAreas;
			}

			set
			{
				aorAreas = value;
				OnPropertyChanged("AORAreas");
			}
		}

		public List<AORGroup> AORGroups
		{
			get
			{
				return aorGroups;
			}

			set
			{
				aorGroups = value;
				OnPropertyChanged("AORGroups");
			}
		}

		public List<DNAAuthority> DNAList
		{
			get
			{
				return dnaList;
			}

			set
			{
				dnaList = value;
				OnPropertyChanged("DNAList");
			}
		}

		public LBModelBase SelectedElement
		{
			get { return selectedElement; }
			set
			{
				selectedElement = value;
				RefreshSelection();
			}
		}

		private void RefreshSelection()
		{
			OnPropertyChanged("SelectedElement");
			OnPropertyChanged("IsPermissionsSelected");
			OnPropertyChanged("IsAuthoritiesSelected");
			OnPropertyChanged("IsAORAreasSelected");
			OnPropertyChanged("IsAORGroupsSelected");
		}

		public Visibility IsPermissionsSelected
		{
			get
            {
                if (SelectedElement != null)
                    return SelectedElement.Name.Equals(LBType.Permissions.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                else
                    return Visibility.Collapsed;
            }
		}

		public Visibility IsAuthoritiesSelected
        {
            get
            {
                if (SelectedElement != null)
                    return SelectedElement.Name.Equals(LBType.DNA_Authorities.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                else
                    return Visibility.Collapsed;
            }
		}

		public Visibility IsAORAreasSelected
		{
            get {
                if (SelectedElement != null)
                    return SelectedElement.Name.Equals(LBType.AOR_Areas.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                 else
                    return Visibility.Collapsed;
            }
		}

		public Visibility IsAORGroupsSelected
        {
            get
            {
                if (SelectedElement != null)
                    return SelectedElement.Name.Equals(LBType.AOR_Groups.ToString()) ? Visibility.Visible : Visibility.Collapsed;
                else
                    return Visibility.Collapsed;
            }
		}
		private void ExecuteAreaPropertiesCommand()
		{
			aorViewCommProxy.Proxy.GetPermissionsForArea(555);
		}
		private void ExecuteAreaDeleteCommand()
		{

		}
		private void ExecuteGroupPropertiesCommand()
		{

		}
		private void ExecuteGroupDeleteCommand()
		{

		}
	}
}
