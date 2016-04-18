using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public interface IDataService
    {
        Task InicializarBaseDeDados(string[] tagsReceita, string[] tagsDespesa, string[] formasPagamento, int versao);
        SQLiteAsyncConnection Contexto
        {
            get;            
        }
    }
}
