using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace SmartCacheLibrary.Interfaces
{
    public interface ICacheServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyUserOfCache(string userName, /*string cache*/Cache cache);

    }
}
