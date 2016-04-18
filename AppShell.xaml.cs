using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Automation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DinDinPro.Universal.Views;
using DinDinPro.Universal.UserControls;
using Windows.UI.ViewManagement;
using DinDinPro.Universal.ViewModels;
using DinDinPro.Universal.Models;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;

namespace DinDinPro.Universal
{
    /// <summary>
    /// The "chrome" layer of the app that provides top-level navigation with
    /// proper keyboarding navigation.
    /// </summary>
    public sealed partial class AppShell : Page
    {
        public ApplicationExecutionState ModoExecucao { get; set; }

        public AppShell(ApplicationExecutionState modoExecucao)
        {
            this.InitializeComponent();
            //BannerAD.Visibility = App.ExibirAds ? Visibility.Visible : Visibility.Collapsed;
            ModoExecucao = modoExecucao;

            var vm = new ShellViewModel();
            this.ViewModel = vm;

            //vm.MenuItems.Add(new MenuItem { Icon = "", Title = "Page 1", PageType = typeof(Page1) });
            //vm.MenuItems.Add(new MenuItem { Icon = "", Title = "Page 2", PageType = typeof(Page2) });
            //vm.MenuItems.Add(new MenuItem { Icon = "", Title = "Page 3", PageType = typeof(Page3) });

            if (App.LoginConfigurado && !App.AppLogado)
            {
                vm.SelectedMenuItem = new MenuItem() { Icon = "", PageType = typeof(LoginPage), Title = "Login" };
            }
            else
            {
                CarregarMenus();

                if (ModoExecucao != ApplicationExecutionState.Terminated)
                vm.SelectedMenuItem = vm.MenuItems.First();

                
                  //  this.AppFrame.GoBack();
            }
            Loaded += AppShell_Loaded;

        }

        internal void CarregarMenus()
        {
            var resourceLoader = new ResourceLoader();
            ViewModel.MenuItems.Add(new MenuItem { Icon = "", Title = resourceLoader.GetString("ShellMenuHome"), PageType = typeof(MainPage) });
            ViewModel.MenuItems.Add(new MenuItem { Icon = "", Title = resourceLoader.GetString("ShellMenuAccounts"), PageType = typeof(ContaListPage) });
            //ViewModel.MenuItems.Add(new MenuItem { Icon = "", Title = "Relatórios", PageType = typeof(RelatorioPage) });
            ViewModel.MenuItems.Add(new MenuItem { Icon = "", Title = resourceLoader.GetString("ShellMenuConfigurations"), PageType = typeof(ConfiguracaoPage) });            
        }

        private async void AppShell_Loaded(object sender, RoutedEventArgs e)
        {
            //Manipulando o botão voltar (apenas para mobile)
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

                // Hide the status bar
                await statusBar.HideAsync();
            }
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame AppFrame
        {
            get
            {
                return this.Frame;
            }
        }
    }
}
