using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class BooleanToSwitchText : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resource = new ResourceLoader();
            if (parameter != null)
            {
                if (value == null) return resource.GetString("TextoAtivado");
                return (bool)value
                    ? resource.GetString("CriarLancamentoPageFechado")
                    : resource.GetString("CriarLancamentoPageNaoFechado");

            }
            else
            {
                if (value == null) return resource.GetString("TextoAtivado");
                return (bool)value
                    ? resource.GetString("TextoAtivado")
                    : resource.GetString("TextoDesativado");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
