using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace DinDinPro.Universal.Services
{
    public class AlertMessageService : IAlertMessageService
    {
        private static bool _isShowing = false;

        public async Task ShowAsync(string message, string title)
        {
            await ShowAsync(message, title, null);
        }

        public async Task ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands)
        {
            // Only show one dialog at a time. 
            if (!_isShowing)
            {
                var messageDialog = new MessageDialog(message, title);

                if (dialogCommands != null)
                {
                    var commands = dialogCommands.Select(c => new UICommand(c.Label, (command) => c.Invoked(), c.Id));

                    foreach (var command in commands)
                    {
                        messageDialog.Commands.Add(command);
                    }
                }

                _isShowing = true;
                await messageDialog.ShowAsync();
                _isShowing = false;
            }
        }


        public async Task ShowContentAsync(string message)
        {
#if WINDOWS_PHONE_APP 
            ContentDialog c = new ContentDialog();            
            c.TitleTemplate = App.Current.Resources["MessageDialogTemplate"] as DataTemplate;// String.Join(Environment.NewLine, erros);
            c.IsPrimaryButtonEnabled = true;
            c.IsSecondaryButtonEnabled = true;
            c.PrimaryButtonText = "Sim";

            c.SecondaryButtonText = "Nao";
            c.Background = new SolidColorBrush(Colors.White);
            c.Foreground = new SolidColorBrush(Colors.Black);            
            c.Content = message;
            c.ContentTemplate = App.Current.Resources["MessageDialogContentTemplate"] as DataTemplate;// String.Join(Environment.NewLine, erros);
            
            await c.ShowAsync();
#endif

        }

    }
}
