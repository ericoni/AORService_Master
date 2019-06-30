using SmartCacheLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCacheLibrary;
using System.ServiceModel;
using System.Threading;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Common;

namespace DERMSApp.Model
{
    /// <summary>
    /// To be documented.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CacheReceiver : ICacheServiceCallback
    {
        private SynchronizationContext _uiSyncContext = null;
        DuplexChannelFactory<ICacheService> factory = null;
        ICacheService proxy = null;

        private List<TableSMItem> _tableItemList = new List<TableSMItem>();
        
        /// <summary>
        /// 
        /// </summary>
        private static volatile CacheReceiver instance;

        /// <summary>
        /// Lock object
        /// </summary>
        private static object syncRoot = new Object();

        private CacheReceiver()
        {

        }

        /// <summary>
        /// Singleton method
        /// </summary>
        public static CacheReceiver Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CacheReceiver();
                    }
                }

                return instance;
            }
        }

        public List<TableSMItem> TableItemList
        {
            get
            {
                return _tableItemList;
            }

            set
            {
                _tableItemList = value;
            }
        }

        public void ConnectToCalculationEngine()
        {
            // Capture the UI synchronization context
            _uiSyncContext = SynchronizationContext.Current;
			// The client callback interface must be hosted for the server to invoke the callback
			// Open a connection to the message service via the proxy 

			factory = new DuplexChannelFactory<ICacheService>(
			  new InstanceContext(this),
			  new NetTcpBinding(),
			  new EndpointAddress("net.tcp://localhost:10012/ICacheService"));
			proxy = factory.CreateChannel();
			proxy.Register("");

			//put a button window closing so that we can unsubscribe from ds

		}

        public void NotifyUserOfCache(string arg_Name, Cache arg_Message)
        {
            // The UI thread won't be handling the callback, but it is the only one allowed to update the controls.  
            // So, we will dispatch the UI update back to the UI sync context.
            SendOrPostCallback callback =
                delegate (object state)
                {
                    //receive cache data... put it in some list and set it to datagrid

                    ConvertAnalogValuesToTableItemValues(arg_Message);
                };

            _uiSyncContext.Post(callback, arg_Name);
        }

        public void ConvertAnalogValuesToTableItemValues(Cache cache)
        {
            if (cache.CacheList[0] != null)
            {
                List<Object> cacheObjects = cache.CacheList[0].Measurements;
                DateTime timestamp = cache.CacheList[0].Timestamp;
				//this._tableItemList.Clear();

				var groups = cacheObjects.GroupBy(c => ((AnalogValue)c).SynchronousMachine);

                //in each group should be two values, one for P and one for Q of the same SynchronousMachine
                foreach (var group in groups)
                {
					foreach (AnalogValue value in group.ToList() )
						{
						TableSMItem tableItem = _tableItemList.Where(o => o.Gid.Equals(value.SynchronousMachine)).FirstOrDefault();

						if (tableItem == null)
						{
							tableItem = new TableSMItem(timestamp, group.ToList());
							this._tableItemList.Add(tableItem);
						}

						tableItem.TimeStamp = timestamp;

                        EventSystem.Publish<DateTime>(timestamp);

						if (value.PowerType == PowerType.Active)
						{
							tableItem.CurrentP = ((AnalogValue)value).Value;
                            tableItem.PIncrease = ((AnalogValue)value).PowIncrease;
                            tableItem.PDecrease = ((AnalogValue)value).PowDecrease;
						}
						else
						{
							tableItem.CurrentQ = ((AnalogValue)value).Value;
                            tableItem.QIncrease = ((AnalogValue)value).PowIncrease;
                            tableItem.QDecrease = ((AnalogValue)value).PowDecrease;
                        }
					}
                }
            }

        }
    }
}
