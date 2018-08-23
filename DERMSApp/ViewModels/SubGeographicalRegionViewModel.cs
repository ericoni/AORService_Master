using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Wires;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DERMSApp.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using Adapter;

namespace DERMSApp.ViewModels
{
	public class SubGeographicalRegionViewModel : TreeViewItemViewModel
	{
		readonly SubGeographicalRegion _subGeographicalRegion;
		private RDAdapter rdAdapter = new RDAdapter();
		ObservableCollection<TableSMItem> _ders;
		private Visibility reactiveVisibility;

		public ICommand ActivePowerCommand { get; private set; }
		public ICommand ReactivePowerCommand { get; private set; }

		public SubGeographicalRegionViewModel(SubGeographicalRegion subGeographicalRegion, GeographicalRegionViewModel parentRegion, ObservableCollection<TableSMItem> ders)
			: base(parentRegion, true)
		{
			ReactiveVisibility = Visibility.Collapsed;

			var allDERs = rdAdapter.GetDERs(subGeographicalRegion.GlobalId);

			foreach (SynchronousMachine der in allDERs)
			{
				if (der.FuelType == FTN.Common.FuelType.Wind)
				{
					ReactiveVisibility = Visibility.Visible;
				}
			}

			_subGeographicalRegion = subGeographicalRegion;
			ActivePowerCommand = new RelayCommand(() => ExecuteActivePowerCommand());
			ReactivePowerCommand = new RelayCommand(() => ExecuteReactivePowerCommand());
			_ders = ders;
			_ders.Clear();
		}

		public string SubGeographicalRegionName
		{
			get { return _subGeographicalRegion.Name; }
		}

		public Visibility ReactiveVisibility
		{
			get
			{
				return reactiveVisibility;
			}

			set
			{
				reactiveVisibility = value;
				OnPropertyChanged("ReactiveVisibility");
			}
		}

		protected override void LoadChildren()
		{
			foreach (Substation substation in rdAdapter.GetSubstationsForSubRegion(_subGeographicalRegion.GlobalId))
			{
				base.Children.Add(new SubstationViewModel(substation, this, _ders));
			}
		}

		protected override void LoadDERS()
		{
			EventSystem.Publish<long>(_subGeographicalRegion.GlobalId);
			_ders.Clear();
			foreach (SynchronousMachine der in rdAdapter.GetDERs(_subGeographicalRegion.GlobalId))
			{
				TableSMItem item = new TableSMItem();
				item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
				if (item == null)
				{
					item = new TableSMItem();
					item.CurrentP = 0;
					item.TimeStamp = new DateTime();
					item.CurrentQ = 0;
					item.PDecrease = 0;
					item.PIncrease = 0;
					item.QDecrease = 0;
					item.QIncrease = 0;
				}
				item.Der = der;
				_ders.Add(item);
			}
		}

		private void ExecuteReactivePowerCommand()
		{
			_ders.Clear();
			foreach (SynchronousMachine der in rdAdapter.GetDERs(_subGeographicalRegion.GlobalId))
			{
				TableSMItem item = new TableSMItem();
				item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
				if (item == null)
				{
					item = new TableSMItem();
					item.CurrentP = 0;
					item.TimeStamp = new DateTime();
					item.CurrentQ = 0;
					item.PDecrease = 0;
					item.PIncrease = 0;
					item.QDecrease = 0;
					item.QIncrease = 0;
				}
				item.Der = der;
				_ders.Add(item);
			}
			base.IsSelected = true;
			EventSystem.Publish<ObservableCollection<TableSMItem>>(_ders);
			EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _subGeographicalRegion.GlobalId, Power = false, IsGroup = true });
		}

		private void ExecuteActivePowerCommand()
		{
			_ders.Clear();
			foreach (SynchronousMachine der in rdAdapter.GetDERs(_subGeographicalRegion.GlobalId))
			{
				TableSMItem item = new TableSMItem();
				item = (TableSMItem)CacheReceiver.Instance.TableItemList.Where(o => o.Gid.Equals(der.GlobalId)).FirstOrDefault();
				if (item == null)
				{
					item = new TableSMItem();
					item.CurrentP = 0;
					item.TimeStamp = new DateTime();
					item.CurrentQ = 0;
					item.PDecrease = 0;
					item.PIncrease = 0;
					item.QDecrease = 0;
					item.QIncrease = 0;
				}
				item.Der = der;
				_ders.Add(item);
			}
			base.IsSelected = true;
			EventSystem.Publish<ObservableCollection<TableSMItem>>(_ders);
			EventSystem.Publish<ForecastObjData>(new ForecastObjData() { Gid = _subGeographicalRegion.GlobalId, Power = true, IsGroup = true });
		}
	}
}
