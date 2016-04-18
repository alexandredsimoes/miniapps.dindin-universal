using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DinDinPro.Universal.Models
{
    public partial class Conta :  INotifyPropertyChanged
    {        

        private int _ContaId;

        private System.DateTime _DataCriacao;

        private System.Nullable<System.DateTime> _DataAlteracao;

        private string _NomeConta;

        private System.Nullable<decimal> _ValorInicial;

        private System.DateTime _PeriodoExibicaoInicio;

        private System.DateTime _PeriodoExibicaoFinal;

        private string _TipoPeriodo;

        private bool _Sincronizado;

        private bool _Fechado;

        public Conta()
        {
                        
        }

        [PrimaryKey, AutoIncrement]
        public int ContaId
        {
            get
            {
                return this._ContaId;
            }
            set
            {
                if ((this._ContaId != value))
                {
                    this._ContaId = value;
                    this.SendPropertyChanged("ContaId");
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
                    this._DataCriacao = value.Date;
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

        public string NomeConta
        {
            get
            {
                return this._NomeConta;
            }
            set
            {
                if ((this._NomeConta != value))
                {
                    this._NomeConta = value;
                    this.SendPropertyChanged("NomeConta");
                }
            }
        }

        public System.Nullable<decimal> ValorInicial
        {
            get
            {
                return this._ValorInicial;
            }
            set
            {
                if ((this._ValorInicial != value))
                {
                    this._ValorInicial = value;
                    this.SendPropertyChanged("ValorInicial");
                }
            }
        }

        public System.DateTime PeriodoExibicaoInicio
        {
            get
            {
                return this._PeriodoExibicaoInicio;
            }
            set
            {
                if ((this._PeriodoExibicaoInicio != value))
                {
                    this._PeriodoExibicaoInicio = value;
                    this.SendPropertyChanged("PeriodoExibicaoInicio");
                }
            }
        }

        public System.DateTime PeriodoExibicaoFinal
        {
            get
            {
                return this._PeriodoExibicaoFinal;
            }
            set
            {
                if ((this._PeriodoExibicaoFinal != value))
                {
                    this.SendPropertyChanged("PeriodoExibicaoFinal");
                }
            }
        }

        public string TipoPeriodo
        {
            get
            {
                return this._TipoPeriodo;
            }
            set
            {
                if ((this._TipoPeriodo != value))
                {
                    this._TipoPeriodo = value;
                    this.SendPropertyChanged("TipoPeriodo");
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

        public bool Fechado
        {
            get
            {
                return this._Fechado;
            }
            set
            {
                if ((this._Fechado != value))
                {
                    this._Fechado = value;
                    this.SendPropertyChanged("Fechado");
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
