using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class DateTimeParaDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return DateTimeOffset.MinValue;
            var data = DateTimeOffset.Parse(value.ToString());
            
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return DateTime.Now;
            var data = value as DateTimeOffset?;

            return data.Value.DateTime;            
        }
    }
}
