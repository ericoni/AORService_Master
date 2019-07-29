using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommonForUI.Converters
{
    public class CoveredToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                    return new SolidColorBrush(Colors.SeaGreen);//PaleGreen
            }
            return new SolidColorBrush(Colors.DimGray);//Salmon (dodje kao neka crvena) DarkGray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
