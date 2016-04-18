using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace DinDinPro.Universal.Models
{
    public class OneDriveFileResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocalFile { get; set; }
        public bool Success { get; set; }
        public string ExceptionInfo { get; set; }
        public bool IsLogon { get; set; }

        public StorageFile File { get; set; }

    }
}
