using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public partial class FormaPagamento : INotifyPropertyChanged
    {


        private int _FormaPagamentoId;

        private System.DateTime _DataCriacao;

        private string _Nome;

        public FormaPagamento()
        {

        }

        [PrimaryKey, AutoIncrement]
        public int FormaPagamentoId
        {
            get
            {
                return this._FormaPagamentoId;
            }
            set
            {
                if ((this._FormaPagamentoId != value))
                {
                    this._FormaPagamentoId = value;
                    //this.SendPropertyChanged("FormaPagamentoId");
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

        public string Nome
        {
            get
            {
                return this._Nome;
            }
            set
            {
                if ((this._Nome != value))
                {
                    this._Nome = value;
                    this.SendPropertyChanged("Nome");
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
