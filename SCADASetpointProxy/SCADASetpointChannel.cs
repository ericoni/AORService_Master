using FTN.Common.CalculationEngine.Model;
using FTN.Common.SCADA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADAReceivingProxyNS
{
    public class SCADASetpointChannel : ClientBase<ISCADAReceiving>, ISCADAReceiving
    {
        public SCADASetpointChannel()
			: base("SCADAReceivingEndpoint")
		{
        }
        public void ReceiveAndSetSetpoint(Setpoint setpoint)
        {
            this.Channel.ReceiveAndSetSetpoint(setpoint);
        }
    }
}
