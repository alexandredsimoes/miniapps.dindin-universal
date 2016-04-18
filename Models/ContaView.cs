using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models
{
    public class ContaView : Conta
    {
        private decimal saldoRealizado = 0 ;
        private decimal saldoPrevisto = 0;

        public decimal SaldoRealizado
        {
            get
            {
                return saldoRealizado;
            }
            set
            {
                saldoRealizado = value;
                SendPropertyChanged("SaldoRealizado");
            }
        }

        public decimal SaldoPrevisto
        {
            get
            {
                return saldoPrevisto;
            }
            set
            {
                saldoPrevisto = value;
                SendPropertyChanged("SaldoPrevisto");
            }
        }
    }
}
