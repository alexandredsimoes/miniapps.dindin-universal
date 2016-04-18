using DinDinPro.Universal.Models;
using DinDinPro.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.DesignViewModels
{
    public class CriarLancamentoPageDesignViewModel
   {        

        public CriarLancamentoPageDesignViewModel()
        {
            Detalhes = new LancamentoView()
            {
                DataCriacao = DateTime.Now,
                DataLancamento = DateTime.Now,
                ValorLancamento = 10850360,
                ValorLancamentoRealizado = 10850360,
                Fechado = true,
                Tags = "alimentação,despesas pessoais,outras despesas",
                Tipo = "-",
                TipoRepeticao = 1,
                TipoFimRepeticao = 1,
                FimRepeticaoData = DateTime.Now,
                FimRepeticaoQuantidade = 1
            };

            PeriodoSelecionado = "D";
            TagsSelecionadas = "alimentação,condominio,lazer";

            //Cria algumas tags
            Tags = new List<Selecao>();

            var formas = new string[] { "alimentação", "despesas pessoais", "automóvel", "financiamento", "juros", "empréstimo", 
                    "vestuário", "diversos", "balada", "testes", "Mais tags", "Outras tags", "Tag muito grande na descrição" };
            foreach (var item in formas)
            {
                var selecao = new Selecao() { Nome = item };
                Tags.Add(selecao);
            }

            
            Contas = new List<ContaView>();
            Contas.Add(new ContaView() { ContaId = 1, NomeConta = "Itaú" });
            Contas.Add(new ContaView() { ContaId = 2, NomeConta = "Cartão Mastercard" });
            Contas.Add(new ContaView() { ContaId = 3, NomeConta = "Cartão Visa" });
            ContaSelecionada = Contas[0];

            
            FormasDePagamento = new List<FormaPagamento>();
            FormasDePagamento.Add(new FormaPagamento() { DataCriacao = DateTime.Now, Nome = "Dinheiro", FormaPagamentoId = 1 });
            FormasDePagamento.Add(new FormaPagamento() { DataCriacao = DateTime.Now, Nome = "Cheque", FormaPagamentoId = 1 });
            FormasDePagamento.Add(new FormaPagamento() { DataCriacao = DateTime.Now, Nome = "Cartão de crédito", FormaPagamentoId = 1 });
            FormasDePagamento.Add(new FormaPagamento() { DataCriacao = DateTime.Now, Nome = "Boleto", FormaPagamentoId = 1 });

            FormaDePagamento = FormasDePagamento[0];

            ListaRepeticoes = new List<ListaSelecao>();
            ListaRepeticoes.Add(new ListaSelecao() { Indice = 0, Descricao = "Nunca" });
            ListaRepeticoes.Add(new ListaSelecao() { Indice = 1, Descricao = "Diario" });
            ListaRepeticoes.Add(new ListaSelecao() { Indice = 2, Descricao = "Mensal" });
            ListaRepeticoes.Add(new ListaSelecao() { Indice = 3, Descricao = "Semanal" });
            ListaRepeticoes.Add(new ListaSelecao() { Indice = 4, Descricao = "Anual" });

            ListaRepeticoesFim = new List<ListaSelecao>();
            ListaRepeticoesFim.Add(new ListaSelecao() { Indice = 0, Descricao = "Não repetir" });
            ListaRepeticoesFim.Add(new ListaSelecao() { Indice = 1, Descricao = "Data específica" });
            ListaRepeticoesFim.Add(new ListaSelecao() { Indice = 2, Descricao = "Depois de x repetições" });

            Tipos = new List<ListaSelecao>();
            Tipos.Add(new ListaSelecao(){
                Descricao = "Receita",
                Indice = 0
            });

            Tipos.Add(new ListaSelecao()
            {
                Descricao = "Despesa",
                Indice = 0
            });

            Tipo = Tipos[0];
            
            


            Repeticao = ListaRepeticoes[2];
            RepeticaoFim = ListaRepeticoesFim[2];

        }

        public Lancamento Detalhes { get; set; }

        public string PeriodoSelecionado
        {
            get;
            set;
        }

        public string TagsSelecionadas { get; set; }

        public DateTime DataLancamento
        {
            get;
            set;
        }

        public List<Selecao> Tags
        {
            get;
            set;
        }

        public List<ContaView> Contas
        {
            get;
            set;
        }

        public FormaPagamento FormaDePagamento { get; set; }

        public List<FormaPagamento> FormasDePagamento { get; set; }

        public List<ListaSelecao> ListaRepeticoesFim
        {
            get;
            set;
        }

        public List<ListaSelecao> ListaRepeticoes
        {
            get;
            set;
        }

        public List<ListaSelecao> Tipos
        {
            get;
            set;
        }

        public ListaSelecao Tipo { get; set; }

        public ListaSelecao Repeticao { get; set; }

        public ListaSelecao RepeticaoFim { get; set; }

        public Conta ContaSelecionada { get; set; }
        public DateTime FimRepeticaoData { get; set; }
    }
}
