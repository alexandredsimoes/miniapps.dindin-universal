using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class InvertBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
