using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public interface IConfiguracaoRepository
    {
        Task<bool> SalvarConfiguracao(string chave, string valor);
        Task<bool> CriarConfiguracao(string chave, string valor);
        Task<string> ObterValor(string chave);
    }
}
