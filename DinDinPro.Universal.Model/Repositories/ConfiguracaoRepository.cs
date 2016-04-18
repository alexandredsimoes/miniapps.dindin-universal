using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models.Repositories
{
    public class ConfiguracaoRepository : IConfiguracaoRepository
    {
        private readonly IDataService _dataService;

        public ConfiguracaoRepository(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<bool> CriarConfiguracao(string chave, string valor)
        {
            return await SalvarConfiguracao(chave, valor);
        }

        public async Task<string> ObterValor(string chave)
        {
            var config = await _dataService.Contexto.Table<Configuracao>().Where(c => c.Nome == chave).FirstOrDefaultAsync();

            if (config == null) return null;

            return config.Valor;
        }

        public async Task<bool> SalvarConfiguracao(string chave, string valor)
        {
            var config = await _dataService.Contexto.Table<Configuracao>().Where(c => c.Nome == chave).FirstOrDefaultAsync();
            var insert = config == null;
            if (insert)
                config = new Configuracao() { Nome = chave, Valor = valor, Sincronizado = false };
            else
                config.Valor = valor;

            return insert ? await _dataService.Contexto.InsertAsync(config) > 0 : await _dataService.Contexto.UpdateAsync(config) > 0;            
        }
    }
}
