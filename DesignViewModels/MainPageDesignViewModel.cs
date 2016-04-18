using DinDinPro.Universal.Models;
using DinDinPro.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DinDinPro.Universal.DesignViewModels
{
    public class MainPageDesignViewModel
    {
        public MainPageDesignViewModel()
        {
            Previsao = new List<PrevisaoInfo>();
            Previsao.Add(new PrevisaoInfo(DateTime.Now.AddMonths(2), 500));
            Previsao.Add(new PrevisaoInfo(DateTime.Now.AddMonths(1), 1500));
            Previsao.Add(new PrevisaoInfo(DateTime.Now.AddMonths(3), 2500));

            Contas = new List<ContaView>();
            Contas.Add(new ContaView() { ContaId = 1, NomeConta = "Itaú", SaldoPrevisto = 500, SaldoRealizado = 510 });
            Contas.Add(new ContaView() { ContaId = 2, NomeConta = "Cartão Mastercard", SaldoPrevisto = 1500, SaldoRealizado = 1500 });
            Contas.Add(new ContaView() { ContaId = 3, NomeConta = "Cartão Visa", SaldoRealizado = 1358, SaldoPrevisto = 1358 });


            ExisteContas = true;
            Saldo = -500;
            Despesas = 1825748;


            this.Expenditure = new List<model>();
            Expenditure.Add(new model() { Expense = "E-Mail", Amount = 20d });
            Expenditure.Add(new model() { Expense = "Skype", Amount = 23d });
            Expenditure.Add(new model() { Expense = "Phone", Amount = 12d });
            Expenditure.Add(new model() { Expense = "Sms", Amount = 19d });
            Expenditure.Add(new model() { Expense = "Facebook", Amount = 10d });
            Expenditure.Add(new model() { Expense = "Twitter", Amount = 10d });
            Expenditure.Add(new model() { Expense = "LinkedIn", Amount = 9d });

            DadosGrafico = new ObservableCollection<GraficoData>();
            DadosGrafico.Add(new GraficoData() { Data = DateTime.Now, Despesa = 100, Receita = 50 });
            DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(-1), Despesa = 1010, Receita = 150 });
            DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(1), Despesa = 10, Receita = 150 });
            DadosGrafico.Add(new GraficoData() { Data = DateTime.Now.AddMonths(2), Despesa = 0, Receita = 510 });

            DadosGraficoDespesa = new List<GraficoDespesa>();
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Item 1", Valor = 200 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Item 2", Valor = 100 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Item 3", Valor = 15 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Item 4", Valor = 500 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Item 5", Valor = 2500 });

            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Cultura", Valor = 200 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Diversao", Valor = 500 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Alimentação", Valor = 250 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Outras despesas", Valor = 25 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Titulo muito grande", Valor = 10 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Vestuario", Valor = 120 });
            DadosGraficoDespesa.Add(new GraficoDespesa() { Despesa = "Outras", Valor = 2500 });
        }

        public bool ExisteContas { get; set; }

        public IList<PrevisaoInfo> Previsao { get; set; }

        public List<ContaView> Contas
        {
            get;
            private set;
        }
        public IList<model> Expenditure
        {
            get;
            set;
        }

        public decimal Saldo { get; set; }
        public decimal Despesas { get; set; }


        public ObservableCollection<GraficoData> DadosGrafico
        {
            get;
            set;
        }
        public List<GraficoDespesa> DadosGraficoDespesa
        {
            get;private set;
        }

    }
    public class model
    {
        public string Expense { get; set; }
        public double Amount { get; set; }
    }
}
