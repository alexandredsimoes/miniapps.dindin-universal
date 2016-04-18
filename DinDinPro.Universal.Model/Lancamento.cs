using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.Models
{
    public partial class Lancamento : ObservableObject
    {
        private int _LancamentoId;
        private System.DateTime _DataCriacao;
        private DateTime? _DataAlteracao;
        private double _ValorLancamento;
        private double _ValorLancamentoRealizado;
        private DateTime? _DataLancamento;
        private string _Descricao;
        private int _ContaId;
        private int _TipoRepeticao;
        private int _TipoFimRepeticao;
        private int _Parcela;
        private DateTime? _FimRepeticaoData;
        private int? _FimRepeticaoQuantidade;
        private string _Tipo;
        private int? _LancamentoPaiId;
        private bool _Compartilhar;
        private bool _Sincronizado;
        private bool _Fechado;
        private bool _ReceberAviso;
        private int? _FormaPagamentoId;
        private string _Tags = string.Empty;
        private string _NumeroDocumento;
        HashSet<LancamentoTag> _TagsSelecionadas;
        
        public Lancamento()
        {
            _Tags = String.Empty;
            _TagsSelecionadas = new HashSet<LancamentoTag>();
        }

        [PrimaryKey, AutoIncrement]
        public int LancamentoId
        {
            get
            {
                return this._LancamentoId;
            }
            set
            {
                Set(() => LancamentoId, ref _LancamentoId, value);
            }
        }

        [Ignore]
        public HashSet<LancamentoTag> TagsSelecionadas
        {
            get
            {
                return this._TagsSelecionadas;
            }
            set
            {
                Set(() => TagsSelecionadas, ref _TagsSelecionadas, value);
            }
        }

        public int Parcela
        {
            get
            {
                return this._Parcela;
            }
            set
            {
                Set(() => Parcela, ref _Parcela, value);
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
                Set(() => DataCriacao, ref _DataCriacao, value);
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
                Set(() => DataAlteracao, ref _DataAlteracao, value);
            }
        }

        public double ValorLancamento
        {
            get
            {
                return this._ValorLancamento;
            }
            set
            {
                Set(() => ValorLancamento, ref _ValorLancamento, value);
            }
        }

        public double ValorLancamentoRealizado
        {
            get
            {
                return this._ValorLancamentoRealizado;
            }
            set
            {
                Set(() => ValorLancamentoRealizado, ref _ValorLancamentoRealizado, value);
            }
        }

        public System.DateTime? DataLancamento
        {
            get
            {
                return this._DataLancamento;
            }
            set
            {
                Set(() => DataLancamento, ref _DataLancamento, value);
            }
        }

        public string Descricao
        {
            get
            {
                return this._Descricao;
            }
            set
            {
                Set(() => Descricao, ref _Descricao, value);
            }
        }

        public int ContaId
        {
            get
            {
                return this._ContaId;
            }
            set
            {
                Set(() => ContaId, ref _ContaId, value);
            }
        }

        public int TipoRepeticao
        {
            get
            {
                return this._TipoRepeticao;
            }
            set
            {
                Set(() => TipoRepeticao, ref _TipoRepeticao, value);
            }
        }

        public int TipoFimRepeticao
        {
            get
            {
                return this._TipoFimRepeticao;
            }
            set
            {
                Set(() => TipoFimRepeticao, ref _TipoFimRepeticao, value);
            }
        }

        public DateTime? FimRepeticaoData
        {
            get
            {
                return this._FimRepeticaoData;
            }
            set
            {
                Set(() => FimRepeticaoData, ref _FimRepeticaoData, value);
            }
        }

        public int? FimRepeticaoQuantidade
        {
            get
            {
                return this._FimRepeticaoQuantidade;
            }
            set
            {
               Set(() => FimRepeticaoQuantidade, ref _FimRepeticaoQuantidade, value);
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
                Set(() => Tipo, ref _Tipo, value);
            }
        }

        public System.Nullable<int> LancamentoPaiId
        {
            get
            {
                return this._LancamentoPaiId;
            }
            set
            {
                Set(() => LancamentoPaiId, ref _LancamentoPaiId, value);
            }
        }

        public bool Compartilhar
        {
            get
            {
                return this._Compartilhar;
            }
            set
            {
               Set(() => Compartilhar, ref _Compartilhar, value);
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
                Set(() => Sincronizado, ref _Sincronizado, value);
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
               Set(() => Fechado, ref _Fechado, value);

                if(value && _ValorLancamentoRealizado == 0)
                {
                    ValorLancamentoRealizado = ValorLancamento;
                }
                else if(!value)
                {
                    ValorLancamentoRealizado = 0;
                }
            }
        }

        public bool ReceberAviso
        {
            get
            {
                return this._ReceberAviso;
            }
            set
            {
               Set(() => ReceberAviso, ref _ReceberAviso, value);
            }
        }

        public int? FormaPagamentoId
        {
            get
            {
                return this._FormaPagamentoId;
            }
            set
            {
                Set(() => FormaPagamentoId, ref _FormaPagamentoId, value);
            }
        }

        public string NumeroDocumento
        {
            get
            {
                return this._NumeroDocumento;
            }
            set
            {
                Set(() => NumeroDocumento, ref _NumeroDocumento, value);
            }
        }             
    }
}
