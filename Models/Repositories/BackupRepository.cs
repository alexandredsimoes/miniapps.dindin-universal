using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models.Repositories
{
    public class BackupRepository : IBackupRepository
    {
        private readonly IDataService _dataService;

        public BackupRepository(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async System.Threading.Tasks.Task<bool> CriarContas(List<Models.Conta> contas)
        {
            return await _dataService.Contexto.InsertAllAsync(contas) > 0;
        }

        public async System.Threading.Tasks.Task<bool> CriarTags(List<Models.Tag> tags)
        {
            return await _dataService.Contexto.InsertAllAsync(tags) > 0;
        }


        public async System.Threading.Tasks.Task<Tag> LocalizarTagPorNome(string nomeTag)
        {
            return await _dataService.Contexto.Table<Tag>().Where(c => c.NomeTag == nomeTag).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task<Models.Lancamento> CriarLancamento(Models.Lancamento lancamento)
        {
            await _dataService.Contexto.InsertAsync(lancamento);
            return lancamento;
        }


        public async System.Threading.Tasks.Task<Conta> LocalizarContaPorNome(string nomeConta)
        {
            return await _dataService.Contexto.Table<Conta>().Where(c => c.NomeConta == nomeConta).FirstOrDefaultAsync();
        }


        public async System.Threading.Tasks.Task<bool> CriarLancamentoTag(LancamentoTag lancamentoTag)
        {
            return await _dataService.Contexto.InsertAsync(lancamentoTag) > 0;
        }


        public async System.Threading.Tasks.Task<bool> LimparBaseDados()
        {
            var result = false;
            try
            {
                await _dataService.Contexto.ExecuteAsync("DELETE FROM LancamentoTag");
                await _dataService.Contexto.ExecuteAsync("DELETE FROM Lancamento");
                await _dataService.Contexto.ExecuteAsync("DELETE FROM Tag");
                await _dataService.Contexto.ExecuteAsync("DELETE FROM Conta");
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }



        public async System.Threading.Tasks.Task<IEnumerable<Tag>> ListarTags()
        {
            return await _dataService.Contexto.Table<Tag>().ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Lancamento>> ListarLancamentos()
        {
            return await _dataService.Contexto.Table<Lancamento>().ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<LancamentoTag>> ListarLancamentosTag(int lancamentoId)
        {
            return await _dataService.Contexto.Table<LancamentoTag>().Where(c=>c.LancamentoId == lancamentoId).ToListAsync();
        }


        public async System.Threading.Tasks.Task<IEnumerable<Conta>> ListarContas()
        {
            return await _dataService.Contexto.Table<Conta>().ToListAsync();
        }
    }
}
