using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTN.Common.RawConverter
{
	public class RawValuesConverter
	{
		/// <summary>
		/// Converts one range to another
		/// </summary>
		/// <param name="value"> Value to be converted </param>
		/// <param name="oldMin"> Old value minimum </param>
		/// <param name="oldMax"> Old value maximum</param>
		/// <param name="newMin"> New value minimum</param>
		/// <param name="newMax"> New value maximum </param>
		/// <returns></returns>
		public static float ConvertRange(float value, float oldMin, float oldMax, float newMin, float newMax)
		{
			return ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
		}
	}
}
