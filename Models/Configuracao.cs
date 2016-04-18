using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Models
{
    public partial class Configuracao : INotifyPropertyChanged
    {

        private string _Nome;

        private string _Valor;

        private System.Nullable<bool> _Sincronizado;



        public Configuracao()
        {

        }

        [PrimaryKey]
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

        public string Valor
        {
            get
            {
                return this._Valor;
            }
            set
            {
                if ((this._Valor != value))
                {
                    this._Valor = value;
                    this.SendPropertyChanged("Valor");
                }
            }
        }

        public System.Nullable<bool> Sincronizado
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
