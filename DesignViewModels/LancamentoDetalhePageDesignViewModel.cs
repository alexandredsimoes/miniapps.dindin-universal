using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.DesignViewModels
{
    public class LancamentoDetalhePageDesignViewModel
    {
        public LancamentoDetalhePageDesignViewModel()
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
        }

        public Lancamento Detalhes { get; set; }
    }
}
