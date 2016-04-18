using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DinDinPro.Universal.Models
{
    public static class Constantes
    {
        public static string DatabasePath = Path.Combine(new[] { ApplicationData.Current.LocalFolder.Path, "database", "dindin.pro.sqlite" });
        public const string PAGINA_PRINCIPAL = "MainPage";
        public const string PAGINA_CONTA = "ContaPage";
        public const string PAGINA_CONTA_DETALHE = "ContaDetalhePage";
        public const string PAGINA_CONFIGURACAO = "ConfiguracaoPage";
        public const string PAGINA_LANCAMENTO = "CriarLancamentoPage";
        public const string PAGINA_FORMA_PAGAMENTO = "FormaPagamentoPage";
        public const string PAGINA_FORMA_PAGAMENTO_CRIAR = "FormaPagamentoCriarPage";
        public const string PAGINA_LANCAMENTO_DETALHE = "LancamentoDetalhePage";
        public const string PAGINA_LOGIN = "LoginPage";
        public const string PAGINA_TAGS = "TagsPage";
        public const string PAGINA_TAGS_CRIAR = "TagsCriarPage";
        public const string PAGINA_PIN_INPUT = "PinInputPage";
        public const string PAGINA_RELATORIO = "RelatorioPage";
        public const string PAGINA_CONTAS = "ContaListPage";
        public const string PAGINA_SOBRE = "SobrePage";

        public const string CONFIG_KEY_EXECUCOES = "DIN_DIN_UNIVERSAL_QTDE_EXECUCOES";
    }
}
