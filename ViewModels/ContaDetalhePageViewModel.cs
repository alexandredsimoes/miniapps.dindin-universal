using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.Linq.Expressions;
using DinDinPro.Universal.Logger;
using System.Globalization;
using GalaSoft.MvvmLight.Views;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight.Command;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class ContaDetalhePageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public ContaDetalhePageViewModel(INavigationService navigationService, IContaRepository contaRepository, ResourceLoader resourceLoader,
            IAlertMessageService alertMessageService)
        {
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            SelecionarLancamento = new RelayCommand<object>(SelecionarLancamentoExecute);
            CriarLancamento = new RelayCommand(CriarLancamentoExecute);            
            AlterarLancamento = new RelayCommand<object>(AlterarLancamentoExecute);            
            Ordernar = new RelayCommand<object>(OrdernarExecute);
            PageLoadCommand = new RelayCommand(PageLoadExecute);
            RemoverConta = new RelayCommand(RemoverContaExecuteAsync);

            MudarPeriodo = new RelayCommand<string>(MudarPeriodoExecute, (s) => { return !IsBusy; });


            Periodo = DateTime.Now;


            Func<string, List<ListaSelecao>> carregarListasAuxiliares = (resourceString) =>
            {
                var items = _resourceLoader.GetString(resourceString).Split(',');
                var lista = new List<ListaSelecao>();
                for (int i = 0; i < items.Length; i++)
                {
                    lista.Add(new ListaSelecao() { Indice = i, Descricao = items[i] });
                }
                return lista;
            };



            if (_TiposOrdenacao == null)
            {
                _TiposOrdenacao = carregarListasAuxiliares("ContaDetalhePageTipoOrdenacao");
            }

            TipoOrdenacao = _TiposOrdenacao[0];

        }



        //public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        //{


        //    _contaAtual = (int)navigationParameter;

        //    if (navigationMode == Windows.UI.Xaml.Navigation.NavigationMode.Back)
        //    {
        //        if (viewModelState.ContainsKey("PERIODO"))
        //            Periodo = DateTime.ParseExact(viewModelState["PERIODO"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

        //        if (viewModelState.ContainsKey("ORDENACAO"))
        //        {
        //            var indice = (int)viewModelState["ORDENACAO"];
        //            TipoOrdenacao = TiposOrdenacao.First(c => c.Indice == indice);
        //        }
        //    }

        //    //Carrega os lançamentos 
        //    await CarregarLancamentos();
        //}

        //public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        //{
        //    //Salva o periodo atual
        //    if (!viewModelState.ContainsKey("PERIODO"))
        //        viewModelState.Add("PERIODO", Periodo.ToString("dd/MM/yyyy"));
        //    else
        //        viewModelState["PERIODO"] = Periodo.ToString("dd/MM/yyyy");

        //    if (!viewModelState.ContainsKey("ORDENACAO"))
        //        viewModelState.Add("ORDENACAO", TipoOrdenacao.Indice);
        //    else
        //        viewModelState["ORDENACAO"] = TipoOrdenacao.Indice;
        //}

        public async Task CarregarLancamentos()
        {
            if (IsBusy) return;
            IsBusy = true;
            var ordenacao = TipoOrdenacaoLancamento.toDataDesc;

            if (_TipoOrdenacao.Indice == 1)
                ordenacao = TipoOrdenacaoLancamento.toDataAsc;
            else if (_TipoOrdenacao.Indice == 2)
                ordenacao = TipoOrdenacaoLancamento.toDescription;
            else if (_TipoOrdenacao.Indice == 3)
                ordenacao = TipoOrdenacaoLancamento.toAmmountDesc;
            else if (_TipoOrdenacao.Indice == 4)
                ordenacao = TipoOrdenacaoLancamento.toAmmountAsc;

            var lancamentos = await _contaRepository.ObterLancamentos(null, null, _periodo, ordenacao);
            Lancamentos = new ObservableCollection<LancamentoView>(lancamentos);


            CalcularSaldo();

            ExisteLancamentos = Lancamentos.Count > 0;

            this.RaisePropertyChanged(() => Lancamentos);
            this.RaisePropertyChanged(() => ExisteLancamentos);
            IsBusy = false;
        }

        private async void CalcularSaldo()
        {
            SaldoPrevisto = await _contaRepository.ObterSaldoMesAtualPrevisto();// (saldoReceita - saldoDespesa);

            Saldo = await _contaRepository.ObterSaldoMesAtual();
        }

        #region Métodos
        private async void RemoverContaExecuteAsync()
        {
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("MainPageRemoverContaButtonSim"),
                Invoked = async () =>
                {
                    await _contaRepository.RemoverContaAsync(_contaAtual);
                    ((AppShell)Window.Current.Content).AppFrame.GoBack();                                                                                
                }
            });
            commands.Add(new DialogCommand()
            {
                Id = 2,
                Label = _resourceLoader.GetString("MainPageRemoverContaButtonNao"),
                Invoked = () => { } //Nada faz
            });
            await _alertMessageService.ShowAsync(_resourceLoader.GetString("MainPageRemoverContaMsg"),
                _resourceLoader.GetString("ApplicationTitle"), commands);

        }

        private async void MudarPeriodoExecute(string tipo)
        {
            Periodo = Periodo.AddMonths( tipo == "+" ? 1 : -1);
            await CarregarLancamentos();
        }

        private async void PageLoadExecute()
        {
            _contaAtual = (int)Parametro;
            await CarregarLancamentos();           
        }


        private async void OrdernarExecute(object arg)
        {
            await CarregarLancamentos();
        }

        private void AlterarLancamentoExecute(object arg)
        {
            //_navigationService.NavigateTo("CriarLancamento", String.Format("{0};{1}", _contaAtual, arg));            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), String.Format("{0};{1}", _contaAtual, arg));
        }
       

        private void CriarLancamentoExecute()
        {
            //_navigationService.NavigateTo(Constantes.PAGINA_LANCAMENTO, _contaAtual);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.CriarLancamentoPage), null);
        }

        private void SelecionarLancamentoExecute(object arg)
        {
            var lancamento = arg as Lancamento;

            if (lancamento != null)
            {
                //_navigationService.NavigateTo(Constantes.PAGINA_LANCAMENTO_DETALHE, String.Format("{0};{1}", _contaAtual, lancamento.LancamentoId));
                ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.LancamentoDetalhePage), String.Format("{0};{1}", _contaAtual, lancamento.LancamentoId));
            }
        }
        #endregion

        #region Propriedades

        #region ViewModel


        private double _Saldo;
        public double Saldo
        {
            get { return _Saldo; }
            set
            {               
                Set(() => Saldo, ref _Saldo, value);
            }
        }

        private double _SaldoPrevisto;
        public double SaldoPrevisto
        {
            get { return _SaldoPrevisto; }
            set
            {
                Set(() => SaldoPrevisto, ref _SaldoPrevisto, value);
            }
        }

        private int _contaAtual;
        public int ContaAtual
        {
            get
            {
                return _contaAtual;
            }
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

        public bool ExisteLancamentos { get; private set; }

        private IReadOnlyList<ListaSelecao> _TiposOrdenacao;
        public IReadOnlyList<ListaSelecao> TiposOrdenacao
        {
            get
            {
                return _TiposOrdenacao;
            }
        }

        private ListaSelecao _TipoOrdenacao;
        public ListaSelecao TipoOrdenacao
        {
            get
            {
                return _TipoOrdenacao;
            }
            set
            {
                _TipoOrdenacao = value;
            }
        }

        #endregion ViewModel

        #region Commmands
        public RelayCommand<object> AlterarLancamento
        {
            get;
            private set;
        }

        public RelayCommand<object> SelecionarLancamento
        {
            get;
            private set;
        }
        public RelayCommand CriarLancamento
        {
            get;
            private set;
        }
        public RelayCommand RemoverConta
        {
            get;
            private set;
        }

        public RelayCommand<object> Ordernar { get; private set; }        
        public RelayCommand PageLoadCommand { get; private set; }
        public RelayCommand<string> MudarPeriodo { get; private set; }
        #endregion Commands

        #endregion
    }
}
