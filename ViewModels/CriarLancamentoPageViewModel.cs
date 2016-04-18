using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml.Media;
using DinDinPro.Universal.Services;
using Windows.UI.ViewManagement;
using Windows.UI.StartScreen;
using DinDinPro.Universal.Logger;
using GalaSoft.MvvmLight.Views;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using DinDinPro.Universal.Aggregators;
using GalaSoft.MvvmLight.Threading;

namespace DinDinPro.Universal.ViewModels
{
    public class CriarLancamentoPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        //private readonly IEventAggregator _eventAggregator;
        private readonly IDecimalInputUserControlViewModel _decimalInputUserControlViewModel;
        private readonly IFormaPagamentoRepository _formaPagamentoRepository;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _messageService;


        public CriarLancamentoPageViewModel(INavigationService navigationService, IContaRepository contaRepository, IFormaPagamentoRepository formaPagamentoRepository,
            IDecimalInputUserControlViewModel decimalInputUserControlViewModel, ResourceLoader resourceLoader,
            IAlertMessageService messageService)
        {
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _formaPagamentoRepository = formaPagamentoRepository;
            _decimalInputUserControlViewModel = decimalInputUserControlViewModel;
            _resourceLoader = resourceLoader;
            _messageService = messageService;

            FormaDePagamento = new ListaSelecao() { Indice = 0, Descricao = _resourceLoader.GetString("CriarLancamentoPageSelecaoVazia") };
            ContaSelecionada = new ListaSelecao() { Indice = 0, Descricao = _resourceLoader.GetString("CriarLancamentoPageSelecaoVazia") };

            FimRepeticaoData = DateTime.Now;
            FimRepeticaoQuantidade = 1;
            // OnPropertyChanged("Detalhes");
            PeriodoSelecionado = "H"; //Hoje

            AlterarValor = new RelayCommand<object>(AlterarValorExecute);
            AlterarValorRealizado = new RelayCommand<object>(AlterarValorRealizadoExecute);
            SelecionarTag = new RelayCommand<object>(SelecionarTagExecute);
            SalvarLancamento = new RelayCommand(SalvarLancamentoExecute);
            AlterarTipo = new RelayCommand<object>(AlterarTipoExecute);
            FixarLancamento = new RelayCommand(FixarLancamentoExecute);


            this.PropertyChanged += (sender, e) => {
                if(e.PropertyName == "Tipo")
                {
                    AlterarTipoExecute(Tipo?.Tag);
                }
            };
            //PageLoad
            PageLoadCommand = new RelayCommand(PageLoadExecute);            
        }

