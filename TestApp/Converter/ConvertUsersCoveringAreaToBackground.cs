using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TestApp.Converter
{
	public class ConvertUsersCoveringAreaToBackground : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return new SolidColorBrush(System.Windows.Media.Colors.Aqua);
			}

			if ((int)value > 0)
			{
				return new SolidColorBrush(System.Windows.Media.Colors.ForestGreen);
			}
			else
			{
				return new SolidColorBrush(System.Windows.Media.Colors.Red);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
