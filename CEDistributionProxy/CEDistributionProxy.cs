using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CEDistributionProxy
{
    public class ICEDistributionProxy
    {
        // Broj pokusaja uspostavljanja komunikacije
        private const int maxTry = 5;

        // Spavanje do narednog pokusaja
        private const int sleepTime = 3000;

        CEDistributionChannel proxy;

        public ICEDistributionProxy()
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
                    throw new Exception("CEDistributionProxy: Connection error.");
                }

                try
                {
                    proxy = new CEDistributionChannel();
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

        public CEDistributionChannel Proxy
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
