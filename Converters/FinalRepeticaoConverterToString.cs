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
    public class FinalRepeticaoConverterToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;
            var resource = new ResourceLoader();

            //Nunca,Data específica,Depois de x repetições
            var periodos = resource.GetString("CriarLancamentoPageListaRepeticoesFinal").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            switch (value.ToString())
            {
                case "0":
                    return periodos[0];
                case "1":
                    return periodos[1];
                case "2":
                    return periodos[2];
                default:
                    return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
