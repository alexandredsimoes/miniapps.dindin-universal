using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Linq;
using DinDinPro.Universal.Models;

using Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;

namespace DinDinPro.Universal.Services
{
    public class OneDriveService : IOneDriveService
    {
        CancellationTokenSource cts;


        public async Task<OneDriveFileResult> EfetuarDownloadArquivo(string nomeArquivo = null)
        {
            var resourceLoader = new ResourceLoader();
            OneDriveFileResult result = new OneDriveFileResult();
            result.Success = false;
            result.ExceptionInfo = String.Empty;
            result.IsLogon = false;


            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.Unspecified;
                openPicker.FileTypeFilter.Add(".xls");

                StorageFile file = await openPicker.PickSingleFileAsync();


                if (file != null)
                {
                    result.File = file;
                    result.LocalFile = file.Path;
                    result.Success = true;

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ExceptionInfo = ex.Message;
                result.Success = false;
            }
            return result;
        }


        public async Task<OneDriveFileResult> EfetuarUploadArquivo(string arquivoLocal)
        {
            OneDriveFileResult result = new OneDriveFileResult();
            result.Success = false;

            FileSavePicker savePicker = new FileSavePicker();


            try
            {
                savePicker.FileTypeChoices.Add("Backup", new[] { ".xls"});
                savePicker.CommitButtonText = "Salvar backup";
                savePicker.SuggestedFileName = "dindin-backup.xls";
                var r = await savePicker.PickSaveFileAsync();


                if (r != null)
                {
                    result.LocalFile = r.Path;
                    result.Success = true;

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ExceptionInfo = ex.Message;
                result.Success = false;
            }
            return result;
        }
    }
}
