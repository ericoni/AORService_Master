using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerValuesSimulator
{
    public class PowerCalculator
    { 
        /// <summary>
        /// Max INS [W/m2]
        /// </summary>
        private const double maxINS = 1050;

		private object  lockSync;

        /// <summary>
        /// Min INS [W/m2]
        /// </summary>
        private const double minINS = 0;

        public PowerCalculator()
        {
			lockSync = new object();
        }
        /// <summary>
        /// Function to generate wind generator active power by speed of the wind
        /// </summary>
        /// <param name="Vws"></param>
        /// <returns></returns>
        public float GetActivePowerForWindGenerator(float Pn, float Vws)
        {
			lock (lockSync)
			{
				float Pws = 0;

				if (Vws >= 3.5 && Vws < 14)
				{
					Pws = (Vws - 3.5f) * 0.035f * Pn;
				}
				else if (Vws >= 14 && Vws <= 25)
				{
					Pws = Pn;
				}

				return Pws;
			}
        }

        /// <summary>
        /// Function to generate wind generator active power by speed of the wind
        /// </summary>
        /// <param name="windSpeed"></param>
        /// <returns></returns>
        //public float GetActivePowerForWindGenerator(float windSpeed)
        //{
        //	//turbine output active power [W]
        //	float Pwtb = 0;

        //	//air density [kg/m3]
        //	float rho = 1.225f;

        //	//speed of the wind [m/s]
        //	float Vw = windSpeed;

        //	//radius of the blade [m]
        //	//diametar of 100m for example
        //	float R = 200;

        //	//tip speed ratio
        //	/*----------------------
        //	 NumOfBlades - OptimumTSR
        //	 2			 -	6
        //	 3			 -  4-5
        //	 4			 -  3
        //	 6			 -  2
        //	 -------------------------*/
        //	float lambda = 6;

        //	//power coefficient
        //	float Cp = 0;

        //	//wind turbine angular speed [rad/s]
        //	//double omega = 0;

        //	//blade pich angle [deg]
        //	float beta = 0;

        //	// if wind speed is < 3.5 active power is 0,
        //	//also if wind speed is over 24 we must turn blades and active pover is 0
        //	if ((Vw >= 2.5) && (Vw <= 25))
        //	{
        //		Cp = 0.5f * (((R / lambda) * (3600 / 1609)) - 0.022f * (float)Math.Pow(beta, 2) - 5.6f) * (float)Math.Exp(-0.17 * ((R / lambda) * (3600 / 1609)));

        //		Pwtb = 0.5f * rho * Cp * 3.14f * (float)Math.Pow(R, 2) * (float)Math.Pow(Vw, 3);
        //	}

        //	return Pwtb;
        //}

        /// <summary>
        /// Function to generate solar generator active power by air temperature,
        /// amount of sunlight and nominal active power of generator
        /// </summary>
        /// <param name="airTemperature"></param>
        /// <param name="cloudyness"></param>
        /// <param name="nominalP"></param>
        /// <returns></returns>
        public float GetActivePowerForSolarGenerator(float airTemperature, float cloudyness, float nominalP, long sunriseTime, long sunsetTime)
        {
            //cell temperature [oC]
            float Tcell = 25;


            //amount of sunlight
            float INS = Convert.ToSingle(GetINS(sunriseTime, sunsetTime, cloudyness));

            //active power of solar generator [W]
            float Psolar = 0;

            if (airTemperature < 25)
            {
                Tcell = airTemperature + 0.025f * INS;
            }

            Psolar = nominalP * INS * 0.00095f * (1 - 0.005f * (Tcell - 25));

            return Psolar;
        }

        public double GetINS(long sunriseTime, long sunsetTime, float cloudyness)
        {
            long currentTime = DateTime.Now.Ticks;

            if (currentTime < sunriseTime || currentTime > sunsetTime)
            {
                return 0;
            }

            if (cloudyness == 1)
            {
                cloudyness = 0.9f;
            }

            float sunshine = 1 - cloudyness;

            // Dnevni pik, kad je najjace sunce
            long peakTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0).Ticks;

            double INS = 0;

            if (currentTime <= peakTime)
            {
                INS = (((double)currentTime - (double)sunriseTime) / ((double)peakTime - (double)sunriseTime)) * (maxINS - minINS) + minINS;
            }
            else
            {
                INS = (((double)currentTime - (double)sunsetTime) / ((double)peakTime - (double)sunsetTime)) * (maxINS - minINS) + minINS;
            }

            INS = sunshine * INS;

            return INS;
        }
    }
}