        private async void PageLoadExecute()
        {
            await CarregarDadosAuxiliares();


            //Verifica se trata de uma edição do lançamento
            if (Parametro != null)
            {
                var args = Parametro.ToString().Split(new char[] { ';' });
                if (args.Length > 1 && args[0] != "add-lancamento") //Selecionamos um lançamento
                {
                    var lancamentoId = int.Parse(args[1]);
                    Detalhes = await _contaRepository.ObterLancamento(lancamentoId);
                    Fechado = Detalhes.Fechado;
                    Tipo = Detalhes.Tipo == "+" ? Tipos[0] : Tipos[1];
                    var contaDetalhe = await _contaRepository.ObterDetalhes(Detalhes.ContaId);
                    if(contaDetalhe != null)
                        ContaSelecionada = new ListaSelecao() { Descricao = contaDetalhe.NomeConta, Indice = contaDetalhe.ContaId };

                    //Carrega as tags
                    var tags = await _contaRepository.ListarTagsAsync(_Detalhes.Tipo);

                    for (int i = 0; i < tags.Count; i++)
                    {
                        if (Detalhes.Tags.ToLower().Contains(tags[i].NomeTag))
                            tags[i].Selecionado = true;
                    }
                    Tags = tags;


                    var formaDetalhe = await _formaPagamentoRepository.ObterDetalhes(Detalhes.FormaPagamentoId ?? 0);
                    if (formaDetalhe != null)
                        FormaDePagamento = new ListaSelecao() { Descricao = formaDetalhe.Nome, Indice = formaDetalhe.FormaPagamentoId };
                    else
                        FormaDePagamento = new ListaSelecao() { Descricao = _resourceLoader.GetString("CriarLancamentoPageSelecaoVazia"), Indice=0 };

                    //FormaDePagamento = await _formaPagamentoRepository.ObterDetalhes(Detalhes.FormaPagamentoId ?? 0);

                    ////Caso não tenha selecionado nenhuma forma de pagamento, exibe a padrão vazia
                    //if (FormaDePagamento == null)
                    //    FormaDePagamento = new FormaPagamento() { Nome = _resourceLoader.GetString("CriarLancamentoPageSelecaoVazia") };

                    if (Detalhes.DataLancamento.Value.Date == DateTime.Now.Date)
                        PeriodoSelecionado = "H";
                    else if (Detalhes.DataLancamento.Value.Date == DateTime.Now.AddDays(-1).Date)
                        PeriodoSelecionado = "O";
                    else
                        PeriodoSelecionado = "D";

                    Repeticao = ListaRepeticoes.FirstOrDefault(c => c.Indice == _Detalhes.TipoRepeticao);
                    RepeticaoFim = ListaRepeticoesFim.FirstOrDefault(c => c.Indice == Detalhes.TipoFimRepeticao);
                    FimRepeticaoQuantidade = _Detalhes.FimRepeticaoQuantidade;
                    FimRepeticaoData = _Detalhes.FimRepeticaoData;


                    //Verifica se existe o terceiro parametro "duplicar"
                    if (args.Length == 3 && args[2] == "duplicar")
                    {
                        //Reseta o id para ficar como inserção
                        Detalhes.LancamentoId = 0;
                    }
                    RaisePropertyChanged(() => Detalhes);
                    //RaisePropertyChanged(() => Tipo);
                    RaisePropertyChanged(() => PeriodoSelecionado);

                }
                else if (args[0] == "add-lancamento") //Estamos vindo da tile fixada
                {
                    Tipo = Tipos.First(c => c.Tag == args[2]);
                    var contaDetalhe = await _contaRepository.ObterDetalhes(int.Parse(args[1]));
                    if (contaDetalhe != null)
                    {
                        ContaSelecionada = new ListaSelecao() { Descricao = contaDetalhe.NomeConta, Indice = contaDetalhe.ContaId };
                    }
                    //ContaSelecionada = await _contaRepository.ObterDetalhes(int.Parse(args[1]));
                    Tags = await _contaRepository.ListarTagsAsync(Tipo.Indice == 0 ? "+" : "-");
                }
                else
                {
                    //Estamos vindo a tela inicial
                    //ContaSelecionada = await _contaRepository.ObterDetalhes(int.Parse(args[0]));
                    //Tags = await _contaRepository.ListarTagsAsync(Tipo.Indice == 0 ? "+" : "-");
                }
            }
            else
            {
                Detalhes = new LancamentoView()
                {
                    DataCriacao = DateTime.Now,
                    DataLancamento = DateTime.Now,
                    Descricao = "",
                    Tipo = "+",
                    ValorLancamento = 0,
                    ValorLancamentoRealizado = 0,
                    Fechado = false,
                    FimRepeticaoData = DateTime.Now,
                    FimRepeticaoQuantidade = 1,
                    Tags = String.Empty
                };
                Tipo = Tipos[0];
            }
        }

