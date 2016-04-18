using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace DinDinPro.Universal.Converters
{
    public sealed class StringDateFormatMainPageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return String.Empty;

            if (parameter == null)
                return value;


            //Converte a data
            var data = DateTime.MinValue;

            if(DateTime.TryParse(value.ToString(), out data))
            {
                if(data.Year > DateTime.Now.Year)
                {
                    var resource = new ResourceLoader();                    
                    return resource.GetString("MainPageFutureExpenses");
                }

                return string.Format((string)parameter, value);
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {

            return value;
        }
    }
}
