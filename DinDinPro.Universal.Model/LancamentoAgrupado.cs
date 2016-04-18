using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public class LancamentoAgrupado
    {
        public DateTime Data { get; set; }
        public ObservableCollection<LancamentoView> Items { get; set; }
    }
}
