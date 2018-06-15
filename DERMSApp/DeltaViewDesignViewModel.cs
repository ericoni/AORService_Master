using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DERMSApp.ViewModels;

namespace DERMSApp
{
    public class DeltaViewDesignViewModel
    {
        public DeltaViewDesignViewModel()
        {
            this.DeltaViewModel = new DeltaViewModel();
        }

        public DeltaViewModel DeltaViewModel { get; set; }

    }
}
