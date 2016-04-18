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
    public class BrushTipoLancamentoConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new SolidColorBrush(Color.FromArgb(167, 0, 0, 0));

            return value.ToString().Equals("receita") ? new SolidColorBrush(Color.FromArgb(200, 3, 79, 24)) : new SolidColorBrush(Color.FromArgb(200, 230, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
