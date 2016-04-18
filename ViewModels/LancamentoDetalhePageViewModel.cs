using DinDinPro.Universal.Logger;
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
    public class LancamentoDetalhePageViewModel : BaseViewModel
    {
        private readonly IContaRepository _contaRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public LancamentoDetalhePageViewModel(INavigationService navigationService, IContaRepository contaRepository,
            ResourceLoader resourceLoader, IAlertMessageService alertMessageService)
        {
            _contaRepository = contaRepository;
            _navigationService = navigationService;
            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            DuplicarLancamento = new RelayCommand(DuplicarLancamentoExecute);
            RemoverLancamento = new RelayCommand(RemoverLancamentoExecute);
            SalvarLancamento = new RelayCommand(SalvarLancamentoExecute);
            AlterarLancamento = new RelayCommand(AlterarLancamentoExecute);
            PageLoad = new RelayCommand(PageLoadExecute);
        }

        private async void PageLoadExecute()
        {

            if (Parametro != null)
            {
                var args = Parametro.ToString().Split(new char[] { ';' });
                if (args.Length > 1)
                    Detalhes = await _contaRepository.ObterLancamento(int.Parse(args[1]));
            }
            else
            {

            }
        }
        private void DuplicarLancamentoExecute()
        {
            var param = String.Format("{0};{1};{2}", Detalhes.ContaId, Detalhes.LancamentoId, "duplicar");

            ((AppShell)Window.Current.Content).AppFrame.GoBack(new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), param, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
        }


        private void AlterarLancamentoExecute()
        {
            //_navigationService.NavigateTo("CriarLancamento", String.Format("{0};{1}", Detalhes.ContaId, Detalhes.LancamentoId));            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), String.Format("{0};{1}", Detalhes.ContaId, Detalhes.LancamentoId));
        }

        private void SalvarLancamentoExecute()
        {
            return;// Task.FromResult<object>(null);
        }

        private async void RemoverLancamentoExecute()
        {
            var id = this.Detalhes.LancamentoId;
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("ContaDetalhePageMsgRemoverItemSim"),
                Invoked = async () =>
                {
                    if (await _contaRepository.RemoverLancamentoAsync(id))
                    {
                        ((AppShell)Window.Current.Content).AppFrame.GoBack();
                    }
                }
            });
            commands.Add(new DialogCommand()
            {
                Id = 2,
                Label = _resourceLoader.GetString("ContaDetalhePageMsgRemoverItemNao"),
                Invoked = () => { } //Nada faz
            });
            await _alertMessageService.ShowAsync(_resourceLoader.GetString("ContaDetalhePageMsgRemoverItem"),
                _resourceLoader.GetString("ApplicationTitle"), commands);

        }


        #region Propriedades

        #region Commands
        public RelayCommand DuplicarLancamento { get; private set; }
        public RelayCommand PageLoad
        {
            get; set;
        }
        public RelayCommand RemoverLancamento
        {
            get;
            private set;
        }

        public RelayCommand SalvarLancamento
        {
            get;
            private set;
        }

        public RelayCommand AlterarLancamento { get; private set; }
        #endregion Command

        #region ViewModel
        private LancamentoView _Detalhes;
        public LancamentoView Detalhes
        {
            get
            {
                return _Detalhes;
            }
            set
            {
                Set(() => Detalhes, ref _Detalhes, value);
            }
        }
        #endregion ViewModel

        #endregion
    }
}
