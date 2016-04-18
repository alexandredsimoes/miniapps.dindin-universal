using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class DataParaExtensoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resource = new ResourceLoader();
            DateTime? data = DateTime.MinValue;

            if (value is Lancamento)
            {
                data = ((Lancamento)value).DataLancamento;
            }
            else
                data = value as DateTime?;


            if (data == null) return "";

            if (data == DateTime.MinValue) return resource.GetString("DataExtensoSaldoAnterior");

            if ((string)parameter == "0")
                return data.Value.ToString();

            if (data.Value.Date == DateTime.Now.Date)
                return resource.GetString("DataExtensoHoje");
            if (data.Value.Date == DateTime.Now.AddDays(-1).Date)
                return resource.GetString("Ontem");
            //if (data.Value.Date > DateTime.Now.Date)
            //    return resource.GetString("Previsto");

            return data.Value.ToString(resource.GetString("FormatoData"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
