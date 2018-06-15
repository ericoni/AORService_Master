using FTN.Common.SCADA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Meas;
using SCADAService;

namespace SCADA
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SCADASubscriber : ISCADASubscriber
    {
        /// <summary>
        /// Singleton instanca
        /// </summary>
        private static volatile SCADASubscriber instance;

        /// <summary>
        /// Objekat za lock
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// Lista subscribera zainteresovani za Scada podatke
        /// </summary>
        private Dictionary<ISCADAForwarding, List<DMSType>> subscribers;

        public SCADASubscriber()
        {
            subscribers = new Dictionary<ISCADAForwarding, List<DMSType>>();
        }

        /// <summary>
        /// Metoda za Singleton
        /// </summary>
        public static SCADASubscriber Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SCADASubscriber();
                    }
                }

                return instance;
            }
        }

        public void Subscribed(List<DMSType> topics)
        {
            OperationContext context = OperationContext.Current;
            ISCADAForwarding client = context.GetCallbackChannel<ISCADAForwarding>();

            if (!subscribers.ContainsKey(client))
            {
                subscribers.Add(client, topics);
            }

            foreach (DMSType topic in topics)
            {
                if (subscribers[client].Contains(topic))
                {
                    continue;
                }
                subscribers[client].Add(topic);
            }
        }

        public void Unsubscribed(List<DMSType> topics)
        {
            OperationContext context = OperationContext.Current;
            ISCADAForwarding client = context.GetCallbackChannel<ISCADAForwarding>();

            if (!subscribers.ContainsKey(client))
            {
                return;
            }

            foreach (DMSType topic in topics)
            {
                if (!subscribers[client].Contains(topic))
                {
                    continue;
                }
                subscribers[client].Remove(topic);
            }

            if (subscribers[client].Count != 0)
            {
                return;
            }

            subscribers.Remove(client);
        }

        public void NotifySubscribers(DMSType dmsType)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (ISCADAForwarding client in subscribers.Keys)
            {
                if (!subscribers[client].Contains(dmsType))
                {
                    continue;
                }

                switch (dmsType)
                {
                    case DMSType.ANALOGVALUE:
                        List<AnalogValue> analogValues = SCADAModel.Instance.AnalogValues;

                        try
                        {
                            client.GetAnalogScadaData(analogValues);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ScadaSubscriber: {0}", ex.Message);
                        }

                        break;
                }
            }
        }
    }
}
