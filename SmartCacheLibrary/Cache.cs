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
    public class Cache
    {
        private List<CacheObject> _cacheList;

        public Cache()
        {
            this._cacheList = new List<CacheObject>();
        }

        public Cache(List<CacheObject> cacheList)
        {
            this._cacheList = new List<CacheObject>();
            if (cacheList != null)
            {
                this._cacheList = cacheList;
            }
        }

        [DataMember]
        public List<CacheObject> CacheList
        {
            get { return _cacheList; }
            set { _cacheList = value; }
        }

    }
}
