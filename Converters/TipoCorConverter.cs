using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DinDinPro.Universal.Converters
{
    public class TipoCorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            string valor = "";
            if (value == null) return new SolidColorBrush(Color.FromArgb(167, 0, 0, 0));

            if (value is string)
            {
                valor = value.ToString();
            }
            return (valor == "+" || valor == "receita") ? new SolidColorBrush(Color.FromArgb(0xFF, 0x1A, 0x78, 0x34))
                : new SolidColorBrush(Color.FromArgb(0xFF, 0xB6, 0x1E, 0x1E));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
