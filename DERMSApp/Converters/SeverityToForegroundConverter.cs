using DERMSApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DERMSApp.Converters
{
	public class SeverityToForegroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush returnBrush = null;

			switch ((AlarmSeverity)value)
			{
				case AlarmSeverity.High:
					returnBrush = new SolidColorBrush(Colors.OrangeRed);
					break;
				case AlarmSeverity.Medium:
					returnBrush = new SolidColorBrush(Colors.Khaki);
					break;

				default:
					returnBrush = new SolidColorBrush(Colors.White);
					break;
			}

			return returnBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
