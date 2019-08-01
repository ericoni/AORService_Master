using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.AORCachedModel
{
    public class AORCachedUserArea
    {
        public int UserId { get; set; }
        public int AreaId{ get; set; }
        public bool IsSelectedForView { get; set; }
    }
}
