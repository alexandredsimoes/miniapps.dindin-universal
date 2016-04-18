using DinDinPro.Universal.Aggregators;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;

namespace DinDinPro.Universal.ViewModels
{
    public class PinInputPageViewModel : BaseViewModel, IPinInputPageViewModel
    {

        private Queue<int> _pilha = new Queue<int>();
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public PinInputPageViewModel(INavigationService navigationService, ResourceLoader resourceLoader,
            IAlertMessageService alertMessageService)
        {
            _navigationService = navigationService;
            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            PageUnload = new RelayCommand(() =>
            {
                Valor1 = null;
                Valor2 = null;
                Valor3 = null;
                Valor4 = null;
            });
            DigitButtonPress = new RelayCommand<object>(DigitButtonPressExecute, (o) => { return !IsBusy; });
        }

        //public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        //{
        //    //Valor1 = null;
        //    //Valor2 = null;
        //    //Valor3 = null;
        //    //Valor4 = null;
        //    base.OnNavigatedFrom(viewModelState, suspending);
        //}

        //public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        //{
        //    base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
        //}

        #region ViewModel

        #region Propriedades

        private int? _Valor1;

        public int? Valor1
        {
            get { return _Valor1; }
            set
            {
                Set(() => Valor1, ref _Valor1, value);
            }
        }

        private int? _Valor2;

        public int? Valor2
        {
            get { return _Valor2; }
            set
            {
                Set(() => Valor2, ref _Valor2, value);
            }
        }

        private int? _Valor3;

        public int? Valor3
        {
            get { return _Valor3; }
            set
            {
                Set(() => Valor3, ref _Valor3, value);
            }
        }

        private int? _Valor4;

        public int? Valor4
        {
            get { return _Valor4; }
            set
            {
                Set(() => Valor4, ref _Valor4, value);
            }
        }

        public RelayCommand<object> DigitButtonPress { get; private set; }
        public RelayCommand PageUnload { get; private set; }

        #endregion Propriedades

        #region Métodos
        private async void DigitButtonPressExecute(object arg)
        {

            var valor = int.Parse(arg.ToString());
            if (_pilha.Count >= 3)
            {
                IsBusy = true;
                Valor4 = valor;


                _pilha.Enqueue(valor);
                var result = String.Empty;
                foreach (var item in _pilha)
                {
                    result += item.ToString();
                }
                await Task.Delay(300);
                Messenger.Default.Send<PinInputEvent>(new PinInputEvent()
                {
                    Valor = int.Parse(result)
                });
                //_eventAggregator.GetEvent<PinInputEvent>().Publish(int.Parse(result));

                ////Reseta tudo

                _pilha.Clear();

                IsBusy = false;
                Valor1 = null;
                Valor2 = null;
                Valor3 = null;
                Valor4 = null;
                return; //Task.FromResult<object>(null);
            }


            _pilha.Enqueue(valor);

            if (_pilha.Count == 1) Valor1 = valor;
            else if (_pilha.Count == 2) Valor2 = valor;
            else if (_pilha.Count == 3) Valor3 = valor;
            else Valor4 = valor;

            IsBusy = false;
            //return Task.FromResult<object>(null);

        }

        #endregion Métodos

        #endregion ViewModel
    }
}
