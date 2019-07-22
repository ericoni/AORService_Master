using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventCommon
{
    [DataContract]
    public class Event
    {
        public Event() { }
        public string Username { get; set; }
        [DataMember]
        public string Details { get; set; }
    }
}
