using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class PeriodoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;

            var periodo = value.ToString();
            return periodo == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;

            var periodo = value as bool?;
            return periodo ?? false ? parameter.ToString() : String.Empty;
        }
    }
}
