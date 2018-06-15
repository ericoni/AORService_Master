using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using SmartCacheLibrary.Interfaces;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Meas;

namespace SmartCacheLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SmartCache : ICacheService
    {


        private static List<ICacheServiceCallback> _callbackList = new List<ICacheServiceCallback>();
        //  number of current users - 0 to begin with
        private static int _registeredUsers = 0;

        //instance of cache that changes when receive data from scada 
        //when changed it needs to notify all clients
        //public static Cache cache = new Cache();
        private Cache _cache = null;

        /// <summary>
        /// Singleton instanca
        /// </summary>
        private static volatile SmartCache instance;
        /// <summary>
        /// Objekat za lock
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Objekat za lock
        /// </summary>
        private static object lockSM = new Object();

        public SmartCache()
        {
            _callbackList = new List<ICacheServiceCallback>();
            _cache = new Cache();
        }

        /// <summary>
        /// Metoda za Singleton
        /// </summary>
        public static SmartCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SmartCache();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Get i Set za subscribers 
        /// </summary>
        public List<ICacheServiceCallback> CallbackList
        {
            get { return _callbackList; }
            set { _callbackList = value; }
        }

        public Cache Cache
        {
            get
            {
                lock (lockSM)
                {
                    return _cache;
                }
            }

            set
            {
                lock (lockSM)
                {
                    _cache = value;
                }
            }
        }


        public void ReceiveCache(string userName, List<string> addressList)
        {
			List<ICacheServiceCallback> removeService = new List<ICacheServiceCallback>();

            // Notify the users of a cache change.
            _callbackList.ForEach(
                delegate (ICacheServiceCallback callback)
                {
					try
					{
						callback.NotifyUserOfCache(userName, _cache);
					}
					catch(Exception)
					{
						removeService.Add(callback);
					}
				});

			foreach(ICacheServiceCallback service in removeService)
			{
				_callbackList.Remove(service);
			}
        }

        public int Register(string userName)
        {
            // Subscribe the user to the conversation
            ICacheServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<ICacheServiceCallback>();

            if (!_callbackList.Contains(registeredUser))
            {
                _callbackList.Add(registeredUser);
            }


            return _registeredUsers;
        }

        public int Unregister(string userName)
        {
            // Unsubscribe the user from the conversation.      
            ICacheServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<ICacheServiceCallback>();

            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
                _registeredUsers--;
            }

            return _registeredUsers;
        }
    }
}

