using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories.Design
{
    public class TagDesignRepository : ITagRepository
    {
        public Task<bool> ExisteRelacionamento(int tagId)
        {
            return Task.FromResult<bool>(true);
        }

        public Task<IList<Tag>> ListarTagsAsync()
        {
            return Task.FromResult<IList<Tag>>(new List<Tag>(new Tag[] {
                        new Tag() { NomeTag = "Tag1 muito grande", Tipo = "+" },
                        new Tag() { NomeTag = "Outra tag grande", Tipo = "+" },
                        new Tag() { NomeTag = "Tag3", Tipo = "+" },
                        new Tag() { NomeTag = "Tag4", Tipo = "+" }
            }));
        }

        public Task<Tag> ObterTag(int tagId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoverTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SalvarTagAsync(Tag obj)
        {
            throw new NotImplementedException();
        }
    }
}
