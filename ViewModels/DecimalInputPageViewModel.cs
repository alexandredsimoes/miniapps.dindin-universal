using System.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Messaging;
using DinDinPro.Universal.Aggregators;
using GalaSoft.MvvmLight.Command;

namespace DinDinPro.Universal.ViewModels
{
    public class DecimalInputPageViewModel : BaseViewModel, IDecimalInputUserControlViewModel
    {
        private INavigationService _navigationService;
       
        string _separadorDecimal = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;


        public DecimalInputPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            if (!ModoSenha)
                Display = (0M).ToString("n2");
            else
                Display = "";

            //Assina o evento para "escutar" qualquer alteração
            Messenger.Default.Register<DecimalnputEventRefresh>(this, ProcessaValor);
            //_eventAggregator.GetEvent<DecimalnputEventRefresh>().Subscribe(ProcessaValor);

            
            DigitButtonPress = new RelayCommand<object>(DigitButtonPressExecute);

            OKButtonPress = new RelayCommand<object>(OKButtonPressExecute);
            CancelButtonPress = new RelayCommand<object>(CancelButtonPressExecute);
            Confirmar = new RelayCommand<object>(ConfirmarExecute);
        }
       

        private void ConfirmarExecute(object arg)
        {
            //Avisa os assinantes que o valor foi alterado
            Messenger.Default.Send<DecimalInputEvent>(new DecimalInputEvent() { Valor = this.Valor });
            //_eventAggregator.GetEvent<DecimalInputEvent>().Publish(Valor);

            //Realmente não me orgulho disso, mas foi a única maneira possivel de fechar esse flyout
            ((Windows.UI.Xaml.Controls.Primitives.Popup)((Windows.UI.Xaml.Controls.FlyoutPresenter)((DinDinPro.Universal.UserControls.DecimalInputUserControl)arg).Parent).Parent).IsOpen = false;

            //_navigationService.GoBack();
        }

        private void CancelButtonPressExecute(object arg)
        {
            Display = ModoSenha ? "" : (0M).ToString("n2");
        }

        private void OKButtonPressExecute(object arg)
        {
            if (ModoSenha && String.IsNullOrWhiteSpace(Display))
            {
                return;// Task.FromResult<object>(null);
            }

            //Avisa os assinantes que o valor foi alterado
            Messenger.Default.Send<DecimalInputEvent>(new DecimalInputEvent() { Valor = double.Parse(Display) });
            //_eventAggregator.GetEvent<DecimalInputEvent>().Publish(decimal.Parse(Display));


            if (ModoSenha)
                Display = String.Empty;

            //Realmente não me orgulho disso, mas foi a única maneira possivel de fechar esse flyout
            try
            {
                ((Windows.UI.Xaml.Controls.Primitives.Popup)((Windows.UI.Xaml.Controls.FlyoutPresenter)((DinDinPro.Universal.UserControls.DecimalInputUserControl)arg).Parent).Parent).IsOpen = false;
            }
            catch (Exception)
            {            
            }
            

            //_navigationService.GoBack();
        }

        private void DigitButtonPressExecute(object parametro)
        {
            var valor = parametro as string;
            if (String.IsNullOrWhiteSpace(Display))
                if (!ModoSenha)
                    Display = (0M).ToString("n2");
                else
                    Display = "";


            int tamanho = Display.Length;
            var d = 0M;
            var saida = String.Empty;

            if (ModoSenha)
            {
                saida = Display;
            }
            else
            {
                d = decimal.Parse(Display);
                saida = d.ToString(modoSenha ? "n0" : "n2").Replace(".", "").Replace(",", "");
            }


            if (valor == "clear")
            {
                if (!String.IsNullOrWhiteSpace(saida))
                    saida = saida.Remove(saida.Length - 1, 1);

                if (!ModoSenha)
                    saida = saida.Insert(saida.Length - 2, _separadorDecimal);
            }
            else
            {
                saida = saida.Insert(saida.Length, valor);

                if (!ModoSenha)
                    saida = saida.Insert(saida.Length - 2, _separadorDecimal);
            }

            if (!ModoSenha)
                Display = decimal.Parse(saida).ToString("n2");
            else
            {
                Display = String.IsNullOrWhiteSpace(saida) ? String.Empty : saida;
            }
            //return Task.FromResult<object>(null);
        }


        private void ProcessaValor(DecimalnputEventRefresh obj)
        {
            if (!ModoSenha)
                Display = obj.Valor.ToString("n2");
            else
                Display = obj.Valor.ToString();

            //_eventAggregator.GetEvent<DecimalInputEvent>().Unsubscribe(ProcessaValor);
            
        }

        #region Propriedades

        /// <summary>
        /// Define quantas linhas na grade, o botão "1" irá ocupar. 
        /// Como no modo senha o botão .00 é ocultado, é necessário definir essa propriedade para 2 para que 
        /// ele ocupe o lugar do botão que foi ocultado.
        /// </summary>
        public string RowSpan
        {
            get
            {
                return ModoSenha ? "2" : "1";
            }
        }
        private string display;
        public string Display
        {
            get
            {
                return display;
            }
            set
            {
                Set(() => Display, ref display, value);
            }
        }

        private bool modoSenha;

        public bool ModoSenha
        {
            get { return modoSenha; }
            set
            {
                Display = "";
                Set(() => ModoSenha, ref modoSenha, value);
            }
        }

        private double _Valor;

        public double Valor
        {
            get { return _Valor; }
            set { _Valor = value; }
        }


        #region ViewModel
        public RelayCommand<object> Confirmar
        { get; private set; }
        public RelayCommand<object> OKButtonPress
        {
            get;
            private set;
        }

        public RelayCommand<object> CancelButtonPress
        {
            get;
            private set;
        }

        #endregion

        public RelayCommand<object> DigitButtonPress
        {
            get;
            private set;
        }

        #endregion        
            
    }
}
