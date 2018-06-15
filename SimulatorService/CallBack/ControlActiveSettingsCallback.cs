using FTN.Common.CE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common.CE.Model;
using ModBusSimulatorService.Model;

namespace ModBusSimulator.Callback
{
    public class ControlActiveSettingsCallback : IControlActiveSettingsCallback
    {
        public void ControlActiveSettings(List<CAS> signals)
        {
            foreach (CAS signal in signals)
            {
                CAS temp = SimulatorModel.Instance.ControlActiveSignals.FirstOrDefault(o => o.Gid.Equals(signal.Gid));

                if (temp == null)
                {
                    SimulatorModel.Instance.ControlActiveSignals.Add(signal);
                    continue;
                }

                temp.ControlledBy = signal.ControlledBy;
            }
        }
    }
}
