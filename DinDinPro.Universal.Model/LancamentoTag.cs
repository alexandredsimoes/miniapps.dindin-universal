using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public partial class LancamentoTag : INotifyPropertyChanged
    {

        private int _LancamentoId;

        private int _TagId;

        private System.Nullable<System.DateTime> _DataCriacao;

        private bool _Sincronizado;
        private int _LancamentoTagId;

       

        public LancamentoTag()
        {

        }


        

        [PrimaryKey, AutoIncrement]
        public int LancamentoTagId
        {
            get
            {
                return this._LancamentoTagId;
            }
            set
            {
                if ((this._LancamentoTagId != value))
                {

                    this._LancamentoTagId = value;
                    this.SendPropertyChanged("LancamentoTagId");
                }
            }
        }

        
        public int LancamentoId
        {
            get
            {
                return this._LancamentoId;
            }
            set
            {
                if ((this._LancamentoId != value))
                {
                    
                    this._LancamentoId = value;
                    this.SendPropertyChanged("LancamentoId");
                }
            }
        }

        public int TagId
        {
            get
            {
                return this._TagId;
            }
            set
            {
                if ((this._TagId != value))
                {
                    
                    this._TagId = value;
                    this.SendPropertyChanged("TagId");
                }
            }
        }

        public System.Nullable<System.DateTime> DataCriacao
        {
            get
            {
                return this._DataCriacao;
            }
            set
            {
                if ((this._DataCriacao != value))
                {
                    this._DataCriacao = value;
                    this.SendPropertyChanged("DataCriacao");
                }
            }
        }

        public bool Sincronizado
        {
            get
            {
                return this._Sincronizado;
            }
            set
            {
                if ((this._Sincronizado != value))
                {
                    this._Sincronizado = value;
                    this.SendPropertyChanged("Sincronizado");
                }
            }
        }


              
        public event PropertyChangedEventHandler PropertyChanged; 

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
