using DinDinPro.Universal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DinDinPro.Universal.Services
{
    public interface IOneDriveService
    {
        Task<OneDriveFileResult> EfetuarDownloadArquivo(string nomeArquivo = null);
        Task<OneDriveFileResult> EfetuarUploadArquivo(string arquivoLocal);
    }
}
