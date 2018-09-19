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
using FTN.Common.AORCachedModel;
using AORViewer.Views;

namespace AORViewer.ViewModels
{
	public class AORVMainWindowViewModel : ViewModelBase
	{
		#region Fields
		private List<LBModelBase> aorViewerList;
		private List<LBModelBase> aorViewerTempList;
		private LBModelBase selectedElement;
		private List<Permission> permissionList;
		private List<DNAAuthority> dnaList;
		private List<AORCachedArea> aorAreas;
		private List<AORCachedGroup> aorGroups;
		private List<User> users;
		private AORViewerCommProxy aorViewCommProxy;
		private AORCachedArea selectedArea;
		private AORCachedGroup selectedGroup;
		private DNAAuthority selectedDna;
		private List<string> dnaUsernames;
		private List<string> areaUsernames;
		private List<string> areaPermissions;
		private List<string> areaGroups;
		private Permission selectedPermisssion;

		#endregion Fields
		#region Commands
		public ICommand AORAreaPropertiesCommand { get; private set; }
		public ICommand AORAreaDeleteCommand { get; private set; }
		public ICommand AORAreaGetUsersCommand { get; private set; }
		public ICommand AORGroupPropertiesCommand { get; private set; }
		public ICommand AORGroupDeleteCommand { get; private set; }
		public ICommand AORGroupGetAreasCommand { get; private set; }
		public ICommand DNAPropertiesCommand { get; private set; }
		public ICommand DNADeleteCommand { get; private set; }
		public ICommand PermissionPropertiesCommand { get; private set; }
		public ICommand PermissionDeleteCommand { get; private set; }


		#endregion Commands

