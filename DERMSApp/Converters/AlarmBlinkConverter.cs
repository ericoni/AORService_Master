using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DERMSApp.Converters
{
	public class AlarmBlinkConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//if ((int)value > 0)
			//{
			//	return FetchStyle("FaderStyleButton");
			//}
			//return null;

			return FetchStyle("FaderStyleButton");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		Style FetchStyle(string name)
		{
			return Application.Current.Resources[name] as Style;
		}
	}
}
