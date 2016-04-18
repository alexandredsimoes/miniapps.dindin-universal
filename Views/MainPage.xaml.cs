using DinDinPro.Universal.Logger;
using DinDinPro.Universal.ViewModels;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sms;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DinDinPro.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MainPageViewModel _viewModel;

        public MainPage()
        {
            var r = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;

            if(r != null)
            {

            }
            this.InitializeComponent();
            _viewModel = DataContext as MainPageViewModel;
            this.Loaded += MainPage_Loaded;
            
            //if(App.LoginConfigurado && !App.AppLogado)
            //{
            //    //senhaUserControl.Height = this.Height;
            //    CommandBarMain.Visibility = Visibility.Collapsed;
            //}
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in Windows.Storage.ApplicationData.Current.LocalSettings.Values)
            {
                
                var v =  item.Value;

                Debug.WriteLine(v);
            }
            
            var tasks = BackgroundTaskRegistration.AllTasks.ToList();

             //if (!tasks.Any(c => c.Value.Name.Contains("SMSToExpense")))
               // await RegisterTaskConverter();

            if (!tasks.Any(c => c.Value.Name.Contains("DinDin Universal Background Task Utility")))
                await RegisterTask();

            if (!tasks.Any(c => c.Value.Name.Contains("DinDin Universal Background Anwser Task Utility")))
                await RegisterAnswerTask();
            

        }

        private async Task RegisterTaskConverter()
        {
            try
            {                
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = "SMSToExpense";



                SmsMessageType messageType = SmsMessageType.Text;
                SmsFilterRule filter = new SmsFilterRule(messageType);
                SmsFilterActionType actionType = SmsFilterActionType.Accept;

                SmsFilterRules filterRules = new SmsFilterRules(actionType);
                IList<SmsFilterRule> rules = filterRules.Rules;
                rules.Add(filter);

                SmsMessageReceivedTrigger trigger = new SmsMessageReceivedTrigger(filterRules);
                //ChatMessageReceivedNotificationTrigger trigger = new ChatMessageReceivedNotificationTrigger();

                taskBuilder.SetTrigger(trigger);

                taskBuilder.TaskEntryPoint = "DinDinPro.Universal.BackgroundService.SMSToExpense";
                var registration = taskBuilder.Register();
                registration.Completed += OnCompleted;                
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MainPage.xaml.cs - RegisterTaskConverter", ex);
            }
        }

        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Update the UI with the complrtion status reported by the background task.
            // Dispatch an anonymous task to the UI thread to do the update.
            await DispatcherHelper.RunAsync(
                () =>
                {
                    try
                    {
                        if ((sender != null) && (e != null))
                        {
                            // this method throws if the event is reporting an error
                            e.CheckResult();

                            // Update the UI with the background task's completion status.
                            // The task stores status in the application data settings indexed by the task ID.
                            var key = sender.TaskId.ToString();
                            var settings = ApplicationData.Current.LocalSettings;
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogs.WriteError("MainPage.xaml.cs - Register Task", ex);
                    }
                });
        }

        async Task RegisterTask()
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = "DinDin Universal Background Task Utility";
                //var t = new Windows.ApplicationModel.Background.SystemTrigger(SystemTriggerType.TimeZoneChange, false);
                TimeTrigger trigger = new TimeTrigger(30, false);
                taskBuilder.SetTrigger(trigger);

                taskBuilder.TaskEntryPoint = "DinDinPro.Universal.BackgroundService.CheckExpiredBillsTask";
                var registration = taskBuilder.Register();
                registration.Completed += (BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args) =>
                {
                    args.CheckResult();
                };
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MainPage.xaml.cs - Register Task", ex);
            }
        }

        async Task RegisterAnswerTask()
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = "DinDin Universal Background Anwser Task Utility";
                //var t = new Windows.ApplicationModel.Background.SystemTrigger(SystemTriggerType.TimeZoneChange, false);
                ToastNotificationActionTrigger trigger = new ToastNotificationActionTrigger();
                taskBuilder.SetTrigger(trigger);

                taskBuilder.TaskEntryPoint = "DinDinPro.Universal.BackgroundService.CheckExpiredBillsAnswerTask";
                var registration = taskBuilder.Register();
                registration.Completed += (BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args) =>
                {
                    args.CheckResult();
                };
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("MainPage.xaml.cs - RegisterAnswerTask", ex);
            }
        }
    }    
}
