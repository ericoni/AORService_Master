﻿using System;
using System.Collections.Generic;
using System.Windows;
using AORViewer.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using FTN.Common.AORCachedModel;
using AORViewer.Views;
using AORManagementProxyNS;
using Adapter;
using FTN.Common.AORHelpers;

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
		private HashSet<AORCachedArea> aorAreas;
		private List<AORCachedGroup> aorGroups;
		private List<AORCachedUser> users;
		private AORCacheAccessProxy aorViewCommProxy;
		private AORCachedArea selectedArea;
		private AORCachedGroup selectedGroup;
		private DNAAuthority selectedDna;
		private List<string> dnaUsernames;
		private List<string> areaUsernames;
		private List<string> areaPermissions;
		private string areaPermissionsOneLine = string.Empty;
		private List<string> areaGroups;
		private Permission selectedPermisssion;
		RDAdapter rdAdapter;

		#endregion Fields
		#region Commands
		public ICommand AORAreaPropertiesCommand { get; private set; }
		public ICommand AORAreaDeleteCommand { get; private set; }
		public ICommand AORAreaGetUsersCommand { get; private set; }
		public ICommand AddNewAreaCommand { get; private set; }
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
			AddNewAreaCommand = new RelayCommand(() => ExecuteAddNewAreaCommand());

			AORGroupPropertiesCommand = new RelayCommand(() => ExecuteGroupPropertiesCommand());
			AORGroupDeleteCommand = new RelayCommand(() => ExecuteGroupDeleteCommand());
			AORGroupGetAreasCommand = new RelayCommand(() => ExecuteGroupGetAreasCommand());

			DNAPropertiesCommand = new RelayCommand(() => ExecuteDNAPropertiesCommand());
			DNADeleteCommand = new RelayCommand(() => ExecuteDNADeleteCommand());

			PermissionPropertiesCommand = new RelayCommand(() => ExecutePermissionPropertiesCommand());
			PermissionDeleteCommand = new RelayCommand(() => ExecutePermissionDeleteCommand());

			try
			{
				aorViewCommProxy = new AORCacheAccessProxy();
				var pList = aorViewCommProxy.Proxy.GetAllPermissions();
				PermissionList = pList;

				var dnas = aorViewCommProxy.Proxy.GetAllDNAs();
				DNAList = dnas;

				//var groups = aorViewCommProxy.Proxy.GetAORGroups();//to do temp commented
				//var groups = new List<AORCachedGroup>() { new AORCachedGroup("Zrenjanin-East-MediumVoltage", 1) };

				rdAdapter = new RDAdapter();
				var nmsAorGroups = rdAdapter.GetAORGroups();
				List<AORCachedGroup> aorCachedGroups = new List<AORCachedGroup>();

				foreach (var nmsGroup in nmsAorGroups)
				{
					var a = NMSModelAORConverter.ConvertAORGroupFromNMS(nmsGroup);
					aorCachedGroups.Add(a);
				}
				AORGroups = aorCachedGroups;

				#region perms
				Permission p1 = new Permission("DNA_PermissionControlSCADA", "Permission to issue commands towards SCADA system.");
				Permission p2 = new Permission("DNA_PermissionUpdateNetworkModel", "Permission to apply delta (model changes)- update current network model within their assigned AOR");
				Permission p3 = new Permission("DNA_PermissionViewSystem", "Permission to view content of AORViewer");
				Permission p4 = new Permission("DNA_PermissionSystemAdministration", "Permission to view system settings in AORViewer");
				Permission p5 = new Permission("DNA_PermissionViewSecurity", "Permission to view security content of AORViewer");
				Permission p6 = new Permission("DNA_PermissionSecurityAdministration", "Permission to edit security content of AORViewer");
				Permission p7 = new Permission("DNA_PermissionViewAdministration", "Permission to edit security content of AORViewer");
				Permission p8 = new Permission("DNA_PermissionViewSCADA", "Permission to view content operating under SCADA system.");
				Permission p9 = new Permission("DNA_PermissionViewSCADA_HV", "Permission to view high voltage content operating under SCADA system.");
				Permission p10 = new Permission("DNA_PermissionViewSCADA_LV", "Permission to view low voltage content operating under SCADA system.");
				#endregion

				//var areas = aorViewCommProxy.Proxy.GetAORAreas();
				#region Areas
				AORCachedArea area1 = new AORCachedArea("West-Area", "", new List<Permission> { p1, p2, p3, p4 }, new List<AORCachedGroup>(aorGroups)); // aorGroup[0] gets id 7 (mozda sto prva nema u sebi SM?)
				AORCachedArea area2 = new AORCachedArea("East-Area", "", new List<Permission> { p1, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1] });
				AORCachedArea area3 = new AORCachedArea("South-Area", "", new List<Permission> { p2, p3, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[5] });
				AORCachedArea area4 = new AORCachedArea("North-Area", "", new List<Permission> { p1, p2, p4, p5, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3] });
				AORCachedArea area5 = new AORCachedArea("North-Area2", "", new List<Permission> { p5, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3] });
				AORCachedArea area6 = new AORCachedArea("North-Area-HighVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area7 = new AORCachedArea("East-Area-Wind", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area8 = new AORCachedArea("East-Area-LowVoltage", "", new List<Permission> { p1, p8 }, new List<AORCachedGroup>() { aorGroups[1], aorGroups[2], aorGroups[3], aorGroups[4] });
				AORCachedArea area9 = new AORCachedArea("East-Area-HighVoltage", "", new List<Permission> { p1, p7, p8 }, new List<AORCachedGroup>() { aorGroups[1] });
				AORCachedArea area10 = new AORCachedArea("Central-Area-HighVoltage", "", new List<Permission> { p1, p2, p3, p7, p8 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1], aorGroups[5], aorGroups[6] });
				AORCachedArea area11 = new AORCachedArea("Central-Area-LowVoltage", "", new List<Permission> { p1, p2, p5, p7 }, new List<AORCachedGroup>() { aorGroups[0], aorGroups[1], aorGroups[4], aorGroups[6] });

				HashSet<AORCachedArea> hashSetAreas = new HashSet<AORCachedArea>() { area1, area2, area3, area4, area5, area6, area7, area8, area9, area10, area11 };
				#endregion
				AORAreas = hashSetAreas;

				//foreach (var item in areas)
				//{
				//	var perms = aorViewCommProxy.Proxy.GetPermissionsForArea(item.Name);
				//	foreach (var p in perms)
				//	{
				//		item.Permissions.Add(new Permission(p));
				//	}
				//}

				//users = aorViewCommProxy.Proxy.GetAllUsers();
				//Users = users;
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

		public HashSet<AORCachedArea> AORAreas
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

		public List<AORCachedUser> Users
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
		public Visibility IsAORGroupsOrAreasSelected
		{
			get
			{
				if (SelectedElement != null)
					return SelectedElement.Name.Equals(LBType.AOR_Groups.ToString()) || SelectedElement.Name.Equals(LBType.AOR_Areas.ToString()) ? Visibility.Visible : Visibility.Collapsed;
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

			//string tempString = string.Empty;

			//foreach (var item in aorViewCommProxy.Proxy.GetPermissionsForArea(SelectedArea.Name))
			//{
			//	tempString += item + ",";
			//}

			//tempString = tempString.Remove(tempString.Length - 1);

			//AreaPermissionsInOneLine = tempString; // budz za dobijanje ispisa svih perms u AOR areas

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

		private void ExecuteAddNewAreaCommand()
		{
			AreaAddNewWindow areaAddNewWindow = new AreaAddNewWindow(this);
			areaAddNewWindow.ShowDialog();
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

		public string AreaPermissionsInOneLine
		{
			get { return areaPermissionsOneLine; }
			set
			{
				areaPermissionsOneLine = value;
				OnPropertyChanged("AreaPermissionsInOneLine");
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