        private async Task CarregarDadosAuxiliares()
        {
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


            if (_Tipos == null)
            {
                _Tipos = carregarListasAuxiliares("CriarLancamentoPageListaTipos");
                _Tipos[0].Tag = "+";
                _Tipos[1].Tag = "-";
            }

            RaisePropertyChanged(() => Tipos);

            //Lista de repeticoes
            if (_ListaRepeticoes == null)
            {
                _ListaRepeticoes = carregarListasAuxiliares("CriarLancamentoPageListaRepeticoes");
                RaisePropertyChanged(() => ListaRepeticoes);
            }

            //Lista de repeticoes (fim)
            if (_ListaRepeticoesFim == null)
            {
                _ListaRepeticoesFim = carregarListasAuxiliares("CriarLancamentoPageListaRepeticoesFinal");
                RaisePropertyChanged(() => ListaRepeticoesFim);
            }

            //Define o tipo padrão
            //Tipo = Tipos[0];

            //Seta as repetiçoes padrão
            Repeticao = _ListaRepeticoes[0];
            RepeticaoFim = _ListaRepeticoesFim[0];

            //Carregas os dados auxiliares (formas de pagamento, tipos etc.)
            FormasDePagamento.Clear();
            foreach(var item in await _formaPagamentoRepository.ListarFormas())
                FormasDePagamento.Add(new ListaSelecao() { Descricao = item.Nome, Indice = item.FormaPagamentoId });

            Contas.Clear();
            foreach (var item in await _contaRepository.ListarContasAsync())
                Contas.Add(new ListaSelecao() { Descricao = item.NomeConta, Indice = item.ContaId });

            //Carrega as de entrada
            // Tags = await _contaRepository.ListarTagsAsync(Tipo.Indice == 0 ? "+" : "-");
        }


        private async Task<bool> Validar()
        {
            IList<string> erros = new List<string>();

            if (_ContaSelecionada == null || _ContaSelecionada.Indice <= 0)
                erros.Add(_resourceLoader.GetString("CriarLancamentoPageMsgSelectAccount"));


            //Verifica as tags selecionadas
            var tags = _Detalhes.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (tags != null && tags.Length < 1)
                erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroTags"));


            //Valor do lançamento
            if (_Detalhes.ValorLancamento <= 0)
                erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroValor"));

            //Se estiver marcado como realizado, verifica se o valor realizado foi preenchido
            if (_Detalhes.Fechado && _Detalhes.ValorLancamentoRealizado <= 0)
            {
                erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroValorRealizado"));
            }

            //Verifica se a repetição final esta preenchida
            if (Repeticao != null && Repeticao.Indice > 0)
            {
                if (RepeticaoFim == null || RepeticaoFim.Indice < 1)
                {
                    erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroSelecionarRepeticaoFim"));
                }
                else
                {
                    //Nunca,Data específica,Depois de x repetições
                    switch (RepeticaoFim.Indice)
                    {
                        case 1: //Data
                            if (Detalhes.FimRepeticaoData == null || Detalhes.FimRepeticaoData < DateTime.Now)
                                erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroFimRepeticaoData"));
                            break;
                        case 2: //Quantidade
                            if (Detalhes.FimRepeticaoQuantidade == null || Detalhes.FimRepeticaoQuantidade <= 0)
                                erros.Add(_resourceLoader.GetString("CriarLancamentoPageErroFimRepeticaoQuantidade"));
                            break;
                    }
                }
            }



            if (erros.Count > 0)
                await _messageService.ShowAsync(String.Join(Environment.NewLine, erros), _resourceLoader.GetString("ApplicationTitle"));

            return erros.Count == 0;
        }


        //public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode,
        //    Dictionary<string, object> viewModelState)
        //{

        //    await CarregarDadosAuxiliares();

        //    //Verifica se trata de uma edição do lançamento
        //    if (navigationParameter != null)
        //    {
        //        var args = navigationParameter.ToString().Split(new char[] { ';' });
        //        if (args.Length > 1 && args[0] != "add-lancamento") //Selecionamos um lançamento
        //        {
        //            var lancamentoId = int.Parse(args[1]);
        //            Detalhes = await _contaRepository.ObterLancamento(lancamentoId);
        //            Fechado = Detalhes.Fechado;
        //            Tipo = Detalhes.Tipo == "+" ? Tipos[0] : Tipos[1];
        //            ContaSelecionada = await _contaRepository.ObterDetalhes(Detalhes.ContaId);

