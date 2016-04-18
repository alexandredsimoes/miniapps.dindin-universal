using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models.Repositories
{
    public class FormaPagamentoRepository : IFormaPagamentoRepository
    {
        private readonly IDataService _dataService;

        public FormaPagamentoRepository(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async System.Threading.Tasks.Task<IReadOnlyList<Models.FormaPagamento>> ListarFormas()
        {
            return await _dataService.Contexto.Table<FormaPagamento>().ToListAsync();
        }


        public async System.Threading.Tasks.Task<FormaPagamento> ObterDetalhes(int formaPagamentoId)
        {
            return await _dataService.Contexto.Table<FormaPagamento>().Where(c => c.FormaPagamentoId == formaPagamentoId).FirstOrDefaultAsync();
        }

        public async System.Threading.Tasks.Task<bool> RemoverFormaPagamento(FormaPagamento obj)
        {
            return await _dataService.Contexto.DeleteAsync(obj) > 0;
        }

        public async System.Threading.Tasks.Task<bool> SalvarFormaPagamento(FormaPagamento obj)
        {
            if (obj.FormaPagamentoId > 0)
                return await _dataService.Contexto.UpdateAsync(obj) > 0;
            else
                return await _dataService.Contexto.InsertAsync(obj) > 0;
        }

        public async System.Threading.Tasks.Task<bool> ExisteRelacionamento(int formaPagamentoId)
        {
            var items = await _dataService.Contexto.ExecuteScalarAsync<int>("select count(1) from Lancamento where FormaPagamentoId = ?", formaPagamentoId);
            return items > 0;
        }
    }
}
