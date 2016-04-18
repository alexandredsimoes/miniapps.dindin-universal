using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DinDinPro.Universal.UserControls
{
    public class SelecaoDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyTemplate { get; set; }
        public DataTemplate MyTemplate2 { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is ContentPresenter)
                return MyTemplate2;
            return MyTemplate;
        }
    }
}