        //            //Carrega as tags
        //            var tags = await _contaRepository.ListarTagsAsync(_Detalhes.Tipo);

        //            for (int i = 0; i < tags.Count; i++)
        //            {
        //                if (Detalhes.Tags.ToLower().Contains(tags[i].NomeTag))
        //                    tags[i].Selecionado = true;
        //            }
        //            Tags = tags;

        //            FormaDePagamento = await _formaPagamentoRepository.ObterDetalhes(Detalhes.FormaPagamentoId ?? 0);

        //            //Caso não tenha selecionado nenhuma forma de pagamento, exibe a padrão vazia
        //            if (FormaDePagamento == null)
        //                FormaDePagamento = new FormaPagamento() { Nome = _resourceLoader.GetString("CriarLancamentoPageSelecaoVazia") };

        //            if (Detalhes.DataLancamento.Value.Date == DateTime.Now.Date)
        //                PeriodoSelecionado = "H";
        //            else if (Detalhes.DataLancamento.Value.Date == DateTime.Now.AddDays(-1).Date)
        //                PeriodoSelecionado = "O";
        //            else
        //                PeriodoSelecionado = "D";

        //            Repeticao = ListaRepeticoes.FirstOrDefault(c => c.Indice == _Detalhes.TipoRepeticao);
        //            RepeticaoFim = ListaRepeticoesFim.FirstOrDefault(c => c.Indice == Detalhes.TipoFimRepeticao);
        //            FimRepeticaoQuantidade = _Detalhes.FimRepeticaoQuantidade;
        //            FimRepeticaoData = _Detalhes.FimRepeticaoData;


        //            //Verifica se existe o terceiro parametro "duplicar"
        //            if (args.Length == 3 && args[2] == "duplicar")
        //            {
        //                //Reseta o id para ficar como inserção
        //                Detalhes.LancamentoId = 0;
        //            }
        //            OnPropertyChanged("Detalhes");
        //            OnPropertyChanged("Tipo");
        //            OnPropertyChanged("PeriodoSelecionado");
        //        }
        //        else if (args[0] == "add-lancamento") //Estamos vindo da tile fixada
        //        {
        //            Tipo = Tipos.First(c => c.Tag == args[2]);
        //            ContaSelecionada = await _contaRepository.ObterDetalhes(int.Parse(args[1]));
        //            Tags = await _contaRepository.ListarTagsAsync(Tipo.Indice == 0 ? "+" : "-");
        //        }
        //        else
        //        {
        //            //Estamos vindo a tela inicial
        //            ContaSelecionada = await _contaRepository.ObterDetalhes(int.Parse(args[0]));
        //            Tags = await _contaRepository.ListarTagsAsync(Tipo.Indice == 0 ? "+" : "-");
        //        }
        //    }
        //    else
        //    {

        //    }


        //    // base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
        //}


        #region Métodos

        #region ViewModel

