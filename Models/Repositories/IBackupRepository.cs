using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public interface IBackupRepository
    {
        Task<bool> CriarContas(List<Conta> contas);
        Task<bool> CriarTags(List<Tag> tags);
        Task<Tag> LocalizarTagPorNome(string nomeTag);
        Task<Lancamento> CriarLancamento(Lancamento lancamento);
        Task<Conta> LocalizarContaPorNome(string nomeConta);
        Task<bool> CriarLancamentoTag(LancamentoTag lancamentoTag);
        Task<bool> LimparBaseDados();
        Task<IEnumerable<Conta>> ListarContas();
        Task<IEnumerable<Tag>> ListarTags();
        Task<IEnumerable<Lancamento>> ListarLancamentos();
        Task<IEnumerable<LancamentoTag>> ListarLancamentosTag(int lancamentoId);
    }
}
