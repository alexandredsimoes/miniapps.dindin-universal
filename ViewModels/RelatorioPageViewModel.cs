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

namespace DinDinPro.Universal.ViewModels
{
    public class RelatorioPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IContaRepository _contaRepository;
        private readonly IAlertMessageService _alertMessageService;
        private readonly ResourceLoader _resourceLoader;

        public RelatorioPageViewModel(IDataService dataService, INavigationService navigationService, IContaRepository contaRepository,
            IAlertMessageService alertMessageService, ResourceLoader resourceLoader)
        {
            _navigationService = navigationService;
            _contaRepository = contaRepository;
            _alertMessageService = alertMessageService;
            _resourceLoader = resourceLoader;

            _DadosGrafico = new ObservableCollection<GraficoData>();

            PageLoad = new RelayCommand(() => { CarregarSumario(); });
            Periodo = DateTime.Now;
        }

        #region Commands

        #region Propriedades
        public RelayCommand PageLoad { get; private set; }
        #endregion

        #endregion

        public async Task CarregarSumario()
        {
            //Previsão
            Previsao = await _contaRepository.ListarPrevisao(DateTime.Now);
            RaisePropertyChanged(() => Previsao);

            var despesasGrafico = await _contaRepository.ListarDespesasAgrupadas(_periodo);
            if (despesasGrafico.Count > 0)
                DespesaMaximaNoMes = despesasGrafico.Max(c => c.Valor);
            DadosGraficoDespesa = despesasGrafico;
        }

        #region ViewModel

        #region Propriedades
        private double _DespesaMaximaNoMes = 0;

        public double DespesaMaximaNoMes
        {
            get { return _DespesaMaximaNoMes; }
            set { Set(() => DespesaMaximaNoMes, ref _DespesaMaximaNoMes, value); }
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

        public IList<PrevisaoInfo> Previsao { get; set; }

        private ObservableCollection<GraficoData> _DadosGrafico;
        public ObservableCollection<GraficoData> DadosGrafico
        {
            get { return _DadosGrafico; }
            set { Set(() => DadosGrafico, ref _DadosGrafico, value); }
        }

        private IReadOnlyList<GraficoDespesa> _DadosGraficoDespesa;
        public IReadOnlyList<GraficoDespesa> DadosGraficoDespesa
        {
            get { return _DadosGraficoDespesa; }
            set
            {
                Set(() => DadosGraficoDespesa, ref _DadosGraficoDespesa, value);
            }
        }
        #endregion

        #endregion
    }
}
