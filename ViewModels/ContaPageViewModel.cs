using DinDinPro.Universal.Models;
using DinDinPro.Universal.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using DinDinPro.Universal.Models.Repositories;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Command;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using DinDinPro.Universal.Aggregators;

namespace DinDinPro.Universal.ViewModels
{
    public class ContaPageViewModel : BaseViewModel
    {
        private readonly IContaRepository _contaRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;
        private readonly IDecimalInputUserControlViewModel _decimalInputUserControlViewModel;

        public ContaPageViewModel(INavigationService navigationService,
            IContaRepository contaRepository, ResourceLoader resourceLoader,
            IAlertMessageService messageService,
            IDecimalInputUserControlViewModel decimalInputUserControlViewModel)
        {
            _contaRepository = contaRepository;
            _navigationService = navigationService;
            _resourceLoader = resourceLoader;
            _alertMessageService = messageService;
            _decimalInputUserControlViewModel = decimalInputUserControlViewModel;

            SalvarConta = new RelayCommand(SalvarContaExecute);
            RemoverConta = new RelayCommand(RemoverContaExecuteAsync);
            AlterarValor = new RelayCommand(AlterarValorExecute);

            Detalhes = new Conta()
            {
                ValorInicial = 0,
                DataCriacao = DateTime.Now
            };
        }

        private async void SalvarContaExecute()
        {
            if (String.IsNullOrWhiteSpace(Detalhes.NomeConta))
            {
                var msg = _resourceLoader.GetString("ContaPageNomeContaEmpty");
                await _alertMessageService.ShowAsync(String.Join(Environment.NewLine, msg), _resourceLoader.GetString("ApplicationTitle"));
                return;
            }

            //if (SaldoNegativo && Detalhes.ValorInicial > 0)
              //  Detalhes.ValorInicial = 0 - Detalhes.ValorInicial;

            var id = await _contaRepository.CriarContaAsync(Detalhes.NomeConta);
            if (id > 0 && Detalhes.ValorInicial != 0)
            {                
                await _contaRepository.CriarLancamentoAsync(new LancamentoView()
                {
                    ContaId = id,
                    DataCriacao = DateTime.Now,
                    DataLancamento = DateTime.Now,
                    Descricao = _resourceLoader.GetString("SaldoInicial"),
                    Fechado = true,
                    Parcela = 1,
                    Tags = _resourceLoader.GetString("SaldoInicialTag"),
                    Tipo = SaldoNegativo ? "-" : "+",
                    ValorLancamentoRealizado = Detalhes.ValorInicial ?? 0,
                    ValorLancamento = Detalhes.ValorInicial ?? 0                    
                });
            }
            //_navigationService.GoBack();
            ((AppShell)Window.Current.Content).AppFrame.GoBack();
        }

        #region Propriedades
        #region Commands
        public RelayCommand RemoverConta
        {
            get;
            private set;
        }
        public RelayCommand SalvarConta
        {
            get;
            private set;
        }

        public RelayCommand AlterarValor { get; private set; }
        #endregion

        #region ViewModel
        private bool _SaldoNegativo;

        public bool SaldoNegativo
        {
            get { return _SaldoNegativo; }
            set { Set(() => SaldoNegativo, ref _SaldoNegativo, value); }
        }

        public Conta Detalhes
        {
            get;
            private set;
        }
        public IDecimalInputUserControlViewModel DecimalInputViewModel
        {
            get
            {
                return _decimalInputUserControlViewModel;
            }
        }
        #endregion

        #endregion

        #region Métodos
        #region Commands

        private void AlterarValorExecute()
        {
            Messenger.Default.Register<DecimalInputEvent>(this, ProcessarValor);
            Messenger.Default.Send(new DecimalnputEventRefresh() { Valor = Detalhes.ValorInicial ?? 0 });
        }

        private void ProcessarValor(DecimalInputEvent obj)
        {
            Detalhes.ValorInicial = obj.Valor;
            RaisePropertyChanged(() => Detalhes);
        }

        private async void RemoverContaExecuteAsync()
        {
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("MainPageRemoverContaButtonSim"),
                Invoked = async () =>
                {
                    await _contaRepository.RemoverContaAsync(Detalhes.ContaId);
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
        #endregion
        #endregion

    }
}
