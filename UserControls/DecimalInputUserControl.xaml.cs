using DinDinPro.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DinDinPro.Universal.UserControls
{
    public sealed partial class DecimalInputUserControl : UserControl
    {
        private readonly IDecimalInputUserControlViewModel _viewModel;
        public DecimalInputUserControl()
        {
            this.InitializeComponent();
            _viewModel = DataContext as IDecimalInputUserControlViewModel;
        }



        public bool ModoSenha
        {
            get { return (bool)GetValue(ModoSenhaProperty); }
            set
            {
                if (_viewModel != null) _viewModel.ModoSenha = value;
                SetValue(ModoSenhaProperty, value);

                if(value)
                {
                    //Oculta alguns controles que não se aplicam
                    //MyGrid.RowDefinitions[1].Height = new GridLength(1);
                }
            }
        }

        // Using a DependencyProperty as the backing store for ModoSenha.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModoSenhaProperty =
            DependencyProperty.Register("ModoSenha", typeof(bool), typeof(DecimalInputUserControl), new PropertyMetadata(0));

    }
}
