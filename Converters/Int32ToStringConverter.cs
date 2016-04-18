﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class Int32ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return value;

            int result = 0;
            if (int.TryParse(value.ToString(), out result))
                return result;

            return null;
        }
    }
}
