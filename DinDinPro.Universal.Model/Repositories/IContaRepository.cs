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
        Task<int> CriarContaAsync(string nome, double saldoInicial);
        Task<bool> RemoverContaAsync(int contaId);
        Task<IReadOnlyList<ContaView>> ListarContasAsync();
        Task<double> ObterSaldoAtual();
        Task<double> ObterSaldoPrevistoAtual();
        Task<Conta> ObterDetalhes(int contaId);
        Task<Conta> ObterDetalhes();
        Task<IList<LancamentoView>> ObterLancamentos(int? contaId, int? lancamentoId, DateTime? periodo, TipoOrdenacaoLancamento ordernacao = TipoOrdenacaoLancamento.toDataDesc);
        Task<IList<LancamentoView>> ObterLancamentos(DateTime periodo);        
        Task<bool> CriarLancamentoAsync(LancamentoView lancamento);
        Task<IReadOnlyList<TagView>> ListarTagsAsync(string tipoLancamento);
        Task<TagView> ListarTagsAsync(string nomeTag, string tipoLancamento);
        Task<double> ObterDespesasFuturas();
        Task<bool> RemoverLancamentoAsync(int lancamentoId);
        Task<IReadOnlyList<GraficoDespesa>> ListarDespesasAgrupadas(DateTime periodo);
        Task<LancamentoView> ObterLancamento(int lancamentoId);
        Task<IList<PrevisaoInfo>> ListarPrevisao(DateTime dataBase);
        Task<IReadOnlyList<LancamentoView>> ListarDespesasAtrasadas();
        Task<IReadOnlyList<LancamentoView>> ListarDespesasVencendo();
        Task<bool> MarcarDespesaEfetivada(int lancamentoId);
        Task<double> ObterSaldoMesAtual();
        Task<double> ObterSaldoMesAtualPrevisto();
    }
}
