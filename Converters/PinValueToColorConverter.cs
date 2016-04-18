using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DinDinPro.Universal.Converters
{
    public class PinValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
            //return new SolidColorBrush(Color.FromArgb(70, 118, 159, 169)); //Vazio

            return App.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
