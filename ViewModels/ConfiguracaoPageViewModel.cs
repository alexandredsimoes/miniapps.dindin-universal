using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using DinDinPro.Universal.Services;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using DinDinPro.Universal.Logger;
using System.Diagnostics;
using System.Globalization;
using GalaSoft.MvvmLight.Views;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight.Command;
using Windows.Storage.Pickers;
using System.IO;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using DinDinPro.Universal.Aggregators;
using Windows.ApplicationModel.Email;
using DinDinPro.Universal.Views;
using Windows.ApplicationModel.Store;
using Windows.UI.Popups;

namespace DinDinPro.Universal.ViewModels
{
    public class ConfiguracaoPageViewModel : BaseViewModel
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IContaRepository _contaRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly INavigationService _navigationService;
        private readonly IOneDriveService _oneDriveService;
        private readonly ResourceLoader _resourceLoader;
        private readonly IAlertMessageService _alertMessageService;
        //private readonly IEventAggregator _eventAggregator;

        private readonly IPinInputPageViewModel _pinInputViewModel;

        public ConfiguracaoPageViewModel(INavigationService navigationService, IBackupRepository backupRepository,
            IOneDriveService oneDriveService, ResourceLoader resourceLoader, IAlertMessageService alertMessageService,
            IContaRepository contaRepository,
            IPinInputPageViewModel pinInputViewModel,
            IConfiguracaoRepository configuracaoRepository
            )
        {
            _backupRepository = backupRepository;
            _navigationService = navigationService;
            _oneDriveService = oneDriveService;
            _contaRepository = contaRepository;
            _pinInputViewModel = pinInputViewModel;
            _configuracaoRepository = configuracaoRepository;

            _resourceLoader = resourceLoader;
            _alertMessageService = alertMessageService;

            RestaurarBackup = new RelayCommand(RestaurarBackupExecute, () => { return !IsBusy; });
            EfetuarBackup = new RelayCommand(EfetuarBackupExecute, () => { return !IsBusy; });
            AlterarTags = new RelayCommand(AlterarTagsExecute, () => { return !IsBusy; });
            AlterarSenha = new RelayCommand(AlterarSenhaExecute, () => { return !IsBusy; });
            AlterarFormaPagamento = new RelayCommand(AlterarFormaPagamentoExecute, () => { return !IsBusy; });
            EnviarLog = new RelayCommand(EnviarLogExecute);
            RemoverAds = new RelayCommand(RemoverAdsExecute);
            About = new RelayCommand(() =>
            {
                ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(SobrePage), null);
            });
        }

        private async void RemoverAdsExecute()
        {
#if DEBUG
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
#endif

            if (!licenseInformation.ProductLicenses["DINDIN_UNIVERSAL_SEM_ADS"].IsActive)
            {
                IsBusy = true;                
                try
                {
                    await CurrentApp.RequestProductPurchaseAsync("DINDIN_UNIVERSAL_SEM_ADS");
                    if (licenseInformation.ProductLicenses["DINDIN_UNIVERSAL_SEM_ADS"].IsActive)
                    {
                        await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraEfetuada")).ShowAsync();
                        App.ExibirAds = false;
                        ApplicationData.Current.RoamingSettings.Values["DINDIN_UNIVERSAL_SEM_ADS"] = true;
                    }
                    else
                    {
                        await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraNaoEfetuada"), _resourceLoader.GetString("ApplicationTitle")).ShowAsync();                        
                        //Debug.WriteLine("this feature was not purchased.");
                    }
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                    //Debug.WriteLine("Unable to buy this feature.");
                }
                finally
                {
                    IsBusy = false;
                }
            }
            else
            {
                IsBusy = false;
                await new MessageDialog(_resourceLoader.GetString("ConfigurationPageCompraJaEfetuada"), _resourceLoader.GetString("ApplicationTitle")).ShowAsync();
                App.ExibirAds = false;
                ApplicationData.Current.RoamingSettings.Values["DINDIN_UNIVERSAL_SEM_ADS"] = true;
                //Debug.WriteLine("You already own this feature.");
            }
        }

        private async void EnviarLogExecute()
        {
            var logExists = true;
            try
            {
                var f = await ApplicationData.Current.LocalFolder.GetFileAsync("AppLogs.txt");
            }
            catch (FileNotFoundException fnf)
            {
                logExists = false;
            }
            if (!logExists)
            {
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("MainPageViewModelNotLogMessage"), _resourceLoader.GetString("ApplicationTitle"));
                return;
            }

            var dialogCommands = new List<DialogCommand>();

            dialogCommands.Add(new DialogCommand()
            {
                Label = _resourceLoader.GetString("Yes"),

                Invoked = async () =>
                {

                    var email = new Windows.ApplicationModel.Email.EmailMessage()
                    {
                        Body = _resourceLoader.GetString("MainPageViewModelSendLogBodyMessage"),
                        Subject = _resourceLoader.GetString("MainPageViewModelSendLogSubject"),
                    };

                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("AppLogs.txt");
                    email.Attachments.Add(new EmailAttachment("AppLogs.txt", file));
                    
                    email.To.Add(new EmailRecipient("alexandre.dias.simoes@outlook.com", "Alexandre Dias Simões"));
                    await EmailManager.ShowComposeNewEmailAsync(email);
                }
            });

            dialogCommands.Add(new DialogCommand()
            {
                Label = _resourceLoader.GetString("No"),
                Invoked = () => { /*Nada faz*/ }
            });

            await _alertMessageService.ShowAsync(_resourceLoader.GetString("MainPageViewModelMsgEmailLog"), _resourceLoader.GetString("ApplicationTitle"),
               dialogCommands);
        }

        private void AlterarSenhaExecute()
        {
            Messenger.Default.Register<PinInputEvent>(this, ObterSenha);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.PinInputPage), null);
        }

        private async void ObterSenha(PinInputEvent obj)
        {
            IsBusy = true;
            await _configuracaoRepository.SalvarConfiguracao("senha", obj.Valor.ToString());
            await _alertMessageService.ShowAsync(_resourceLoader.GetString("MsgConfirmaAlteracaoSenha"), _resourceLoader.GetString("ApplicationTitle"));
            IsBusy = false;
            ((AppShell)Window.Current.Content).AppFrame.GoBack();
        }

        private void AlterarFormaPagamentoExecute()
        {
            //_navigationService.NavigateTo("FormaPagamento", null);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.FormaPagamentoPage), null);
        }

        private void AlterarTagsExecute()
        {
            //_navigationService.NavigateTo("Tags", null);
            ((AppShell)Window.Current.Content).AppFrame.Navigate(typeof(Views.TagsPage), null);
        }

        private void DefinirSenhaExecute()
        {
            throw new NotImplementedException();
        }

        private async void EfetuarBackupExecute()
        {
            //await Task.Delay(1000);
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("ConfiguracaoPageMsgEfetuarBackupSim"),
                Invoked = async () =>
                {
                    await IniciarBackup();
                }
            });
            commands.Add(new DialogCommand()
            {
                Id = 2,
                Label = _resourceLoader.GetString("ConfiguracaoPageMsgEfetuarBackupNao"),
                Invoked = () => { } //Nada faz
            });

            await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgEfetuarBackup"),
                _resourceLoader.GetString("ApplicationTitle"), commands);
        }

        private async void RestaurarBackupExecute()
        {
            var commands = new List<DialogCommand>();
            commands.Add(new DialogCommand()
            {
                Id = 1,
                Label = _resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackupSim"),
                Invoked = async () =>
                {
                    await ExecutarRestauracao();
                }
            });
            commands.Add(new DialogCommand()
            {
                Id = 2,
                Label = _resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackupNao"),
                Invoked = () => { } //Nada faz
            });

            await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackup"),
                _resourceLoader.GetString("ApplicationTitle"), commands);
            //await _alertMessageService.ShowContentAsync(_resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackup"));
        }

        private async System.Threading.Tasks.Task ExecutarRestauracao()
        {
            //System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            //System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("pt-BR");

            Func<string, DateTime> tentarConverterData = (dataString) =>
                    {
                        DateTime retorno;
                        string format =  "MM/dd/yyyy";
                        if (DateTime.TryParse(dataString, out retorno))
                        {
                            return retorno;
                        }
                        return DateTime.MinValue;
                    };

            Func<string, short?> tentarConverterShort = dataString =>
            {
                short retorno;

                if (short.TryParse(dataString, out retorno))
                {
                    return retorno;
                }
                return null;
            };

            Func<string, decimal> tentarConverterDecimal = dataString =>
            {
                decimal retorno = 0;

                if (decimal.TryParse(dataString, out retorno))
                {
                    return retorno;
                }
                return retorno;
            };

            Func<string, double> tentarConverterDouble = dataString =>
            {
                double retorno = 0;

                if (double.TryParse(dataString, out retorno))
                {
                    return retorno;
                }
                return retorno;
            };


            var msg = String.Empty;
            var isLogon = false;
            var erro = false;
            try
            {
                IsBusy = true;
                Message = _resourceLoader.GetString("ConfiguracaoPageMsgEfetuarBackupMsgProcessandoRestauracao");

                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                //openPicker.SuggestedStartLocation = PickerLocationId.Unspecified;
                openPicker.FileTypeFilter.Add(".xls");
                openPicker.FileTypeFilter.Add(".xlsx");

                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    // isLogon = file.IsLogon;
                    //Antes de processar o arquivo de backup, apaga tudo
                    if (!(await _backupRepository.LimparBaseDados()))
                    {
                        IsBusy = false;
                        return;
                    }

                    //Processa o arquivo de backup
                    var listaTags = new List<Tag>();
                    var listaContas = new List<Conta>();
                    var listaLancamento = new List<Lancamento>();
                    var listaLancamentoTag = new List<LancamentoTag>();
                    try
                    {
                        //Processa o arquivo
                        ExcelEngine excelEngine = new ExcelEngine();
                        excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;


                        //StorageFile excelFile = (file.File);//await ApplicationData.Current.LocalFolder.GetFileAsync("Backup\\" + file.Name);
                        //StorageFile excelFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Backup\\dindin-backup.xlsx");



                        //var r = await file.OpenStreamForReadAsync();
                        IWorkbook workbook = await excelEngine.Excel.Workbooks.OpenAsync(file);
                        workbook.Date1904 = false;
                        workbook.DetectDateTimeInValue = false;

                        //Primeira planilha pega as categorias
                        IWorksheet sheet = workbook.Worksheets[0];

                        int lastRow = sheet.UsedRange.LastRow;
                        for (int i = 1; i <= lastRow; i++)
                        {
                            listaTags.Add(
                            new Tag()
                            {
                                DataCriacao = DateTime.Now,
                                NomeTag = sheet.Range[i, 1].Value,
                                Tipo = sheet.Range[i, 2].Value,
                                Sincronizado = sheet.Range[i, 3].Value == "1" ? true : false
                            });
                        }


                        //Segunda planilha, pega as contas
                        sheet = workbook.Worksheets[1];
                        lastRow = sheet.UsedRange.LastRow;
                        for (int i = 1; i <= lastRow; i++)
                        {
                            listaContas.Add(
                            new Conta()
                            {
                                DataCriacao = DateTime.Now,
                                NomeConta = sheet[i, 2].Value,
                                TipoPeriodo = sheet[i, 3].Value,
                                PeriodoExibicaoInicio = tentarConverterData(sheet[i, 4].Text),
                                PeriodoExibicaoFinal = tentarConverterData(sheet[i, 5].Text),
                                Sincronizado = sheet[1, 6].Value == "1" ? true : false,
                                ValorInicial = tentarConverterDouble(sheet[1, 7].Value)
                            });
                        }



                        Func<string, int> tentarConverterTipoRepeticao = tipo =>
                        {

                            switch (tipo)
                            {
                                case "N":
                                    return 0;
                                case "D":
                                    return 1;
                                case "S":
                                    return 2;
                                case "Q":
                                    return 3;
                                case "M":
                                    return 4;
                                case "A":
                                    return 5;
                                case "U":
                                    return 6;
                                case "F":
                                    return 7;
                            }
                            return 0;
                        };

                        Func<string, int> tentarConverterTipoFimRepeticao = tipo =>
                        {

                            switch (tipo)
                            {
                                case "N":
                                    return 0;
                                case "D":
                                    return 1;
                                case "X":
                                    return 2;
                            }
                            return 0;
                        };

                        //Cria as contas e tags
                        await _backupRepository.CriarContas(listaContas);
                        await _backupRepository.CriarTags(listaTags);


                        //Terceira planilha, pega os lancamentos
                        sheet = workbook.Worksheets[2];
                        lastRow = sheet.UsedRange.LastRow;


                        //var val = sheet.GetValueRowCol(1, 14);
#if DEBUG
                        Debug.WriteLine("Reading Sheet3 (workbook.Worksheets[2])");
#endif
                        for (int i = 1; i <= lastRow; i++)
                        {
                            var conta = await _backupRepository.LocalizarContaPorNome(sheet[i, 2].Value);

                            if (conta == null) continue;

#if DEBUG
                            Debug.WriteLine("Linha " + i);
                            Debug.WriteLine("sheet[1,1].Value = " + sheet[i, 1].Value);
                            Debug.WriteLine("sheet[1,2].Value = " + sheet[i, 2].Value);
                            Debug.WriteLine("sheet[1,3].Value = " + sheet[i, 3].Value);
                            Debug.WriteLine("sheet[1,4].Value = " + sheet[i, 4].Value);
                            Debug.WriteLine("sheet[1,5].Value = " + sheet[i, 5].Value);
                            Debug.WriteLine("sheet[1,6].Value = " + sheet[i, 6].Value);
                            Debug.WriteLine("sheet[1,7].Value = " + sheet[i, 7].Value);
                            Debug.WriteLine("sheet[1,8].Value = " + sheet[i, 8].Value);
                            Debug.WriteLine("sheet[1,9].Value = " + sheet[i, 9].Value);
                            Debug.WriteLine("sheet[1,10].Value = " + sheet[i, 10].Value);
                            Debug.WriteLine("sheet[1,11].Value = " + sheet[i, 11].Value);
                            Debug.WriteLine("sheet[1,12].Value = " + sheet[i, 12].Value);
                            Debug.WriteLine("sheet[1,13].Value = " + sheet[i, 13].Value);
                            Debug.WriteLine("sheet[1,14].Value = " + sheet[i, 14].Value);

                            Debug.WriteLine("\r\n");


                            //Debug.WriteLine("sheet[1,14].Value = " + tentarConverterData(sheet[1, 14].Value));
#endif
                            var lancamento = await _backupRepository.CriarLancamento(new Lancamento()
                            {
                                DataCriacao = DateTime.Now,
                                ContaId = conta.ContaId,
                                Tipo = sheet[i, 3].Value,
                                Sincronizado = sheet[i, 5].Value == "1" ? true : false,
                                TipoFimRepeticao = tentarConverterTipoFimRepeticao(sheet[i, 7].Text), //Never,specific date,after x retries
                                TipoRepeticao = tentarConverterTipoRepeticao(sheet[i, 8].Text), //Never repeat,Daily,Weekly,Fortnightly,Monthly,Annual,Weekdays,Weekends
                                ValorLancamento = Double.Parse(sheet[i, 9].Value),
                                Compartilhar = sheet[i, 10].Value == "1" ? true : false,
                                Descricao = sheet[i, 11].Value,
                                FimRepeticaoData = tentarConverterData(sheet[i, 12].Text),
                                FimRepeticaoQuantidade = tentarConverterShort(sheet[i, 13].Value),
                                DataLancamento = tentarConverterData(sheet[i, 14].Text),
                                Fechado = sheet[i, 15].Value == "1" ? true : sheet[i, 15].Value == "0" ? false : true,
                                ReceberAviso = sheet[i, 16].Value == "1" ? true : false,
                                ValorLancamentoRealizado = string.IsNullOrWhiteSpace(sheet[i, 17].Value) ? double.Parse(sheet[i, 9].Value) : double.Parse(sheet[i, 17].Value) //Implementado a partir da versão 4 da base de dados
                            });

                            if (lancamento.LancamentoId > 0)
                            {
                                //Processa as tags do lançamento em questão
                                foreach (var tag in sheet[i, 4].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    //Tenta localizar a tag pelo nome
                                    var tagLocalizada = await _backupRepository.LocalizarTagPorNome(tag);
                                    if (tagLocalizada != null)
                                    {
                                        await _backupRepository.CriarLancamentoTag(new LancamentoTag()
                                        {
                                            DataCriacao = DateTime.Now,
                                            LancamentoId = lancamento.LancamentoId,
                                            TagId = tagLocalizada.TagId,
                                            Sincronizado = false
                                        });

                                    }
                                    else
                                    {
                                        //lancamento.LancamentoTags.Add();
                                    }
                                }
                            }
                        }

                        workbook.Close();
                        excelEngine.Dispose();
                        IsBusy = false;

                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                        AppLogs.WriteError("ConfiguracaoPageViewModel", ex);
                        erro = true;
                    }
                }
                else
                {
                    erro = true;
                }
            }
            finally
            {
                IsBusy = false;
            }

            if (erro)
            {
                try
                {

                    IsBusy = true;
                    //Se temos algum erro, limpa toda bagunça                
                    if (isLogon)
                        await _backupRepository.LimparBaseDados();
                }
                finally
                {
                    IsBusy = false;
                }


                await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackupErro"),
                            _resourceLoader.GetString("ApplicationTitle"));
            }
            else
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgRestaurarBackupSucesso"),
                            _resourceLoader.GetString("ApplicationTitle"));
        }


        private async System.Threading.Tasks.Task IniciarBackup()
        {
            var erro = false;
            try
            {
                IsBusy = true;
                Message = _resourceLoader.GetString("ConfiguracaoPageMsgEfetuarBackupMsgProcessando");
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;
                    IApplication application = excelEngine.Excel;

                    IWorkbook workbook = application.Workbooks.Create(3);

                    //Primeira planilha guarda as categorias
                    IWorksheet sheet = workbook.Worksheets[0];

                    int row = 1;


                    foreach (var item in await _backupRepository.ListarTags())
                    {
                        sheet[row, 1].Value = item.NomeTag;
                        sheet[row, 2].Value = item.Tipo;
                        sheet[row, 3].Value2 = item.Sincronizado ? 1 : 0;
                        row++;
                    }

                    //Segunda planilha guarda as contas
                    sheet = workbook.Worksheets[1];
                    row = 1;
                    foreach (var item in await _backupRepository.ListarContas())
                    {
                        sheet[row, 1].Value2 = item.ContaId;
                        sheet[row, 2].Value = item.NomeConta;
                        sheet[row, 3].Value = item.TipoPeriodo;
                        sheet[row, 4].Value2 = item.PeriodoExibicaoInicio;
                        sheet[row, 5].Value2 = item.PeriodoExibicaoFinal;
                        sheet[row, 6].Value2 = item.Sincronizado ? 1 : 0;
                        sheet[row, 7].Value2 = item.ValorInicial;
                        row++;
                    }

                    //Terceira planilha guarda os lançamentos
                    sheet = workbook.Worksheets[2];
                    row = 1;
                    foreach (var item in await _contaRepository.ObterLancamentos(null, null, null))
                    {
                        sheet[row, 1].Value2 = item.LancamentoId;
                        sheet[row, 2].Value2 = item.NomeConta;
                        sheet[row, 3].Value = item.Tipo;
                        sheet[row, 4].Value = item.Tags;
                        sheet[row, 5].Value2 = item.Sincronizado ? 1 : 0;
                        sheet[row, 6].Value2 = item.LancamentoPaiId;
                        sheet[row, 7].Value2 = item.TipoFimRepeticao;
                        sheet[row, 8].Value2 = item.TipoRepeticao;

                        sheet[row, 9].Value2 = item.ValorLancamento;
                        sheet[row, 10].Value2 = item.Compartilhar ? 1 : 0;
                        sheet[row, 11].Value2 = item.Descricao;
                        sheet[row, 12].Text = item.FimRepeticaoData == null ? String.Empty : item.FimRepeticaoData.Value.ToString("dd/MM/yyyy");
                        sheet[row, 13].Value2 = item.FimRepeticaoQuantidade;
                        sheet[row, 14].Text = item.DataLancamento.Value.ToString("dd/MM/yyyy");
                        sheet[row, 15].Value2 = item.Fechado ? 1 : 0;
                        sheet[row, 16].Value2 = item.ReceberAviso ? 1 : 0;
                        sheet[row, 17].Value2 = item.ValorLancamentoRealizado;

                        row++;
                    }

                    FileSavePicker savePicker = new FileSavePicker();

                    savePicker.FileTypeChoices.Add("Backup", new[] { ".xlsx", ".xls" });
                    savePicker.CommitButtonText = "Salvar backup";
                    savePicker.SuggestedFileName = "dindin-backup.xls";
                    var r = await savePicker.PickSaveFileAsync();
                    await workbook.SaveAsAsync(r);

                    workbook.Close();
                    excelEngine.Dispose();
                }
            }
            finally
            {
                IsBusy = false;
            }


            if (erro)
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgBackupErro"),
                            _resourceLoader.GetString("ApplicationTitle"));
            else
                await _alertMessageService.ShowAsync(_resourceLoader.GetString("ConfiguracaoPageMsgBackupSucesso"),
                            _resourceLoader.GetString("ApplicationTitle"));
        }

#region Propriedades

#region Commands
        public RelayCommand RestaurarBackup { get; private set; }
        public RelayCommand EfetuarBackup { get; private set; }
        public RelayCommand AlterarTags { get; private set; }
        public RelayCommand AlterarFormaPagamento { get; private set; }
        public RelayCommand AlterarSenha { get; private set; }
        public RelayCommand EnviarLog { get; private set; }
        public RelayCommand About { get; private set; }
        public RelayCommand RemoverAds { get; private set; }


#endregion Commands


#region ViewModel

        public IPinInputPageViewModel PinInputControlViewModel
        {
            get
            {
                return _pinInputViewModel;
            }
        }


#endregion ViewModel

#endregion Propriedades
    }
}
