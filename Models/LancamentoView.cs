using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models
{
    public class LancamentoView : Lancamento
    {
        private string _Tags;
        public string Tags
        {
            get { return _Tags; }
            set
            {
                Set(() => Tags, ref _Tags, value);
            }
        }

        private string _NomeConta;
        public string NomeConta
        {
            get { return _NomeConta; }
            set { Set(() => NomeConta, ref _NomeConta, value); }
        }

        private string _FormaPagamento;
        public string FormaPagamento
        {
            get { return _FormaPagamento; }
            set { Set(() => FormaPagamento, ref _FormaPagamento, value); }
        }
    }
}
