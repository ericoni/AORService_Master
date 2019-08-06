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
            //var a = (short)value;
            //if ((short)value == 0)
            //{
            //    return new SolidColorBrush(Colors.SeaGreen);//PaleGreen
            //}

            //return new SolidColorBrush(Colors.Yellow);//PaleGreen
            System.Windows.Media.Brush brushReturn = System.Windows.Media.Brushes.Red;
            return brushReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
