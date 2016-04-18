using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models
{
    public class PrevisaoInfo
    {
        public PrevisaoInfo()
        {

        }
        public PrevisaoInfo(DateTime data, double valor)
        {
            Data = data;
            Valor = valor;
        }
        public DateTime Data { get; set; }
        public double Valor { get; set; }
    }
}
