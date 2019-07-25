using EventCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EventAlarmService
{
	/// <summary>
	/// Prima poruke od AOR management servisa.Service class made for event aggregation.
	/// Implements <seealso cref="IDERMSEventCollector"/> interface. 
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class DERMSEventCollector : IDERMSEventCollector
	{
		static DERMSEventCollector instance;
        private static object syncRoot = new Object();

        public static DERMSEventCollector Instance
		{
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DERMSEventCollector();
                    }
                }

                return instance;
            }
        }
		public void SendEvent(Event e)
		{
			//Event ev = new Event("randomUsername", "randomDetalji", DateTime.Now);
			DERMSEventSubscription.Instance.NotifyClients(7, e);//Problem ovo dovodi do instanciranja novog objekta (iako bi ne bi trebalo zbog postojeceg pa sam uveo static polja tamo)
		}
	}
}
