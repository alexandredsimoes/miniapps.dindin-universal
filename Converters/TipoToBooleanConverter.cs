using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class TipoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(parameter == null) throw new ArgumentNullException("O parametro 'parameter', precisa ser '+' ou '-'");
            if (value == null) return false;

            return value.ToString().ToLower().Equals(parameter.ToString().ToLower());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return parameter;
            throw new NotImplementedException();
        }
    }
}
