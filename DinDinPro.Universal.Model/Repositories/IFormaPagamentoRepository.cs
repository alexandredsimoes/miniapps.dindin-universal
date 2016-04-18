using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public interface IFormaPagamentoRepository
    {
        Task<IReadOnlyList<FormaPagamento>> ListarFormas();
        Task<FormaPagamento> ObterDetalhes(int formaPagamentoId);
        Task<bool> RemoverFormaPagamento(FormaPagamento obj);
        Task<bool> SalvarFormaPagamento(FormaPagamento obj);
        Task<bool> ExisteRelacionamento(int formaPagamentoId);
    }
}
