using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.ViewModels
{
    public class AORSupervisionViewModel : BindableBase
    {
        private ObservableCollection<AreaModel> supervisionAreas = new ObservableCollection<AreaModel>();

        public AORSupervisionViewModel()
        {
            var a1 = new AreaModel("Backa-Area", DateTime.Now);
            SupervisionAreas.Add(a1);
        }
        public ObservableCollection<AreaModel> SupervisionAreas
        {
            get
            {
                return supervisionAreas;
            }

            set
            {
                supervisionAreas = value;
                OnPropertyChanged("SupervisionAreas");
            }
        }


    }
}
