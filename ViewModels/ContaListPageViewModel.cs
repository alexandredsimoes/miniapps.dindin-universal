using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class ContaListPageViewModel : BaseViewModel
    {

        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        private readonly IAlertMessageService _alertMessageService;
        private readonly ResourceLoader _resourceLoader;

        public ContaListPageViewModel(INavigationService navigationService, IContaRepository contaRepository,
            IAlertMessageService alertMessageService, ResourceLoader resourceLoader)
        {
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _alertMessageService = alertMessageService;
            _resourceLoader = resourceLoader;

            PageLoad = new RelayCommand(PageLoadExecute);
            CriarConta = new RelayCommand(CriarContaExecute);
            SelecionarConta = new RelayCommand<object>(SelecionarContaAsync);
        }

        #region Métodos

        #region Commands
        private async void PageLoadExecute()
        {
            Contas = new ObservableCollection<ContaView>(await _contaRepository.ListarContasAsync());
            RaisePropertyChanged(() => ExisteContas);
        }
        private void CriarContaExecute()
        {
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ContaPage), null);
        }

        private void SelecionarContaAsync(object arg)
        {
            var obj = arg as ContaView;

            if (obj != null)
            {
                ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.ContaDetalhePage), obj.ContaId);
            }
        }
        #endregion

        #endregion


        #region Propriedades

        #region Commands
        public RelayCommand<object> SelecionarConta
        {
            get;
            private set;
        }
        public RelayCommand CriarConta { get; private set; }
        public RelayCommand PageLoad { get; private set; }
        #endregion

        #region ViewModel
        private ObservableCollection<ContaView> _Contas;
        public ObservableCollection<ContaView> Contas
        {
            get
            {
                return _Contas;
            }
            set
            {
                Set(() => Contas, ref _Contas, value);
            }
        }
        public bool ExisteContas
        {
            get
            {
                return Contas == null ? false : Contas.Count > 0;
            }
        }

        #endregion

        #endregion


    }
}
