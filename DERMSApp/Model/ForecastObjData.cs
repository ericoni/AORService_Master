using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DERMSApp.Model
{
    public class ForecastObjData
    {
        private long gid;
        private bool power;
        private bool isGroup;

        public ForecastObjData()
        {

        }

        public long Gid
        {
            get
            {
                return gid;
            }

            set
            {
                gid = value;
            }
        }

        public bool Power
        {
            get
            {
                return power;
            }

            set
            {
                power = value;
            }
        }

        public bool IsGroup
        {
            get
            {
                return isGroup;
            }

            set
            {
                isGroup = value;
            }
        }
    }
}
