using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public interface ITagRepository
    {
        Task<IList<Tag>> ListarTagsAsync();
        Task<bool> SalvarTagAsync(Tag obj);
        Task<bool> RemoverTagAsync(Tag tag);
        Task<Tag> ObterTag(int tagId);
        Task<bool> ExisteRelacionamento(int tagId);
    }
}
