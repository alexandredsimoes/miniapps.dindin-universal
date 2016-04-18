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

namespace DinDinPro.Universal.ViewModels
{
    public class FormaPagamentoPageViewModel : BaseViewModel
    {
        private readonly IFormaPagamentoRepository _formaPagamentoRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public FormaPagamentoPageViewModel(INavigationService navigationService, ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            IFormaPagamentoRepository formaPagamentoRepository)
        {
            _navigationService = navigationService;
            _formaPagamentoRepository = formaPagamentoRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            PageLoad = new RelayCommand(PageLoadExecute);
            CriarFormaPagamento = new RelayCommand(CriarFormaPagamentoExecute, () => { return !IsBusy; });            
            SelecionarFormaPagamento = new RelayCommand<object>(SelecionarFormaPagamentoExecute, (o) => { return !IsBusy; });

        }


        #region Commands

        #region Métodos

        private async void PageLoadExecute()
        {
            Lista = new ObservableCollection<FormaPagamento>(await _formaPagamentoRepository.ListarFormas());
        }

        private void SelecionarFormaPagamentoExecute(object arg)
        {
            //_navigationService.NavigateTo("FormaPagamentoCriar", arg);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.FormaPagamentoCriarPage), arg);
        }

        private void CriarFormaPagamentoExecute()
        {
            //_navigationService.NavigateTo("FormaPagamentoCriar", null);            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.FormaPagamentoCriarPage), null);
        }

        #endregion


        #region Propriedades
        public RelayCommand PageLoad { get; private set; }        
        public RelayCommand CriarFormaPagamento { get; private set; }
        public RelayCommand<object> SelecionarFormaPagamento { get; private set; }
        #endregion

        #endregion Commands

        #region Propriedades


        private ObservableCollection<FormaPagamento> _Lista = new ObservableCollection<FormaPagamento>();

        public ObservableCollection<FormaPagamento> Lista
        {
            get { return _Lista; }
            set
            {
                Set(() => Lista, ref _Lista, value);
            }
        }
        #endregion
    }
}
