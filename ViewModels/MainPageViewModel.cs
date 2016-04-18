using DinDinPro.Universal.Aggregators;
using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DinDinPro.Universal.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        private readonly IAlertMessageService _alertMessageService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IPinInputPageViewModel _pinInputPageViewModel;

        public MainPageViewModel(IDataService dataService, INavigationService navigationService, IContaRepository contaRepository,
            IAlertMessageService alertMessageService, ResourceLoader resourceLoader,
            IPinInputPageViewModel pinInputPageViewModel)
        {

            _DadosGrafico = new ObservableCollection<GraficoData>();
            //_DadosGrafico.Add(new GraficoData() { Data = DateTime.Now, Despesa = 100, Receita = 50 });
            //_DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(-1), Despesa = 1010, Receita = 150 });
            //_DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(1), Despesa = 10, Receita = 150 });
            //_DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(2), Despesa = 0, Receita = 510 });

            LoginMode = (App.LoginConfigurado && !App.AppLogado);
            //Periodo = DateTime.Now;

            _dataService = dataService;
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _alertMessageService = alertMessageService;
            _resourceLoader = resourceLoader;
            _pinInputPageViewModel = pinInputPageViewModel;

            //Liga os comandos relacionados as transações-
            //SelecionarLancamento = new RelayCommand<object>(SelecionarLancamentoExecute);
            //CriarLancamento = new RelayCommand<Lancamento>(CriarLancamentoExecute);
            //RemoverLancamento = new RelayCommand<object>(RemoverLancamentoExecute);
            //AlterarLancamento = new RelayCommand<object>(AlterarLancamentoExecute);
            Configurar = new RelayCommand<object>(ConfigurarExecute);
            PageLoadCommand = new RelayCommand(OnPageLoad);
            PageUnloadCommand = new RelayCommand(OnPageUnload);
            Compartilhar = new RelayCommand(CompartilharExecute);

            if (App.LoginConfigurado)
                Messenger.Default.Register<PinInputEvent>(this, ValidarSenha);

            //_eventAggregator.GetEvent<DinDinPro.Universal.Aggregators.PinInputEvent>().Subscribe(ValidarSenha);            
        }

        private void CompartilharExecute()
        {
            DataTransferManager.ShowShareUI();
        }

        private void OnPageUnload()
        {
            DataTransferManager.GetForCurrentView().DataRequested -= MainPageViewModel_DataRequested;
        }

        private void MainPageViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            args.Request.Data.Properties.Title = _resourceLoader.GetString("MainPageShareTitle");
            args.Request.Data.Properties.Description = _resourceLoader.GetString("MainPageShareDescription");
            args.Request.Data.SetText(_resourceLoader.GetString("MainPageShareDescription"));
            args.Request.Data.Properties.ApplicationName = _resourceLoader.GetString("ApplicationTitle");
            args.Request.Data.SetApplicationLink(new Uri("https://www.microsoft.com/store/apps/9nblggh0drgm", UriKind.Absolute));
            args.Request.Data.SetWebLink(new Uri("https://www.microsoft.com/store/apps/9nblggh0drgm", UriKind.Absolute));
            //Windows.ApplicationModel.Package.Current.Id.ProductId


            deferral.Complete();
        }

        private async void OnPageLoad()
        {
            DataTransferManager.GetForCurrentView().DataRequested += MainPageViewModel_DataRequested;

            //_Contas = await _contaRepository.ListarContasAsync();
            await CarregarSumario();
            //await CarregarLancamentos();
            DadosGraficoDespesa = await _contaRepository.ListarDespesasAgrupadas(DateTime.Now);

            await VerificarExecucoes();
        }

        private async Task VerificarExecucoes()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constantes.CONFIG_KEY_EXECUCOES))
            {
                var qtde = 1;
                if (int.TryParse(ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES].ToString(), out qtde))
                {                    
                    if (qtde == 3)
                    {
                        var confirmou = false;
                        var commands = new List<DialogCommand>();

                        //Confirmou, manda pra tela de avaliação
                        commands.Add(new DialogCommand()
                        {
                            Label = _resourceLoader.GetString("Yes"),
                            Invoked = async () =>
                            {
                                confirmou = true;
                                var uri = new Uri(string.Format("ms-windows-store://review/?PFN={0}", Package.Current.Id.FamilyName));
                                await Windows.System.Launcher.LaunchUriAsync(uri);
                            }
                        });

                        //Não confirmou, pergunta se deseja enviar feedback por  e-mail
                        commands.Add(new DialogCommand()
                        {
                            Label = _resourceLoader.GetString("No"),
                            Invoked =  () =>
                            {
                                confirmou = false;                                
                            }
                        });

                        await _alertMessageService.ShowAsync(_resourceLoader.GetString("MsgPerguntaAvaliacao"), _resourceLoader.GetString("ApplicationTitle"), commands);                        

                        //Se recusou a votar, pergunta se quer enviar um feedback
                        if (!confirmou)
                        {
                            var feedbackCommands = new List<DialogCommand>();
                            feedbackCommands.Add(new DialogCommand()
                            {
                                Invoked = async () =>
                                {
                                    EmailMessage message = new EmailMessage();
                                    message.Subject = _resourceLoader.GetString("SobreEmailAssunto");
                                    message.Body = _resourceLoader.GetString("SobreEmailBody");
                                    message.To.Add(new EmailRecipient(_resourceLoader.GetString("SobreEmail")));
                                    await EmailManager.ShowComposeNewEmailAsync(message);
                                },
                                Label = _resourceLoader.GetString("Yes")
                            });
                            feedbackCommands.Add(new DialogCommand()
                            {
                                Invoked = () =>
                                {
                                    //Nada faz
                                },
                                Label = _resourceLoader.GetString("No")
                            });
                            //Exibe a tela de votação
                            await _alertMessageService.ShowAsync(_resourceLoader.GetString("MsgPerguntaFeedback"), _resourceLoader.GetString("ApplicationTitle"), feedbackCommands);

                            ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES] = 0   ;
                        }
                        else
                        {
                            //Se o usuário avaliou o aplicativo, reseta a configuração para não exibir mais a mensagem para ele
                            ApplicationData.Current.LocalSettings.Values[Constantes.CONFIG_KEY_EXECUCOES] = -1; 
                        }                        
                    }
                }
            }
        }

        private async void ValidarSenha(PinInputEvent obj)
        {
            //await Task.Delay(1000);
            //var configRep = _container.Resolve<IConfiguracaoRepository>();
            //var alertService = _container.Resolve<IAlertMessageService>();
            //var resource = _container.Resolve<IResourceLoader>();

            if (App.PIN.Equals(obj.Valor.ToString()))
            {
                App.AppLogado = true;
                LoginMode = false;
                RaisePropertyChanged(() => LoginMode);
            }
            else
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("MsgSenhaIncorreta"), _resourceLoader.GetString("ApplicationTitle"));
            }
        }

        private void ConfigurarExecute(object arg)
        {
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ConfiguracaoPage), null);
            //_navigationService.NavigateTo(Constantes.PAGINA_CONFIGURACAO, null);
        }


        #region Propriedades Binding
        public RelayCommand Compartilhar
        {
            get;
            private set;
        }
        public RelayCommand<object> Configurar
        {
            get;
            private set;
        }

        public RelayCommand<object> CriarConta
        {
            get;
            private set;
        }

        public RelayCommand PageLoadCommand
        {
            get;
            private set;
        }

        public RelayCommand PageUnloadCommand
        {
            get;
            private set;
        }

        #endregion

        #region Propriedades ViewModel

        public string NomeConta
        {
            get;
            set;
        }


        private double _Saldo = 0;

        public double Saldo
        {
            get { return _Saldo; }
            set
            {
                Set(() => Saldo, ref _Saldo, value);
            }
        }


        private double _Despesas = 0;
        public double Despesas
        {
            get { return _Despesas; }
            set
            {
                Set(() => Despesas, ref _Despesas, value);
            }
        }



        private bool _LoginMode;

        public bool LoginMode
        {
            get { return _LoginMode; }
            set
            {
                Set(() => LoginMode, ref _LoginMode, value);
            }
        }

        public IPinInputPageViewModel PinInputViewModel
        {
            get { return _pinInputPageViewModel; }
        }

        private ObservableCollection<GraficoData> _DadosGrafico;
        public ObservableCollection<GraficoData> DadosGrafico
        {
            get { return _DadosGrafico; }
            set { _DadosGrafico = value; }
        }

        private IReadOnlyList<GraficoDespesa> _DadosGraficoDespesa;
        public IReadOnlyList<GraficoDespesa> DadosGraficoDespesa
        {
            get { return _DadosGraficoDespesa; }
            set
            {
                Set(() => DadosGraficoDespesa, ref _DadosGraficoDespesa, value);
            }
        }


        #endregion



        #region OnNavigate
        //public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        //{
        //    _Contas = await _contaRepository.ListarContasAsync();

        //    await CarregarSumario();

        //    //await CarregarLancamentos();
        //}

        private async Task CarregarSumario()
        {
            Saldo = await _contaRepository.ObterSaldoAtual();
            Despesas = await _contaRepository.ObterDespesasFuturas();

        }
        #endregion
    }

    public class GraficoData
    {
        public double Receita { get; set; }
        public double Despesa { get; set; }
        public DateTime Data { get; set; }
    }

}
