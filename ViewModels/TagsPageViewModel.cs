using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class TagsPageViewModel : BaseViewModel
    {

        private readonly ITagRepository _tagRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public TagsPageViewModel(INavigationService navigationService, ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            ITagRepository tagRepository)
        {
            _navigationService = navigationService;
            _tagRepository = tagRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            PageLoad = new RelayCommand(PageLoadExecute);
            CriarTag = new RelayCommand(CriarTagExecute, () => { return !IsBusy; });            
            SelecionarTag = new RelayCommand<object>(SelecionarTagExecute, (o) => { return !IsBusy; });

            if(ViewModelBase.IsInDesignModeStatic)
            {
                Lista = new ObservableCollection<Tag>(_tagRepository.ListarTagsAsync().Result);
            }
            
        }


        //public async override void OnNavigatedTo(object navigationParameter, Windows.UI.Xaml.Navigation.NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        //{
        //    Lista = new ObservableCollection<Tag>(await _tagRepository.ListarTagsAsync());
        //}

        #region Commands

        #region Métodos
        private async void PageLoadExecute()
        {
            Lista = new ObservableCollection<Tag>(await _tagRepository.ListarTagsAsync());
        }

        private void SelecionarTagExecute(object arg)
        {
            //_navigationService.NavigateTo("TagsCriar", arg);            
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.TagsCriarPage), arg);
        }  

        private void CriarTagExecute()
        {
            //_navigationService.NavigateTo("TagsCriar", null);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.TagsCriarPage), null);
        }


        #endregion

        public RelayCommand PageLoad { get; private set; }
        #region Propriedades
        
        public RelayCommand CriarTag { get; private set; }
        public RelayCommand<object> SelecionarTag { get; private set; }
        #endregion

        #endregion Commands

        #region Propriedades

        private ObservableCollection<Tag> _Lista = new ObservableCollection<Tag>();

        public ObservableCollection<Tag> Lista
        {
            get { return _Lista; }
            set
            {
                Set(() => Lista, ref _Lista, value);
            }
        }

        

        #endregion
    }
}
