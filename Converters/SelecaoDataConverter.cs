using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class SelecaoDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = new ResourceLoader();
            if (value == null) return value;
            if (parameter == null)
                throw new ArgumentNullException("O parametro 'parameter', precisa ser informado com o nome da resource string.");

            var formatoData = resourceLoader.GetString("FormatoData");
            var data = value as DateTime?;

            var descricao = resourceLoader.GetString(parameter.ToString());
            var dataFormatada = data == DateTime.MinValue ? resourceLoader.GetString("CriarLancamentoPageDataLancamentoDescricaoVazia")
                                                            : data.Value.ToString(formatoData);

            return string.Format("{0} {1}", descricao, dataFormatada);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
            var valor = value as string;

            return DateTime.Parse(valor.Substring(valor.IndexOf(":") + 1));
            return value;
        }
    }
}
