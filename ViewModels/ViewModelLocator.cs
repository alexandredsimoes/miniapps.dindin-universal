using DinDinPro.Universal.DesignViewModels;
using DinDinPro.Universal.Model.Design;
using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Models.Repositories.Design;
using DinDinPro.Universal.Services;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace DinDinPro.Universal.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var navigationService = new NavigationService();
            navigationService.Configure(Constantes.PAGINA_PRINCIPAL, typeof(Views.MainPage));
            navigationService.Configure(Constantes.PAGINA_CONTA, typeof(Views.ContaPage));
            navigationService.Configure(Constantes.PAGINA_CONTA_DETALHE, typeof(Views.ContaDetalhePage));

            navigationService.Configure(Constantes.PAGINA_CONFIGURACAO, typeof(Views.ConfiguracaoPage));
            navigationService.Configure(Constantes.PAGINA_LANCAMENTO, typeof(Views.CriarLancamentoPage));
            navigationService.Configure(Constantes.PAGINA_FORMA_PAGAMENTO, typeof(Views.FormaPagamentoPage));
            navigationService.Configure(Constantes.PAGINA_FORMA_PAGAMENTO_CRIAR, typeof(Views.FormaPagamentoCriarPage));
            navigationService.Configure(Constantes.PAGINA_LANCAMENTO_DETALHE, typeof(Views.LancamentoDetalhePage));
            navigationService.Configure(Constantes.PAGINA_LOGIN, typeof(Views.LoginPage));
            navigationService.Configure(Constantes.PAGINA_TAGS, typeof(Views.TagsPage));
            navigationService.Configure(Constantes.PAGINA_TAGS_CRIAR, typeof(Views.TagsCriarPage));
            navigationService.Configure(Constantes.PAGINA_PIN_INPUT, typeof(Views.PinInputPage));
            navigationService.Configure(Constantes.PAGINA_RELATORIO, typeof(Views.RelatorioPage));
            navigationService.Configure(Constantes.PAGINA_CONTAS, typeof(Views.ContaListPage));
            navigationService.Configure(Constantes.PAGINA_SOBRE, typeof(Views.SobrePage));


            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            if (!GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register<IBackupRepository, BackupRepository>();

                SimpleIoc.Default.Register<IOneDriveService, OneDriveService>();
                SimpleIoc.Default.Register<IContaRepository, ContaRepository>();
                SimpleIoc.Default.Register<IFormaPagamentoRepository, FormaPagamentoRepository>();
                SimpleIoc.Default.Register<IPinInputPageViewModel, PinInputPageViewModel>();
                SimpleIoc.Default.Register<IDecimalInputUserControlViewModel, DecimalInputPageViewModel>();
                SimpleIoc.Default.Register<IConfiguracaoRepository, ConfiguracaoRepository>();
                SimpleIoc.Default.Register<ITagRepository, TagRepository>();

            }
            else
            {
                SimpleIoc.Default.Register<ITagRepository, TagDesignRepository>();
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }

            SimpleIoc.Default.Register(() => new ResourceLoader(), true);
            SimpleIoc.Default.Register<IAlertMessageService, AlertMessageService>();

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<ContaPageViewModel>();
            SimpleIoc.Default.Register<ContaDetalhePageViewModel>();
            SimpleIoc.Default.Register<ConfiguracaoPageViewModel>();
            SimpleIoc.Default.Register<CriarLancamentoPageViewModel>();
            SimpleIoc.Default.Register<FormaPagamentoCriarPageViewModel>();
            SimpleIoc.Default.Register<FormaPagamentoPageViewModel>();
            SimpleIoc.Default.Register<LancamentoDetalhePageViewModel>();
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<TagsCriarPageViewModel>();
            SimpleIoc.Default.Register<TagsPageViewModel>();
            SimpleIoc.Default.Register<RelatorioPageViewModel>();
            SimpleIoc.Default.Register<ContaListPageViewModel>();
            SimpleIoc.Default.Register<AppShellViewModel>(true);

        }


        public AppShellViewModel Shell
        {
            get { return SimpleIoc.Default.GetInstance<AppShellViewModel>(); }
        }
        public IPinInputPageViewModel PinInput
        {
            get { return SimpleIoc.Default.GetInstance<IPinInputPageViewModel>(); }
        }

        public ContaListPageViewModel ContaList
        {
            get { return SimpleIoc.Default.GetInstance<ContaListPageViewModel>(); }
        }

        public RelatorioPageViewModel Relatorio
        {
            get { return SimpleIoc.Default.GetInstance<RelatorioPageViewModel>(); }
        }

        public LancamentoDetalhePageViewModel LancamentoDetalhe
        {
            get { return SimpleIoc.Default.GetInstance<LancamentoDetalhePageViewModel>(); }
        }

        public LoginPageViewModel Login
        {
            get { return SimpleIoc.Default.GetInstance<LoginPageViewModel>(); }
        }

        public TagsCriarPageViewModel TagsCriar
        {
            get { return SimpleIoc.Default.GetInstance<TagsCriarPageViewModel>(); }
        }

        public TagsPageViewModel Tags
        {
            get { return SimpleIoc.Default.GetInstance<TagsPageViewModel>(); }
        }

        public ConfiguracaoPageViewModel Configuracao
        {
            get { return SimpleIoc.Default.GetInstance<ConfiguracaoPageViewModel>(); }
        }

        public CriarLancamentoPageViewModel CriarLancamento
        {
            get { return SimpleIoc.Default.GetInstance<CriarLancamentoPageViewModel>(); }
        }

        public FormaPagamentoCriarPageViewModel FormaPagamentoCriar
        {
            get { return SimpleIoc.Default.GetInstance<FormaPagamentoCriarPageViewModel>(); }
        }

        public FormaPagamentoPageViewModel FormaPagamento
        {
            get { return SimpleIoc.Default.GetInstance<FormaPagamentoPageViewModel>(); }
        }

        public ContaDetalhePageViewModel ContaDetalhe
        {
            get { return SimpleIoc.Default.GetInstance<ContaDetalhePageViewModel>(); }
        }

        public MainPageViewModel Main
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainPageViewModel>();
            }
        }

        public ContaPageViewModel Conta
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ContaPageViewModel>();
            }
        }
    }
}
