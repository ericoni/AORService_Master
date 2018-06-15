using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCADAReceivingProxyNS
{
    public class SCADASetpointProxy
    {
        // Broj pokusaja uspostavljanja komunikacije
        private const int maxTry = 5;

        // Spavanje do narednog pokusaja
        private const int sleepTime = 3000;

        SCADASetpointChannel proxy;

        public SCADASetpointProxy()
        {
            OpenChannel();
        }

        private void OpenChannel()
        {
            int tryCounter = 0;

            while (true)
            {
                if (tryCounter.Equals(maxTry))
                {
                    throw new Exception("SCADASetpointProxy: Connection error.");
                }

                try
                {
                    proxy = new SCADASetpointChannel();
                    proxy.Open();

                    break;
                }
                catch (Exception)
                {
                    tryCounter++;
                    Thread.Sleep(sleepTime);
                }
            }
        }

        public SCADASetpointChannel Proxy
        {
            get
            {
                if (proxy.State != System.ServiceModel.CommunicationState.Opened)
                {
                    OpenChannel();
                }

                return proxy;
            }
        }
    }
}
