using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Common;

namespace SmartCacheLibrary
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(TypeOfReference))]
    [KnownType(typeof(PowerType))]
    [KnownType(typeof(DiscreteValue))]
    [KnownType(typeof(MeasurementValue))]
    [KnownType(typeof(AnalogValue))]
    public class CacheObject
    {
        private DateTime _timestamp;
        //private List<MeasurementValue> _measurements;
        private List<Object> _measurements;

        public CacheObject()
        {
            this._measurements = new List<Object>();
        }
        public CacheObject(DateTime timestamp, List<Object> measurements)
        {
            this._timestamp = timestamp;
            this._measurements = new List<Object>();
            if (measurements != null)
            {
                this._measurements = measurements;
            }

        }

        [DataMember]
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        [DataMember]
        public List<Object> Measurements
        {
            get { return _measurements; }
            set { _measurements = value; }
        }
    }
}
