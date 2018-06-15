using FTN.Common;
using FTN.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NMSSub
{
    public class NMSSubscriber
    {
        /// <summary>
        /// Channel for NMS pub/sub
        /// </summary>
        private INMSSubscriber proxy = null;

        private object instance = null;

        // Broj pokusaja uspostavljanja komunikacije
        private const int maxTry = 5;

        // Spavanje do narednog pokusaja
        private const int sleepTime = 3000;

        /// <summary>
        /// Constructor
        /// </summary>
        public NMSSubscriber(object instance)
        {
            this.instance = instance;
            OpenChannel();
        }

        /// <summary>
        /// This method is opening channel to the NMS
        /// </summary>
        private void OpenChannel()
        {
            DuplexChannelFactory<INMSSubscriber> factory = new DuplexChannelFactory<INMSSubscriber>(
              new InstanceContext(instance),
              new NetTcpBinding(),
              new EndpointAddress("net.tcp://localhost:10010/INMSSubscriber"));

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
                    throw new Exception("TSDBProxy: Connection error.");
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
                    throw new Exception("TSDBProxy: Connection error.");
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