		public AORVMainWindowViewModel() // na instanciranju uzima sve iz cache
		{
			aorViewerTempList = new List<LBModelBase>(4)
			{ new LBModelBase(LBType.Permissions.ToString(), "Neki opis", @"..\..\..\Images\Permission.jpg"),
				new LBModelBase(LBType.AOR_Groups.ToString(), "AOR GRUPE",@"..\..\..\Images\AORGroup.jpg"),
				new LBModelBase(LBType.AOR_Areas.ToString(), "ARea",@"..\..\..\Images\AORArea.jpg"),
				new LBModelBase(LBType.DNA_Authorities.ToString(), "Dna nesto", @"..\..\..\Images\Authority.jpg")};
			AORViewerList = aorViewerTempList;

			AORAreaPropertiesCommand = new RelayCommand(() => ExecuteAreaPropertiesCommand());
			AORAreaDeleteCommand = new RelayCommand(() => ExecuteAreaDeleteCommand());
			AORAreaGetUsersCommand = new RelayCommand(() => ExecuteAreaGetUsersCommand());

			AORGroupPropertiesCommand = new RelayCommand(() => ExecuteGroupPropertiesCommand());
			AORGroupDeleteCommand = new RelayCommand(() => ExecuteGroupDeleteCommand());
			AORGroupGetAreasCommand = new RelayCommand(() => ExecuteGroupGetAreasCommand());

			DNAPropertiesCommand = new RelayCommand(() => ExecuteDNAPropertiesCommand());
			DNADeleteCommand = new RelayCommand(() => ExecuteDNADeleteCommand());

			PermissionPropertiesCommand = new RelayCommand(() => ExecutePermissionPropertiesCommand());
			PermissionDeleteCommand = new RelayCommand(() => ExecutePermissionDeleteCommand());

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

				users = aorViewCommProxy.Proxy.GetAllUsers();
				Users = users;
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

		public List<AORCachedArea> AORAreas
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

		public List<AORCachedGroup> AORGroups
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

		public List<User> Users
		{
			get
			{
				return users;
			}

			set
			{
				users = value;
				OnPropertyChanged("Users");
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

		#region Visibility For Perms/DNAs/Authorities
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
			get
			{
				if (SelectedElement != null)
				{
					return SelectedElement.Name.Equals(LBType.AOR_Areas.ToString()) ? Visibility.Visible : Visibility.Collapsed;
				}
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
		#endregion

		#region Command Execution
		private void ExecuteAreaPropertiesCommand()
		{
			var usernames = aorViewCommProxy.Proxy.GetUsernamesForArea(SelectedArea.Name);
			AreaUsernames = usernames;

			AreaGroups = aorViewCommProxy.Proxy.GetGroupsForArea(SelectedArea.Name);

			AreaPermissions = aorViewCommProxy.Proxy.GetPermissionsForArea(SelectedArea.Name);

			AreaPropertiesWindow areaPropWindow = new AreaPropertiesWindow(this);
			areaPropWindow.ShowDialog();
		}
		private void ExecuteAreaDeleteCommand()
		{

		}

		private void ExecuteAreaGetUsersCommand()
		{
			AreaAddUserWindow areaAddUserWindow = new AreaAddUserWindow(this);
			areaAddUserWindow.ShowDialog();
		}
		private void ExecuteGroupPropertiesCommand()
		{
			GroupPropertiesWindow groupPropWindow = new GroupPropertiesWindow(this);
			groupPropWindow.ShowDialog();
		}
		private void ExecuteGroupDeleteCommand()
		{

		}

		private void ExecuteGroupGetAreasCommand()
		{
			GroupAddAreaWindow groupAddAreaWindow = new GroupAddAreaWindow(this);
			groupAddAreaWindow.ShowDialog();
		}

		private void ExecuteDNAPropertiesCommand()
		{
			var usernames = aorViewCommProxy.Proxy.GetUsernamesForDNA(SelectedDNA.Name);
			DNAUsernames = usernames;

			DNAPropertyWindow dnaPropWidow = new DNAPropertyWindow(this);
			dnaPropWidow.ShowDialog();
		}
		private void ExecuteDNADeleteCommand()
		{

		}

		private void ExecutePermissionPropertiesCommand()
		{
			PermissionPropertyWindow permPropWindow = new PermissionPropertyWindow(this);
			permPropWindow.ShowDialog();
		}
		private void ExecutePermissionDeleteCommand()
		{

		}
		#endregion Command Execution

		public AORCachedArea SelectedArea
		{
			get { return selectedArea; }
			set
			{
				selectedArea = value;
				OnPropertyChanged("SelectedArea");
			}
		}

		public AORCachedGroup SelectedGroup
		{
			get { return selectedGroup; }
			set
			{
				selectedGroup = value;
				OnPropertyChanged("SelectedGroup");
			}
		}

		public DNAAuthority SelectedDNA
		{
			get { return selectedDna; }
			set
			{
				selectedDna = value;
				OnPropertyChanged("SelectedDNA");
			}
		}

		public List<string> DNAUsernames
		{
			get { return dnaUsernames; }
			set
			{
				dnaUsernames = value;
				OnPropertyChanged("DNAUsernames");
			}
		}

		#region ForAreaProperties fill
		public List<string> AreaUsernames
		{
			get { return areaUsernames; }
			set
			{
				areaUsernames = value;
				OnPropertyChanged("AreaUsernames");
			}
		}

		public List<string> AreaPermissions
		{
			get { return areaPermissions; }
			set
			{
				areaPermissions = value;
				OnPropertyChanged("AreaPermissions");
			}
		}

		public List<string> AreaGroups
		{
			get { return areaGroups; }
			set
			{
				areaGroups = value;
				OnPropertyChanged("AreaGroups");
			}
		}

		#endregion ForAreaProperties fill

		public Permission SelectedPermission
		{
			get { return selectedPermisssion; }
			set
			{
				selectedPermisssion = value;
				OnPropertyChanged("SelectedPermission");
			}
		}

		public string DisplayedUserImage
		{
			get
			{
				return @"..\..\..\Images\user.png";
			}
		}
	}
}
