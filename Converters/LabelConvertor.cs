using DinDinPro.Universal.Models;
using DinDinPro.Universal.ViewModels;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class Labelconvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ChartAdornment)
            {
                ChartAdornment pieAdornment = value as ChartAdornment;
                if (pieAdornment.Item != null)
                    return String.Format((pieAdornment.Item as GraficoDespesa).Despesa + ": " + pieAdornment.YData.ToString("n2"));
                else
                    return value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
