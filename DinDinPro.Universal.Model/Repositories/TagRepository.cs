using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly IDataService _dataService;       

        public TagRepository(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async System.Threading.Tasks.Task<IList<Models.Tag>> ListarTagsAsync()
        {
            return await _dataService.Contexto.Table<Tag>().ToListAsync();
        }

        public async System.Threading.Tasks.Task<bool> SalvarTagAsync(Models.Tag obj)
        {
            if (obj.TagId <= 0)
                return await _dataService.Contexto.InsertAsync(obj) > 0;

            return await _dataService.Contexto.UpdateAsync(obj) > 0;
        }

        public async System.Threading.Tasks.Task<bool> RemoverTagAsync(Tag tag)
        {
            return await _dataService.Contexto.DeleteAsync(tag) > 0;
        }


        public async System.Threading.Tasks.Task<Tag> ObterTag(int tagId)
        {
            var obj = await _dataService.Contexto.FindAsync<Tag>(tagId);
            return obj;
        }


        public async System.Threading.Tasks.Task<bool> ExisteRelacionamento(int tagId)
        {
            //Verifica se a tag está sendo utilizada nos lançamentos
            var items = await _dataService.Contexto.ExecuteScalarAsync<int>("select count(1) from LancamentoTag where TagId = ?", tagId);
            
            return items > 0;
        }
    }
}
