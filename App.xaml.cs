using DinDinPro.Universal.Logger;
using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Views;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;
using Windows.Globalization.DateTimeFormatting;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace DinDinPro.Universal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        class DadosDespesa
        {
            public double Valor;
            public string Origem;
            public DateTime Data;
            public string Tipo;
        }

        // Create the singleton container that will be used for type resolution in the app         
        //public static string DatabasePath = "dindin.pro.sqlite";
        public static bool LoginConfigurado = false;
        public static string PIN = String.Empty;
        private int VERSAO = 1;

        private IConfiguracaoRepository _configurationRepository;

        public static bool AppLogado { get; set; }
        public static bool ExibirAds { get; internal set; }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;

            //Application.Current.RequestedTheme = ApplicationTheme.Dark;
            UnhandledException += App_UnhandledException;

            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            //CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            //#if DEBUG
            //            string sInput = @"Realizado pagamento de VIVO-SP 0246633009 no valor de R$ 34,99 na sua conta Itau XXX98-2 em 23/10 as 08:38.";

            //            /*
            //            ITAU DEBITO: Cartao final 5737 COMPRA APROVADA 26/10 19:46:29 R$ 50,00 Local: POSTO AGU. Consulte tambem pelo celular www.itau.com.br.

            //            Compra aprovada no seu ITAU MULT 2.0 MC GOL final 5737 - NETFLIX.COM valor RS 19,90 em 24/10, as 08h47.

            //            Realizado pagamento de VIVO-SP 0246633009 no valor de R$ 34,99 na sua conta Itau XXX98-2 em 23/10 as 08:38.

            //            */

            //            if (sInput.Length > 0)
            //            {

            //                var retorno = new DadosDespesa();
            //                DateTimeFormatter format = new DateTimeFormatter("{day.integer(2)}‎/‎{month.integer(2)}‎/‎{year.full} {hour.integer}‎:‎{minute.integer(2)}‎:‎{second.integer(2)}");

            //                var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //                for (int i = 0; i < s.Length; i++)
            //                {
            //                    if (s[i] == "de" && String.IsNullOrWhiteSpace(retorno.Origem))
            //                    {
            //                        for (int x = i + 1; x < s.Length; x++)
            //                        {
            //                            if (s[x] == "no") break;

            //                            retorno.Origem += s[x] + " ";
            //                        }


            //                    }

            //                    if (s[i] == "R$")
            //                    {
            //                        retorno.Valor = double.Parse(s[i + 1], new CultureInfo("pt-BR"));
            //                    }

            //                    if (s[i] == "em")
            //                    {
            //                        var data = s[i + 1].Replace(",", "") + "/" + DateTime.Now.Year.ToString() + " ";
            //                        var hora = s[i + 3].Replace("h", ":");
            //                        //Tenta obter a data                                               
            //                        retorno.Data = DateTime.Parse(string.Concat(data, hora), new CultureInfo("pt-BR"));
            //                    }
            //                }

            //                if (retorno != null)
            //                {

            //                }
            //            }
            //#endif
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                Debug.WriteLine(e.Exception.Message.ToString().ToUpper());
                Exception exception = e.Exception;
                if (exception is NullReferenceException && exception.ToString().ToUpper().Contains("SOMA"))
                {
                    AppLogs.WriteError("App", exception);
                    e.Handled = true;
                    return;
                }
                else
                {
                    Debug.WriteLine(exception.Message, "ERRO CAPTURADO");
                    AppLogs.WriteError("App", exception);
                    e.Handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.Kind == ActivationKind.Launch)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constantes.CONFIG_KEY_EXECUCOES))
                {
                    var qtde = 1;
                    if (int.TryParse(ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES].ToString(), out qtde))
                    {
                        if(qtde != -1)
                        {
                            qtde++;
                            ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES] = qtde;
                        }                        
                    }
                    else
                        ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES] = 1;
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values.Add(Constantes.CONFIG_KEY_EXECUCOES, 1);
                }

            }
            //Efetua a verificação da licença do usuário (remover ADS)
            VerificarLicensa();

            var dataService = new DataService();
            if (e.PreviousExecutionState == ApplicationExecutionState.NotRunning)
            {
                var resourceLoader = new ResourceLoader();

                //var dataService = new DataService();
                await dataService.InicializarBaseDeDados(
                                                            resourceLoader.GetString("CriarLancamentoPageTagsReceitas").Split(new char[] { ',' }),
                                                            resourceLoader.GetString("CriarLancamentoPageTagsDespesas").Split(new char[] { ',' }),
                                                            resourceLoader.GetString("CriarLancamentoPageFormasPagamento").Split(new char[] { ',' }),
                                                            VERSAO
                                                            );

            }
            var _configurationRepository = new ConfiguracaoRepository(dataService);

            //Verifica se foi configurado uma senha para o aplicativo
            if (_configurationRepository != null)
            {
                PIN = await _configurationRepository.ObterValor("senha");
                if (!String.IsNullOrWhiteSpace(PIN)) App.LoginConfigurado = true;
            }


            var shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Shell which navigates to the first page
                shell = new AppShell(e.PreviousExecutionState);

                // hook-up shell root frame navigation events
                shell.AppFrame.NavigationFailed += OnNavigationFailed;
                shell.AppFrame.Navigated += OnNavigated;
                //shell.AppFrame.Navigating += OnNavigating;



                // set the Shell as content
                Window.Current.Content = shell;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NavigationState"))
                    {
                        //Pega a pagina via Expressão Regular
                        //DinDinPro.Universal.Views.\w+
                        shell.AppFrame.SetNavigationState((string)ApplicationData.Current.LocalSettings.Values["NavigationState"]);
                    }
                }

                // listen for back button clicks (both soft- and hardware)
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed += OnBackPressed;
                }

                UpdateBackButtonVisibility();
            }

            DispatcherHelper.Initialize();
            // Ensure the current window is active
            Window.Current.Activate();

        }

        private void VerificarLicensa()
        {
            //ApplicationData.Current.RoamingSettings.Values.Clear();
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("DINDIN_UNIVERSAL_SEM_ADS"))
                ExibirAds = !(bool)ApplicationData.Current.RoamingSettings.Values["DINDIN_UNIVERSAL_SEM_ADS"];
            else
                ExibirAds = true;

        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            //Caso o login esteja configurado e não esteja logado,
            //exibe a tela de login
            if (e.SourcePageType != typeof(LoginPage) && LoginConfigurado && !App.AppLogado)
            {
                var shell = Window.Current.Content as AppShell;
                //shell.AppFrame.Navigate(typeof(LoginPage), null);
                Window.Current.Content = new LoginPage();
                e.Cancel = true;
            }
        }

        // handle hardware back button press
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var shell = (AppShell)Window.Current.Content;
            if (shell.AppFrame.CanGoBack)
            {
                e.Handled = true;
                shell.AppFrame.GoBack();
            }
        }

        // handle software back button press
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var shell = (AppShell)Window.Current.Content;
            if (shell.AppFrame.CanGoBack)
            {
                e.Handled = true;
                shell.AppFrame.GoBack();
            }
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity


            var shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                var frame = shell.AppFrame;
                if (frame != null)
                {
                    ApplicationData.Current.LocalSettings.Values["NavigationState"] = frame.GetNavigationState();
                }
            }

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {

        }

        private void UpdateBackButtonVisibility()
        {
            var shell = (AppShell)Window.Current.Content;

            var visibility = AppViewBackButtonVisibility.Collapsed;
            if (shell.AppFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }
    }
}
