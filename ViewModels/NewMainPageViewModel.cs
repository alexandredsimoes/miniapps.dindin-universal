using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DinDinPro.Universal.ViewModels
{
    public partial class NewMainPageViewModel : BaseViewModel
    {
        private bool _loginMode = false;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        private readonly IAlertMessageService _alertMessageService;
        private readonly ResourceLoader _resourceLoader;

        public NewMainPageViewModel(IDataService dataService, INavigationService navigationService, IContaRepository contaRepository,
            IAlertMessageService alertMessageService, ResourceLoader resourceLoader)
        {
            LoginConfigurado = App.LoginConfigurado;
            

            _dataService = dataService;
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _alertMessageService = alertMessageService;
            _resourceLoader = resourceLoader;

            //Liga os commandos            
            CriarConta = new RelayCommand<object>(CriarContaAsync);
            SelecionarConta = new RelayCommand<object>(SelecionarContaAsync);
            

            //Liga os comandos relacionados as transações-           
            Configurar = new RelayCommand<object>(ConfigurarExecute);

        }

        private void ConfigurarExecute(object arg)
        {
            //_navigationService.NavigateTo("Configuracao", null);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ConfiguracaoPage), null);
        }



        private void SelecionarContaAsync(object arg)
        {
            var obj = arg as ContaView;

            if (obj != null)
            {
                //var conta = await _contaRepository.ObterDetalhes(obj.ContaId);
                //_navigationService.NavigateTo("ContaDetalhe", obj.ContaId);
                ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ContaDetalhePage), obj.ContaId);
            }
        }

        private void CriarContaAsync(object sender)
        {
            //_navigationService.NavigateTo("Conta", null);            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ContaDetalhePage), null);
        }

        #region Propriedades Binding
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
        public RelayCommand<object> SelecionarConta
        {
            get;
            private set;
        }
 
        #endregion

        #region Propriedades ViewModel
        public bool LoginConfigurado { get; set; }

        public string NomeConta
        {
            get;
            set;
        }

        private IReadOnlyList<ContaView> _Contas;

        public IReadOnlyList<ContaView> Contas
        {
            get
            {
                return _Contas;
            }
            private set
            {

            }
        }

        private decimal _Saldo = 0;

        public decimal Saldo
        {
            get { return _Saldo; }
            set
            {
                Set(() => Saldo, ref _Saldo, value);
            }
        }


        private decimal _Despesas = 0;
        public decimal Despesas
        {
            get { return _Despesas; }
            set
            {
                Set(() => Despesas, ref _Despesas, value);
            }
        }


        #endregion



        #region OnNavigate
        //public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        //{
        //    _loginMode = navigationParameter != null;
        //    //if (_loginMode)
        //    //{
        //    //    _navigationService.Navigate("PinInput", null);
        //    //    return;
        //    //}

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
}
