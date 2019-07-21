using FTN.Common.CE.Model;
using FTN.Common.CE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CalculationEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CASSubscriber : IControlActiveSettingsSubscriber
    {
        /// <summary>
        /// Singleton instanca
        /// </summary>
        private static volatile CASSubscriber instance;

        /// <summary>
        /// Objekat za lock
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Lista subscribera 
        /// </summary>
        private List<IControlActiveSettingsCallback> subscribers;

        public CASSubscriber()
        {
            subscribers = new List<IControlActiveSettingsCallback>();
        }

        /// <summary>
        /// Metoda za Singleton
        /// </summary>
        public static CASSubscriber Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CASSubscriber();
                    }
                }

                return instance;
            }
        }
        #region IControlActiveSettingsSubscriber
        public void Subscribed()
        {
            OperationContext context = OperationContext.Current;
            IControlActiveSettingsCallback client = context.GetCallbackChannel<IControlActiveSettingsCallback>();

            if (!subscribers.Contains(client))
            {
                subscribers.Add(client);
            }
        }

        public void Unsubscribed()
        {
            OperationContext context = OperationContext.Current;
            IControlActiveSettingsCallback client = context.GetCallbackChannel<IControlActiveSettingsCallback>();

            if (!subscribers.Contains(client))
            {
                subscribers.Remove(client);
            }
        }
        #endregion IControlActiveSettingsSubscriber

        public void NotifySubscribers(List<CAS> signals)
        {
            List<IControlActiveSettingsCallback> removeList = new List<IControlActiveSettingsCallback>();

            foreach (IControlActiveSettingsCallback callback in subscribers)
            {
                try
                {
                    callback.ControlActiveSettings(signals);
                }
                catch (Exception)
                {
                    removeList.Add(callback);
                }
            }

            foreach (IControlActiveSettingsCallback callback in removeList)
            {
                subscribers.Remove(callback);
            }
        }
    }
}
