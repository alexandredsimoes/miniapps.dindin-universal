using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Async;

namespace DinDinPro.Universal.Model.Design
{
    public class DesignDataService : IDataService
    {
        public SQLiteAsyncConnection Contexto
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task InicializarBaseDeDados(string[] tagsReceita, string[] tagsDespesa, string[] formasPagamento, int versao)
        {
            throw new NotImplementedException();
        }
    }
}
