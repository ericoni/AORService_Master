using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TestApp.Converter
{
	public class ConvertUsersCoveringAreaToText : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || (int)value <= 0)
			{
				return "Uncovered";
			}
			else
			{
				return "Covered";
			}
			//else
			//{
			//	return "Uncovered";
			//}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
