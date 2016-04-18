using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.ViewManagement;

namespace DinDinPro.Universal.DesignViewModels
{
    public class ContaDetalhesPageDesignViewModel
    {
        public ContaDetalhesPageDesignViewModel() 
        {
            PopularDadosExemplo();
        }

       
        private void PopularDadosExemplo()
        {
            Periodo = DateTime.Now;
            Lancamentos = new ObservableCollection<LancamentoView>();
            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now,
                Descricao = "Apenas a descrição da conta muito mas muito comprida mesmo. Porque o cara deve ser sem noção das coisas.",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "+",
                ValorLancamento = 980752.18D,
                ValorLancamentoRealizado = 980752.18D,
                NomeConta = "Cartão de crédito"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now,
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "+",
                ValorLancamento = 980752.18D,
                ValorLancamentoRealizado = 980752.18D,
                NomeConta = "Cartão de crédito"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now,
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "+",
                ValorLancamento = 980752.18D,
                ValorLancamentoRealizado = 980752.18D,
                NomeConta = "Cartão de crédito"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(3),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "alimentação,despesas pessoais,médico,outra tag muito grande,testando",
                Tipo = "+",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Itaú"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(7),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "-",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Itaú"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(7),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "+",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Cartão de crédito"
            });

            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(7),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "-",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Cartão de crédito"
            });
            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(7),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "-",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Itaú"
            });
            Lancamentos.Add(new LancamentoView()
            {
                ContaId = 1,
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now.AddDays(7),
                Descricao = "Apenas a descrição da conta",
                Fechado = true,
                LancamentoId = 1,
                Tags = "Tag1,Tag2,Tag3,Tag4,Tag5",
                Tipo = "-",
                ValorLancamento = 500,
                ValorLancamentoRealizado = 500,
                NomeConta = "Itaú"
            });

            TiposOrdenacao = new List<ListaSelecao>();
            TiposOrdenacao.Add(new ListaSelecao() { Descricao = "Item", Indice = 0, Tag = "0" });

            TipoOrdenacao = TiposOrdenacao[0];
            ExisteLancamentos = true;
        }

        public ObservableCollection<LancamentoView> Lancamentos
        {
            get;
            private set;

        }

        public DateTime Periodo
        {
            get;
            private set;
        }

        public IList<ListaSelecao> TiposOrdenacao
        {
            get; set;
        }


        public ListaSelecao TipoOrdenacao
        {
            get; set;
        }

        public bool ExisteLancamentos { get; set; }
    }
}
