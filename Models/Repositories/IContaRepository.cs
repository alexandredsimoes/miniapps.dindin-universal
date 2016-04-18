using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public interface IContaRepository
    {
        Task<int> CriarContaAsync(string nome);
        Task<bool> RemoverContaAsync(int contaId);
        Task<IReadOnlyList<ContaView>> ListarContasAsync();
        Task<decimal> ObterSaldoAtual();
        Task<decimal> ObterSaldoPrevistoAtual();
        Task<Conta> ObterDetalhes(int contaId);
        Task<IList<LancamentoView>> ObterLancamentos(int? contaId, int? lancamentoId, DateTime? periodo, TipoOrdenacaoLancamento ordernacao = TipoOrdenacaoLancamento.toDataDesc);
        Task<IList<LancamentoView>> ObterLancamentos(DateTime periodo);        
        Task<bool> CriarLancamentoAsync(LancamentoView lancamento);
        Task<IReadOnlyList<TagView>> ListarTagsAsync(string tipoLancamento);
        Task<decimal> ObterDespesasFuturas();
        Task<bool> RemoverLancamentoAsync(int lancamentoId);
        Task<IReadOnlyList<GraficoDespesa>> ListarDespesasAgrupadas(DateTime periodo);
        Task<LancamentoView> ObterLancamento(int lancamentoId);
        Task<IList<PrevisaoInfo>> ListarPrevisao(DateTime dataBase);
    }
}
