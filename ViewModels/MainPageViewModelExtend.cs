using DinDinPro.Universal.Models;
using DinDinPro.Universal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public partial class MainPageViewModel
    {
        private void AlterarLancamentoExecute(object arg)
        {
            //_navigationService.NavigateTo("CriarLancamento", arg as Lancamento);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), arg as Lancamento);
        }

        private async void RemoverLancamentoExecute(object arg)
        {
            var id = (int)arg;
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("ContaDetalhePageMsgRemoverItemSim"),
                Invoked = async () =>
                {
                    if (await _contaRepository.RemoverLancamentoAsync(id))
                    {
                        Lancamentos.Remove(Lancamentos.FirstOrDefault(c => c.LancamentoId == id));
                        await CarregarSumario();

                        //Rever a necessidade de colocarr isso no CarregarSumario
                        _Contas = new ObservableCollection<ContaView>(await _contaRepository.ListarContasAsync());
                        RaisePropertyChanged(() => Contas);
                        RaisePropertyChanged(() => ExisteContas);
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

        private async void CriarLancamentoExecute(Lancamento arg)
        {
            if(Contas == null  || Contas.Count == 0)
            {
                var msg = _resourceLoader.GetString("MainPageMsgNoAccount");
                await _alertMessageService.ShowAsync(msg, _resourceLoader.GetString("ApplicationTitle"));
                return;
            }
            //_navigationService.NavigateTo("CriarLancamento", arg);            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), arg);
        }

        private void SelecionarLancamentoExecute(object arg)
        {
            var lancamento = arg as Lancamento;

            if (lancamento != null)
                //_navigationService.NavigateTo("LancamentoDetalhe", arg);
                ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.LancamentoDetalhePage), arg);

        }

        public async Task CarregarLancamentos()
        {
            var lancamentos = await _contaRepository.ObterLancamentos(_periodo);
            Lancamentos = new ObservableCollection<LancamentoView>(lancamentos);
            RaisePropertyChanged(() => Lancamentos);
        }

        public ObservableCollection<LancamentoView> Lancamentos
        {
            get;
            private set;

        }


        private DateTime _periodo;

        public DateTime Periodo
        {
            get
            {
                return _periodo;
            }
            set
            {
                Set(() => Periodo, ref _periodo, value);
            }
        }
            


        #region Commmands
        public RelayCommand<object> AlterarLancamento
        {
            get;
            private set;
        }
        public RelayCommand<object> RemoverLancamento
        {
            get;
            private set;
        }
        public RelayCommand<object> SelecionarLancamento
        {
            get;
            private set;
        }
        public RelayCommand<Lancamento> CriarLancamento
        {
            get;
            private set;
        }
        #endregion Commands
    }
}
