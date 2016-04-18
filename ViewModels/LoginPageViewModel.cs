using DinDinPro.Universal.Aggregators;
using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DinDinPro.Universal.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IContaRepository _contaRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly INavigationService _navigationService;

        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;        
        private readonly IPinInputPageViewModel _pinInputPageViewModel;

        public LoginPageViewModel(INavigationService navigationService,
            ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            IContaRepository contaRepository,
            IPinInputPageViewModel pinInputPageViewModel,
            IConfiguracaoRepository configuracaoRepository)
        {

            _navigationService = navigationService;

            _contaRepository = contaRepository;
            _pinInputPageViewModel = pinInputPageViewModel;
            _configuracaoRepository = configuracaoRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            //RestaurarBackup = DelegateCommand.FromAsyncHandler(RestaurarBackupExecute, () => { return !IsBusy; });
            //EfetuarBackup = DelegateCommand.FromAsyncHandler(EfetuarBackupExecute, () => { return !IsBusy; });
            //DefinirSenha = DelegateCommand.FromAsyncHandler(DefinirSenhaExecute, () => { return !IsBusy; });
            //AlterarTags = DelegateCommand.FromAsyncHandler(AlterarTagsExecute, () => { return !IsBusy; });
            //AlterarFormaPagamento = DelegateCommand.FromAsyncHandler(AlterarFormaPagamentoExecute, () => { return !IsBusy; });
            
            Messenger.Default.Register<PinInputEvent>(this, ObterSenha);
            //_eventAggregator.GetEvent<DecimalInputEvent>().Subscribe(ObterSenha);
        }

        private async void ObterSenha(PinInputEvent obj)
        {            
            //Verifica as credenciais
            var senha = await _configuracaoRepository.ObterValor("senha").ConfigureAwait(false);

            if (senha.Equals(obj.Valor.ToString()))
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.MainPage), null);
                    ((AppShell)Window.Current.Content).CarregarMenus();
                });               
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async() =>
                {
                    await _alertMessageService.ShowAsync("Senha incorreta, tente novamente.", _resourceLoader.GetString("ApplicationTitle"));
                });                
            }
            //IsBusy = false;
        }

        public IPinInputPageViewModel PinInputPageViewModel
        {
            get
            {
                return _pinInputPageViewModel;
            }
        }
    }
}
