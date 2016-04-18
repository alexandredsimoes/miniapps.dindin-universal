using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public partial class Tag : INotifyPropertyChanged
    {


        private int _TagId;

        private System.DateTime _DataCriacao;

        private System.Nullable<System.DateTime> _DataAlteracao;

        private string _NomeTag;

        private string _Tipo;

        private bool _Sincronizado;

      
        public Tag()
        {
           
        }

        [PrimaryKey, AutoIncrement]
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
                    //this.SendPropertyChanged("TagId");
                }
            }
        }

        public System.DateTime DataCriacao
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

        public System.Nullable<System.DateTime> DataAlteracao
        {
            get
            {
                return this._DataAlteracao;
            }
            set
            {
                if ((this._DataAlteracao != value))
                {
                    this._DataAlteracao = value;
                    this.SendPropertyChanged("DataAlteracao");
                }
            }
        }

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

        public string Tipo
        {
            get
            {
                return this._Tipo;
            }
            set
            {
                if ((this._Tipo != value))
                {
                    this._Tipo = value;
                    this.SendPropertyChanged("Tipo");
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
