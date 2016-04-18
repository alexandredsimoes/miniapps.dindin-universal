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
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace DinDinPro.Universal.BackgroundService
{
    public sealed class CheckExpiredBillsTask : IBackgroundTask
    {
        private readonly ResourceLoader _resourceLoader;

        public CheckExpiredBillsTask()
        {
            _resourceLoader = new ResourceLoader();
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High)
            {
                Debug.WriteLine("Background TASK - CUSTO MUITO ALTO!");
            }
            else
            {
                taskInstance.Canceled += (sender, evt) =>
                {
                    Debug.WriteLine("Background TASK - CANCELADO!");
                };

                var deferral = taskInstance.GetDeferral();
                try
                {
                    int items = 0;
                    Debug.WriteLine("Background TASK - COMPLETO");
                    ContaRepository contaRepository = new ContaRepository(new DataService());
                    var lancamentos = contaRepository.ListarDespesasAtrasadas().Result;

                    foreach (var item in lancamentos)
                    {
                        ExibirToast(item);
                    }
                    //if (lancamentos.Count == 1)
                    //{
                    //    ExibirToast(lancamentos.FirstOrDefault());
                    //}
                    //foreach (var item in lancamentos)
                    //{
                    //    items++;
                    //    Debug.WriteLine("Background TASK - LANCAMENTO -> " + item.ValorLancamento);
                    //}




                    //Processa as contas a vencer
                    var lancamentosVencer = contaRepository.ListarDespesasVencendo().Result;
                    foreach (var item in lancamentosVencer)
                    {
                        Debug.WriteLine("Background TASK - LANCAMENTO A VENCER -> " + item.ValorLancamento);
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private void ExibirToast(LancamentoView lancamentoView)
        {
            if (lancamentoView == null) return;

            var titulo = _resourceLoader.GetString("ApplicationTitle");
            var valor = lancamentoView.ValorLancamento.ToString("c2");
            var descricao = _resourceLoader.GetString("ToastDespesaNaoPagaDescricao").Replace("{valor}", valor);
            descricao += Environment.NewLine + lancamentoView.Descricao;

            var toastContent = new StringBuilder();
            //toastContent.Append("");
            toastContent.Append("<toast>");
            toastContent.Append("<visual>");
            toastContent.Append("<binding template=\"ToastGeneric\">");
            toastContent.Append($"<text>{titulo}</text>");
            toastContent.AppendFormat("<text>{0}</text>", descricao);
            toastContent.Append("<image placement=\"AppLogoOverride\" src=\"oneAlarm.png\" />");
            toastContent.Append("</binding>");
            toastContent.Append("</visual>");
            toastContent.Append("<actions>");
            toastContent.AppendFormat("<action activationType=\"background\" content=\"{0}\" arguments=\"{1}\" imageUri=\"check.png\" />", _resourceLoader.GetString("Yes"), lancamentoView.LancamentoId);
            toastContent.AppendFormat("<action activationType=\"background\" content=\"{0}\" arguments=\"cancel\" />", _resourceLoader.GetString("No"));
            toastContent.Append("</actions>");
            toastContent.Append("<audio src=\"ms-winsoundevent:Notification.Reminder\"/>");
            toastContent.Append("</toast>");

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent.ToString());

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);


        }
    }
}
