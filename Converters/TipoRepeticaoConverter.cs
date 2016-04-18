using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;


namespace DinDinPro.Universal.Converters
{
    public class TipoRepeticaoConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return String.Empty;

            var resource = new ResourceLoader();

            //Nunca repetir,Repetição diária,Repetição semanal,Repetição quinzenal,Repetir todo mês,Repetição anual,Repetir em dias úteis,Repetir em fins de semana
            var periodos = resource.GetString("CriarLancamentoPageListaRepeticoes").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            switch (value.ToString())
            {
                case "0":
                    return periodos[0];
                case "1":
                    return periodos[1];
                case "2":
                    return periodos[2];
                case "3":
                    return periodos[3];
                case "4":
                    return periodos[4];
                case "5":
                    return periodos[5];
                case "6":
                    return periodos[6];
                case "7":
                    return periodos[7];
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
