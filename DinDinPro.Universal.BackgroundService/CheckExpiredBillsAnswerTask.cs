using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace DinDinPro.Universal.BackgroundService
{
    public sealed class CheckExpiredBillsAnswerTask : IBackgroundTask
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly IContaRepository _contaRepositorio;
        public CheckExpiredBillsAnswerTask()
        {
            _resourceLoader = new ResourceLoader();
            _contaRepositorio = new ContaRepository(new DataService());
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background TASK - CheckExpiredBillsAnswerTask");
            //arguments and user inputs;
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            var arguments = details.Argument;

            if(!String.IsNullOrWhiteSpace(arguments) && arguments != _resourceLoader.GetString("No"))
            {
                var lancamentoId = int.Parse(arguments);
                var result = _contaRepositorio.MarcarDespesaEfetivada(lancamentoId).Result;
            }

        }
    }
}
