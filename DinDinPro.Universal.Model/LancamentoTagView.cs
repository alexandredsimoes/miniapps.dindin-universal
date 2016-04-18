using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.Models
{
    public class LancamentoTagView : LancamentoTag
    {
        private string _NomeTag;

        public string NomeTag
        {
            get
            {
                return this._NomeTag;
            }
            set
            {
                if ((this._NomeTag != value))
                {
                    this._NomeTag = value;
                    this.SendPropertyChanged("NomeTag");
                }
            }
        }
    }
}
