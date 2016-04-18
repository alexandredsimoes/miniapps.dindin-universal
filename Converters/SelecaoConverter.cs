using DinDinPro.Universal.Models;
using DinDinPro.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public class SelecaoConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var resourceLoader = new ResourceLoader();
            if (value == null) return value;
            if (parameter == null)
                throw new ArgumentNullException("O parametro 'parameter', precisa ser informado com o nome da resource string.");

            var resourceString = resourceLoader.GetString(parameter.ToString());
            var item = value as ListaSelecao;

            if (item != null)
            {
                item.Descricao = string.Format("{0}: {1}", resourceString, item.Descricao);
                return item;
            }
            else
            {
                return string.Format("{0} {1}", resourceString, value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