        private async void FixarLancamentoExecute()
        {

            try
            {
                if (Detalhes == null) return;

                //var tile = TileContentFactory.CreateTileSquare150x150Block();
                //tile.TextBlock.Text = _resourceLoader.GetString("ApplicationTitle");
                //tile.TextSubBlock.Text = Detalhes.Tipo == "+" ? _resourceLoader.GetString("CriarLancamentoPageTileTitleReceita") : _resourceLoader.GetString("CriarLancamentoPageTileTitleDepesa");
                //tile.Branding = TileBranding.Name;

                //var notification = tile.CreateNotification();


                var title = Detalhes.Tipo == "+" ? _resourceLoader.GetString("CriarLancamentoPageTileTitleReceita") : _resourceLoader.GetString("CriarLancamentoPageTileTitleDespesa");
                var name = Detalhes.Tipo == "+" ? "DINDIN_UNVERSAL_ADD_TILE_INCOME" : "DINDIN_UNVERSAL_ADD_TILE_EXPENSE";
                var uri = Detalhes.Tipo == "+" ? new Uri("ms-appx:///Assets/MoneyIn.png", UriKind.RelativeOrAbsolute) :
                    new Uri("ms-appx:///Assets/MoneyOut.png", UriKind.RelativeOrAbsolute);

                var tile2 = new SecondaryTile(
                    name,
                    title,
                    String.Format("{0};{1};{2}", "add-lancamento", _ContaSelecionada.Indice, Detalhes.Tipo),
                    uri,
                    TileSize.Square150x150);


                var isPinned = await tile2.RequestCreateAsync();
                await tile2.UpdateAsync();
                //var updater = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForSecondaryTile("dindinUniversal-add");
                //updater.Update(notification);



                //return Task.FromResult<object>(null);
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Essa função é responsável por alterar o Tipo (Receita/Despesa) e recarregar os dados referene a ele
        /// </summary>
        /// <returns></returns>
        private async void AlterarTipoExecute(object origem)
        {
            if (origem != null)
            {
                //Carrega as tags
                //Detalhes.Tags = String.Empty;
                //Detalhes.Tipo = origem.ToString().Equals("0") ? "+" : "-";
                Tags = await _contaRepository.ListarTagsAsync(origem.ToString());
                //Tipo = _Tipos.FirstOrDefault(c => c.Tag == (origem.ToString().Equals("0") ? "+" : "-"));
            }


            RaisePropertyChanged(() => Detalhes);
        }

        private void SalvarLancamentoExecute()
        {

            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    //Valida as informações
                    if (!await Validar())
                    {
                        IsBusy = false;
                        return;
                    }

                    IsBusy = true;

                    //Define o periodo selecionado pelo usuário
                    if (PeriodoSelecionado == "H")
                        _Detalhes.DataLancamento = DateTime.Now;
                    else if (PeriodoSelecionado == "O")
                        _Detalhes.DataLancamento = DateTime.Now.AddDays(-1);

                    _Detalhes.TipoRepeticao = Repeticao.Indice;
                    _Detalhes.TipoFimRepeticao = RepeticaoFim.Indice;
                    _Detalhes.FormaPagamentoId = FormaDePagamento?.Indice;
                    _Detalhes.ContaId = ContaSelecionada.Indice;
                    _Detalhes.Tipo = Tipo.Indice == 0 ? "+" : "-";
                    //_Detalhes.Fechado = Fechado;


                    var result = await _contaRepository.CriarLancamentoAsync(_Detalhes);

                    //Tivemos sucesso, processa o restante, tags, lançamentos replicados
                    if (result)
                    {
                        ///if (_navigationService.CanGoBack())
                        //_navigationService.GoBack();
                        ((AppShell)Window.Current.Content).AppFrame.GoBack();
                        //else
                        //  _navigationService.Navigate("Main", null);
                    }
                });
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("CriarLancamentoPageViewModel.SalvarLancamentoExecute", ex);
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsBusy = false;
                });
            }

        }

        private void SelecionarTagExecute(object arg)
        {
            var button = arg as ToggleButton;
            var tag = button.Content as string;
            var isChecked = button.IsChecked ?? false;

            string[] tagAtual = Detalhes.Tags.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var items = tagAtual.Count();
            var tempTag = "";

            for (int i = 0; i < tagAtual.Count(); i++)
            {
                if (!tagAtual[i].Equals(tag, StringComparison.CurrentCultureIgnoreCase) && (isChecked || !isChecked))
                    tempTag += tagAtual[i] + (i < items - 1 ? "," : string.Empty);
            }

            var tagSaida = (!String.IsNullOrWhiteSpace(tempTag) ? tempTag : String.Empty);
            if (isChecked)
            {
                if (!String.IsNullOrWhiteSpace(tagSaida) && !tagSaida.Trim().EndsWith(","))
                    tagSaida += ",";
                tagSaida += tag;
            }

            Detalhes.Tags = tagSaida;
        }



        private void AlterarValorRealizadoExecute(object arg)
        {
            //Assina o evento para obter o valor da outra tela (edicao de valor numerico)
            Messenger.Default.Register<DecimalInputEvent>(this, ProcessarValorRealizado);
            Messenger.Default.Send<DecimalnputEventRefresh>(new DecimalnputEventRefresh() { Valor = Detalhes.ValorLancamentoRealizado });

            //_eventAggregator.GetEvent<DecimalInputEvent>().Subscribe(ProcessarValorRealizado);
            //_eventAggregator.GetEvent<DecimalnputEventRefresh>().Publish(Detalhes.ValorLancamentoRealizado);

            //Flyout.ShowAttachedFlyout(arg as FrameworkElement);            
        }

        private void AlterarValorExecute(object arg)
        {
            Messenger.Default.Register<DecimalInputEvent>(this, ProcessarValor);
            Messenger.Default.Send<DecimalnputEventRefresh>(new DecimalnputEventRefresh() { Valor = Detalhes.ValorLancamento });

            //Assina o evento para obter o valor da outra tela (edicao de valor numerico)
            //_eventAggregator.GetEvent<DecimalInputEvent>().Subscribe(ProcessarValor);
            //_eventAggregator.GetEvent<DecimalnputEventRefresh>().Publish(Detalhes.ValorLancamento);

        }

        private void ProcessarValorRealizado(DecimalInputEvent obj)
        {
            Detalhes.ValorLancamentoRealizado = obj.Valor;
            RaisePropertyChanged(() => Detalhes);

            //Cancela a assinatura para evitar chamadas desnecessarias
            //_eventAggregator.GetEvent<DecimalInputEvent>().Unsubscribe(ProcessarValorRealizado);
        }

        private void ProcessarValor(DecimalInputEvent obj)
        {
            Detalhes.ValorLancamento = obj.Valor;
            RaisePropertyChanged(() => Detalhes);

            //Cancela a assinatura para evitar chamadas desnecessarias
            //_eventAggregator.GetEvent<DecimalInputEvent>().Unsubscribe(ProcessarValor);
        }
        #endregion

        #endregion

        #region Propriedades

        #region Commands

        public RelayCommand FixarLancamento
        {
            get;
            private set;
        }
        public RelayCommand<object> AlterarTipo
        {
            get;
            private set;
        }
        public RelayCommand SalvarLancamento
        {
            get;
            private set;
        }

        public RelayCommand<object> SelecionarTag
        {
            get;
            private set;
        }

        public RelayCommand<object> AlterarValor
        {
            get;
            private set;
        }

        public RelayCommand<object> AlterarValorRealizado
        {
            get;
            private set;
        }

        public RelayCommand PageLoadCommand
        {
            get;
            private set;
        }
        #endregion Commands


        #region ViewModel
        public string Message { get; set; }

        private bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                //_IsBusy = value;
                Set(() => IsBusy, ref _IsBusy, value);
            }
        }

        private bool _Fechado;

        public bool Fechado
        {
            get { return _Fechado; }
            set
            {
                if (Detalhes.ValorLancamentoRealizado == 0 && Detalhes.ValorLancamento != 0)
                {
                    Detalhes.ValorLancamentoRealizado = Detalhes.ValorLancamento;
                    RaisePropertyChanged(() => Detalhes.ValorLancamentoRealizado);
                    //OnPropertyChanged("Detalhes.ValorLancamentoRealizado");
                }
                Set(() => Fechado, ref _Fechado, value);
            }
        }

        private IList<ListaSelecao> _Contas = new List<ListaSelecao>();
        public IList<ListaSelecao> Contas
        {
            get
            {
                return _Contas;
            }
            private set
            {
                Set(() => Contas, ref _Contas, value);
            }
        }


        private IReadOnlyList<TagView> _Tags;
        public IReadOnlyList<TagView> Tags
        {
            get
            {
                return _Tags;
            }
            set
            {
                Set(() => Tags, ref _Tags, value);
            }
        }


        private List<ListaSelecao> _ListaRepeticoesFim;
        public List<ListaSelecao> ListaRepeticoesFim
        {
            get
            {
                return _ListaRepeticoesFim;
            }
        }

        private List<ListaSelecao> _ListaRepeticoes;
        public List<ListaSelecao> ListaRepeticoes
        {
            get
            {
                return _ListaRepeticoes;
            }
        }

        private IReadOnlyList<ListaSelecao> _Tipos;
        public IReadOnlyList<ListaSelecao> Tipos
        {
            get
            {
                return _Tipos;
            }
        }


        private ListaSelecao _Repeticao;
        public ListaSelecao Repeticao
        {
            get
            {
                return _Repeticao;
            }
            set
            {
                Set(() => Repeticao, ref _Repeticao, value);
            }
        }

        private ListaSelecao _RepeticaoFim;
        public ListaSelecao RepeticaoFim
        {
            get
            {
                return _RepeticaoFim;
            }
            set
            {
                Set(() => RepeticaoFim, ref _RepeticaoFim, value);
            }
        }



        private ListaSelecao _FormaDePagamento;

        public ListaSelecao FormaDePagamento
        {
            get
            {
                return _FormaDePagamento;
            }
            set
            {
                Set(() => FormaDePagamento, ref _FormaDePagamento, value);
            }
        }


        private IList<ListaSelecao> _FormasDePagamento= new List<ListaSelecao>();

        public IList<ListaSelecao> FormasDePagamento
        {
            get
            {
                return _FormasDePagamento;
            }
            set
            {
                Set(() => FormasDePagamento, ref _FormasDePagamento, value);
            }
        }


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



        private decimal _Valor = 0;
        public decimal Valor
        {
            get
            {
                return _Valor;
            }
            set
            {
                Set(() => Valor, ref _Valor, value);
            }
        }

        public IDecimalInputUserControlViewModel DecimalInputViewModel
        {
            get
            {
                return _decimalInputUserControlViewModel;
            }
        }

        private string _PeriodoSelecionado;
        public string PeriodoSelecionado
        {
            get { return _PeriodoSelecionado; }
            set
            {
                if (String.IsNullOrWhiteSpace(value)) value = "H";
                Set(() => PeriodoSelecionado, ref _PeriodoSelecionado, value);
            }
        }

        private ListaSelecao _ContaSelecionada;
        public ListaSelecao ContaSelecionada
        {
            get { return _ContaSelecionada; }
            set
            {
                Set(() => ContaSelecionada, ref _ContaSelecionada, value);
            }
        }

        private int? _FimRepeticaoQuantidade;
        public int? FimRepeticaoQuantidade
        {
            get { return _FimRepeticaoQuantidade; }
            set
            {
                Set(() => FimRepeticaoQuantidade, ref _FimRepeticaoQuantidade, value);
            }
        }

        private DateTime? _FimRepeticaoData;
        public DateTime? FimRepeticaoData
        {
            get { return _FimRepeticaoData; }
            set
            {
                Set(() => FimRepeticaoData, ref _FimRepeticaoData, value);
            }
        }

        private ListaSelecao _Tipo;
        /// <summary>
        /// Tipo de lançamento (receita ou despesa)
        /// </summary>
        public ListaSelecao Tipo
        {
            get { return _Tipo; }
            set
            {
                Set(() => Tipo, ref _Tipo, value);
            }
        }



        #endregion ViewModel

        #endregion Propriedades

    }

    public sealed class Selecao
    {
        public string Nome { get; set; }
        public ICommand Comando { get; set; }

        public object Parametro { get; set; }
    }
}
