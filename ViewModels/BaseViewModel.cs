using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinDinPro.Universal.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        public BaseViewModel()
        {
            IsBusy = false;
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {                
                Set(() => IsBusy, ref _IsBusy, value);
            }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set
            {
                Set(() => Message, ref _Message, value);
            }
        }

        private object _Parametro;

        public object Parametro
        {
            get { return _Parametro; }
            set
            {
                Set(() => Message, ref _Parametro, value);
            }
        }

        private object _ModoNavegacao;

        public object ModoNavegacao
        {
            get { return _ModoNavegacao; }
            set
            {
                Set(() => ModoNavegacao, ref _ModoNavegacao, value);
            }
        }
    }
}
