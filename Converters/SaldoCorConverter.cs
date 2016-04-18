using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace DinDinPro.Universal.Converters
{
    public  class SaldoCorConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader;
       
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || (double.Parse(value.ToString()) == 0))
                return App.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush;

            return double.Parse(value.ToString()) > 0 ? new SolidColorBrush(Color.FromArgb(255, 45, 211, 8)) : new SolidColorBrush(Color.FromArgb(255, 222, 9, 15));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
