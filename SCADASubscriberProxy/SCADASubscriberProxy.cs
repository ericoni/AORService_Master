using FTN.Common;
using FTN.Common.SCADA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCADASubscriberProxyNS
{
    public class SCADASubscriberProxy
    {
        /// <summary>
        /// Channel for Scada pub/sub
        /// </summary>
        private ISCADASubscriber proxy = null;

        private object instance = null;

        // Broj pokusaja uspostavljanja komunikacije
        private const int maxTry = 5;

        // Spavanje do narednog pokusaja
        private const int sleepTime = 3000;

        /// <summary>
        /// Constructor
        /// </summary>
        public SCADASubscriberProxy(object instance)
        {
            this.instance = instance;
            OpenChannel();
        }

        /// <summary>
        /// This method is opening channel to the NMS
        /// </summary>
        private void OpenChannel()
        {
            DuplexChannelFactory<ISCADASubscriber> factory = new DuplexChannelFactory<ISCADASubscriber>(
              new InstanceContext(instance),
              new NetTcpBinding(),
              new EndpointAddress("net.tcp://localhost:10011/ISCADASubscriber"));

            try
            {
                proxy = factory.CreateChannel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Subscribe to the desired type
        /// </summary>
        /// <param name="topics"> List of DMSTypes </param>
        public void Subscribed(List<DMSType> topics)
        {
            int tryCounter = 0;

            while (true)
            {
                if (tryCounter.Equals(maxTry))
                {
                    throw new Exception("SCADASubscriberProxy: Connection error.");
                }

                try
                {
                    proxy.Subscribed(topics);
                    break;
                }
                catch (Exception)
                {
                    tryCounter++;
                    Thread.Sleep(sleepTime);
                    OpenChannel();
                }
            }
        }

        /// <summary>
        /// Unsubscribe
        /// </summary>
        /// <param name="topics"> List of DMS Types </param>
        public void Unsubscribed(List<DMSType> topics)
        {
            int tryCounter = 0;

            while (true)
            {
                if (tryCounter.Equals(maxTry))
                {
                    throw new Exception("SCADASubscriberProxy: Connection error.");
                }

                try
                {
                    proxy.Unsubscribed(topics);
                    break;
                }
                catch (Exception)
                {
                    tryCounter++;
                    Thread.Sleep(sleepTime);
                    OpenChannel();
                }
            }
        }
    }
}
