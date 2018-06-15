using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Services.NetworkModelService.DataModel.Wires;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using System.ComponentModel;
using Adapter;

namespace DERMSApp.Model
{
	public class TableSMItem : BindableBase
    {
		private long gid;
		private float _currentP;
		private float _currentQ;
        private float _pIncrease;
        private float _pDecrease;
        private float _qIncrease;
        private float _qDecrease;
		private DateTime _timestamp;
		private RDAdapter rdAdapter = new RDAdapter();
        private SynchronousMachine der;

        public TableSMItem()
		{
			
		}

		public TableSMItem(DateTime timestamp, List<Object> group)
		{
			this.Gid = ((AnalogValue)group[0]).SynchronousMachine;
			this.TimeStamp = timestamp;
			foreach (var item in group)
			{
				if(((AnalogValue)item).PowerType == PowerType.Active)
				{
					this.CurrentP = ((AnalogValue)item).Value;
				}
				else
				{
					this.CurrentQ = ((AnalogValue)item).Value;
				}
			}
		}

		public long Gid
		{
			get { return gid; }
			set { gid = value;  }
		}

		public float CurrentP
		{
			get { return _currentP; }
			set { _currentP = value; OnPropertyChanged("CurrentP"); }
		}

		public float CurrentQ
		{
			get { return _currentQ; }
			set { _currentQ = value; OnPropertyChanged("CurrentQ"); }
		}

		public DateTime TimeStamp
		{
			get { return _timestamp; }
			set { _timestamp = value; OnPropertyChanged("TimeStamp"); }
		}

        public SynchronousMachine Der
        {
            get
            {
                return der;
            }

            set
            {
                der = value;
            }
        }

        public float PIncrease
        {
            get
            {
                return _pIncrease;
            }

            set
            {
                _pIncrease = value;
                OnPropertyChanged("PIncrease");
            }
        }

        public float PDecrease
        {
            get
            {
                return _pDecrease;
            }

            set
            {
                _pDecrease = value;
                OnPropertyChanged("PDecrease");
            }
        }

        public float QIncrease
        {
            get
            {
                return _qIncrease;
            }

            set
            {
                _qIncrease = value;
                OnPropertyChanged("QIncrease");
            }
        }

        public float QDecrease
        {
            get
            {
                return _qDecrease;
            }

            set
            {
                _qDecrease = value;
                OnPropertyChanged("QDecrease");
            }
        }
    }
}
