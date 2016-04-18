using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class TagsCriarPageViewModel : BaseViewModel
    {
        private readonly ITagRepository _tagRepository;
        private readonly INavigationService _navigationService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;

        public TagsCriarPageViewModel(INavigationService navigationService,
            ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            ITagRepository tagRepository)
        {
            _navigationService = navigationService;
            _tagRepository = tagRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            PageLoad = new RelayCommand(PageLoadExecute);
            SalvarTag = new RelayCommand<object>(SalvarTagExecute, (o) => { return !IsBusy; });
            RemoverTag = new RelayCommand(RemoverTagExecute, () => { return !IsBusy; });
            _TagSelecionada = new Tag() { Tipo = "-" };
        }


        #region Commands

        #region Métodos
        private void PageLoadExecute()
        {
            if (Parametro != null)
                TagSelecionada = Parametro as Tag;
        }
        private async void RemoverTagExecute()
        {
            if (TagSelecionada == null) return;

            if (!await _tagRepository.ExisteRelacionamento(TagSelecionada.TagId))
            {
                if (await _tagRepository.RemoverTagAsync(TagSelecionada))
                {
                    ((AppShell)Window.Current.Content).AppFrame.GoBack();
                }
            }
            else
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("TagsPageMsgExisteRelacionamento"),
                    _resourceLoader.GetString("ApplicationTitle"));
            }
        }

        private async void SalvarTagExecute(object obj)
        {
            if (String.IsNullOrWhiteSpace(TagSelecionada.NomeTag))
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("TagPageFlyoutMsgErroNome"),
                    _resourceLoader.GetString("ApplicationTitle"));
                return;
            }

            if (String.IsNullOrWhiteSpace(TagSelecionada.Tipo))
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("TagPageFlyoutMsgTipo"),
                    _resourceLoader.GetString("ApplicationTitle"));
                return;
            }

            await _tagRepository.SalvarTagAsync(TagSelecionada);

            //_navigationService.GoBack();
            ((AppShell)Window.Current.Content).AppFrame.GoBack();
        }
        #endregion Métodos

        #region Propriedades
        public RelayCommand PageLoad { get; private set; }
        public RelayCommand<object> SalvarTag { get; private set; }
        public RelayCommand RemoverTag { get; private set; }
        #endregion Propriedades

        #endregion Commands

        #region ViewModel

        private Tag _TagSelecionada;

        public Tag TagSelecionada
        {
            get { return _TagSelecionada; }
            set
            {
                Set(() => TagSelecionada, ref _TagSelecionada, value);
            }
        }

        #endregion ViewModel
    }
}
