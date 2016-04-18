using DinDinPro.Universal.Logger;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace DinDinPro.Universal.Behaviors
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {

        public object Execute(object sender, object parameter)
        {
            try
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

                flyoutBase.ShowAt(senderElement);

                return flyoutBase;
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("OpenMenuFlyoutAction", ex);
                return null;
            }            
        }
    }
}
