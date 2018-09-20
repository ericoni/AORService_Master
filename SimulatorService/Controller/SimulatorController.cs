using FTN.Common;
using FTN.Common.CE.Model;
using FTN.Common.RawConverter;
using FTN.Common.WeatherForecast.Model;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using MODBUSLibrary;
using ModBusSimulatorService.Model;
using PowerValuesSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModBusSimulatorService.Controller
{
    public class SimulatorController
    {
        private SimulatorModel model;

        private PowerCalculator pCalculator = new PowerCalculator();
        /// <summary>
        /// Simulator client
        /// </summary>
        private ModbusTcpClient modbusClient;

        /// <summary>
        /// Function to get random number
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Lock object
        /// </summary>
        private static readonly object syncLock = new object();

        /// <summary>
        /// Constant for min raw value
        /// </summary>
        private const float RAW_MIN = 0;

        /// <summary>
        /// Constant for max raw value
        /// </summary>
        private const float RAW_MAX = 4095;


        /// <summary>
        /// Constant for min egu value
        /// </summary>
        private const float EGU_MIN = -200;

        /// <summary>
        /// Constant for max egu value
        /// </summary>
        private const float EGU_MAX = 1500;

        /// <summary>
        /// Constructor 
        /// </summary>
        public SimulatorController()
        {
            model = SimulatorModel.Instance;
            OpenChannel();
        }

        /// <summary>
        /// This method is opening channel to the TCP Server
        /// </summary>
        public void OpenChannel()
        {
            try
            {
                this.modbusClient = new ModbusTcpClient();
                modbusClient.Connect();
                Console.WriteLine("Communication enabled...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Method to write single analog value.
        /// </summary>
        public void WriteSingleHoldingRegister(int registerAddress, float value)
        {
            modbusClient.WriteSingleHoldingRegister2(registerAddress, value);
        }

        /// <summary>
        /// Starting simulator
        /// </summary>
        public void StartSimulator()
        {
            //SimulatorForAnalogPoints(); //vrati se
        }

        /// <summary>
        /// Simulation of analog points changes 
        /// </summary>
        private void SimulatorForAnalogPoints()
        {
            while (true)
            {
                lock (SimulatorModel.Instance.Lock2PC)
                {
                    foreach (AnalogValue value in model.AnalogPoints)
                    {
                        SynchronousMachine der = model.Ders[value.SynchronousMachine];

                        if (!SimulatorModel.Instance.CurrentWeathers.ContainsKey(der.GlobalId))
                        {
                            Console.WriteLine(DateTime.Now + ": Weather forecast is not available for GlobalId: " + der.GlobalId);
                            continue;
                        }

                        CAS signal = SimulatorModel.Instance.ControlActiveSignals.Where(o => o.Gid.Equals(value.GlobalId)).FirstOrDefault();

                        if (signal != null)
                        {
                            if (signal.ControlledBy.Equals(CASEnum.Normal))
                            {
                                Console.WriteLine(DateTime.Now + ": Signal gid: {0}, Status: {1}", signal.Gid, signal.ControlledBy);
                                continue;
                            }
                        }

                        WeatherInfo wInfo = SimulatorModel.Instance.CurrentWeathers[der.GlobalId];

                        switch (der.FuelType)
                        {
                            case FuelType.Sun:
                                {
                                    if (value.PowerType == PowerType.Reactive)
                                    {
                                        float currentQ = 0;
                                        currentQ = RawValuesConverter.ConvertRange(currentQ, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX);
                                        WriteSingleHoldingRegister(value.Address, currentQ);
                                    }
                                    else
                                    {
                                        long sunriseTime = wInfo.Daily.Data.FirstOrDefault().SunriseTime;
                                        long sunsetTime = wInfo.Daily.Data.FirstOrDefault().SunsetTime;

                                        float currentP = pCalculator.GetActivePowerForSolarGenerator((float)wInfo.Currently.Temperature, (float)wInfo.Currently.CloudCover, der.NominalP, sunriseTime, sunsetTime);

                                        currentP = RawValuesConverter.ConvertRange(currentP, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX);
                                        WriteSingleHoldingRegister(value.Address, currentP);
                                    }
                                }

                                break;

                            case FuelType.Wind:
                                {
                                    float currentP = pCalculator.GetActivePowerForWindGenerator(der.NominalP, (float)(wInfo.Currently.WindSpeed));

                                    if (value.PowerType == PowerType.Reactive)
                                    {
                                        // 5% of active power
                                        float currentQ = currentP * 0.05f;

                                        currentQ = RawValuesConverter.ConvertRange(currentQ, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX);
                                        WriteSingleHoldingRegister(value.Address, currentQ);
                                    }
                                    else
                                    {
                                        currentP = RawValuesConverter.ConvertRange(currentP, EGU_MIN, EGU_MAX, RAW_MIN, RAW_MAX);
                                        WriteSingleHoldingRegister(value.Address, currentP);
                                    }
                                }

                                break;
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Fucntion to get random number
        /// </summary>
        /// <param name="min"> Min value </param>
        /// <param name="max"> Max value </param>
        /// <returns></returns>
        private float GetRandomNumber(float min, float max)
        {
            lock (syncLock)
            {
                var tempValue = (float)(random.NextDouble() * (max - min) + min);
                return tempValue;
            }
        }
    }
}
