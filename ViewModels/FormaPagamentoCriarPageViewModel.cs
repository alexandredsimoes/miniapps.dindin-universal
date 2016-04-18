using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class FormaPagamentoCriarPageViewModel : BaseViewModel
    {
        private readonly IFormaPagamentoRepository _formaPagamentoRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public FormaPagamentoCriarPageViewModel(INavigationService navigationService, ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            IFormaPagamentoRepository formaPagamentoRepository)
        {
            _navigationService = navigationService;
            _formaPagamentoRepository = formaPagamentoRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            PageLoad = new RelayCommand(PageLoadExecute);
            SalvarFormaPagamento = new RelayCommand<object>(SalvarFormaPagamentoExecute, (o) => { return !IsBusy; });
            FormaPagamentoSelecionada = new FormaPagamento() { DataCriacao = DateTime.Now };
            RemoverFormaPagamento = new RelayCommand(RemoverFormaPagamentoExecute, () => { return !IsBusy; });
        }

        private void PageLoadExecute()
        {
            if (Parametro != null)
                FormaPagamentoSelecionada = Parametro as FormaPagamento;
            else
                FormaPagamentoSelecionada = new FormaPagamento() { DataCriacao = DateTime.Now };
        }

        #region Commands

        #region Métodos

        private async void RemoverFormaPagamentoExecute()
        {
            if (FormaPagamentoSelecionada == null) return;

            if (!await _formaPagamentoRepository.ExisteRelacionamento(FormaPagamentoSelecionada.FormaPagamentoId))
            {
                if (await _formaPagamentoRepository.RemoverFormaPagamento(FormaPagamentoSelecionada))
                {
                    ((AppShell)Window.Current.Content).AppFrame.GoBack();  
                }
            }
            else
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("FormaPagamentoPageMsgExisteRelacionamento"),
                    _resourceLoader.GetString("ApplicationTitle"));
            }
        }
        private async void SalvarFormaPagamentoExecute(object obj)
        {
            if (String.IsNullOrWhiteSpace(FormaPagamentoSelecionada.Nome))
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("FormaPagamentoPageFlyoutMsgErroNome"),
                    _resourceLoader.GetString("ApplicationTitle"));
                return;
            }

            await _formaPagamentoRepository.SalvarFormaPagamento(FormaPagamentoSelecionada);

            //_navigationService.GoBack();
            ((AppShell)Window.Current.Content).AppFrame.GoBack();
        }
        #endregion Métodos

        #region Propriedades
        public RelayCommand RemoverFormaPagamento { get; private set; }
        public RelayCommand PageLoad { get; private set; }
        public RelayCommand<object> SalvarFormaPagamento { get; private set; }
        #endregion Propriedades

        #endregion Commands

        #region ViewModel


        private FormaPagamento _FormaPagamentoSelecionada;

        public FormaPagamento FormaPagamentoSelecionada
        {
            get { return _FormaPagamentoSelecionada; }
            set
            {
                Set(() => FormaPagamentoSelecionada, ref _FormaPagamentoSelecionada, value);
            }
        }


        #endregion ViewModel
    }
}
