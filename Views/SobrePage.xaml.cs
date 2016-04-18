using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DinDinPro.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SobrePage : Page
    {
        private ResourceLoader _resourceLoader;

        public SobrePage()
        {
            this.InitializeComponent();
            _resourceLoader = SimpleIoc.Default.GetInstance<ResourceLoader>();
            PackageVersion version = Package.Current.Id.Version;
            

            VersaoTextBlock.Text = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            RateAppButton.Click += RateAppButton_Click;
            SobreFaleDevButton.Click += SobreFaleDevButton_Click;
        }

        private async void SobreFaleDevButton_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage message = new EmailMessage();
            message.Subject = _resourceLoader.GetString("SobreEmailAssunto");
            message.Body = _resourceLoader.GetString("SobreEmailBody");
            message.To.Add(new EmailRecipient(_resourceLoader.GetString("SobreEmail")));
            await EmailManager.ShowComposeNewEmailAsync(message);
        }

        private async void RateAppButton_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(string.Format("ms-windows-store://review/?PFN={0}", Package.Current.Id.FamilyName));
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
