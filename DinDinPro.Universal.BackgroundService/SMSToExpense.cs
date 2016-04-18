using DinDinPro.Universal.Logger;
using DinDinPro.Universal.Models;
using DinDinPro.Universal.Models.Repositories;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel.Background;
using Windows.Devices.Sms;
using Windows.UI.Notifications;

namespace DinDinPro.Universal.BackgroundService
{
    class DadosDespesa
    {
        public double Valor;
        public string Origem;
        public DateTime Data;
        public string Tipo;
    }

    public sealed class SMSToExpense : IBackgroundTask
    {
        // 
        // The Run method is the entry point of a background task. 
        // 
        public void Run(IBackgroundTaskInstance taskInstance)
        {

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            try
            {
                Debug.WriteLine("Background " + taskInstance.Task.Name + ("process ran"));

                SmsMessageReceivedTriggerDetails smsDetails = taskInstance.TriggerDetails as SmsMessageReceivedTriggerDetails;

                // Just registered for text messages
                SmsTextMessage2 smsTextMessage;
                if (smsDetails.MessageType == SmsMessageType.Text)
                {
                    smsTextMessage = smsDetails.TextMessage;


                    IContaRepository repositorio = new ContaRepository(new DataService());

                    //Obtem a conta atual (primeira)
                    var conta = repositorio.ObterDetalhes().Result;

                    if (conta != null)
                    {
                        //Tenta obter os dados do SMS
                        var dadosSMS = ProcessarSMS(smsTextMessage.Body);

                        if (dadosSMS != null)
                        {
                            //Tenta localizar a tag atraves da origem]
                            //var tag = repositorio.ListarTagsAsync(dadosSMS.Value.Origem, dadosSMS.Value.Tipo).Result;

                            //Tenta efetuar um lançamento na conta 
                            var lancamento = new LancamentoView()
                            {
                                ContaId = conta.ContaId,
                                DataCriacao = DateTime.Now,
                                Fechado = true,
                                Descricao = dadosSMS.Origem,
                                DataLancamento = dadosSMS.Data,
                                Tipo = dadosSMS.Tipo,
                                ValorLancamento = dadosSMS.Valor,
                                ValorLancamentoRealizado = dadosSMS.Valor,
                                Tags = dadosSMS.Origem
                            };

                            var result = repositorio.CriarLancamentoAsync(lancamento).Result;

                            if (result)
                            {
                                var titulo = String.Format("{0} efetuada.", dadosSMS.Tipo == "+" ? "Receita" : "Despesa");
                                var mensagem = string.Format("{0} - {1:c2}", dadosSMS.Origem, dadosSMS.Valor);
                                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                                var stringElements = toastXml.GetElementsByTagName("text");

                                stringElements.Item(0).AppendChild(toastXml.CreateTextNode(titulo));

                                stringElements.Item(1).AppendChild(toastXml.CreateTextNode(mensagem));

                                ToastNotification notification = new ToastNotification(toastXml);
                                ToastNotificationManager.CreateToastNotifier().Show(notification);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Invalid message type: " + smsDetails.MessageType);
                }
                smsDetails.Accept();
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("SmsToExpense", ex);
                Debug.WriteLine("Error displaying toast: " + ex.Message);
            }
            finally
            {

            }
        }

        private DadosDespesa ProcessarSmsSantander(string sInput)
        {
            var retorno = new DadosDespesa();
            retorno.Valor = 0D;
            retorno.Origem = String.Empty;
            retorno.Data = DateTime.MinValue;
            retorno.Tipo = "-"; //Padrão é despesa

            var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    //Pega o valor
                    if (s[i] == "R$")
                    {
                        retorno.Valor = double.Parse(s[i + 1]);
                        continue;
                    }
                    //Pega a data e o texto descritivo
                    if (s[i] == "em")
                    {
                        retorno.Data = DateTime.Parse(s[i + 1] + " " + s[i + 3]);
                        for (int x = i + 4; x < s.Length; x++)
                        {
                            retorno.Origem += s[x] + " ";
                        }
                        break;
                    }
                }
            }
            return retorno;
        }

        private DadosDespesa ProcessarSmsSantanderDOCTED(string sInput)
        {
            var retorno = new DadosDespesa();
            retorno.Valor = 0D;
            retorno.Origem = String.Empty;
            retorno.Data = DateTime.MinValue;
            retorno.Tipo = "-"; //Padrão é despesa

            var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (s.Length > 0)
            {
                retorno.Tipo = s[4] == "credito" ? "+" : "-";
                retorno.Data = DateTime.Parse(s[7] + " " + s[9]);
                retorno.Valor = double.Parse(s[14]);
                retorno.Origem = s[2];
            }
            return retorno;
        }

        private DadosDespesa ProcessarSmsCredicard(string sInput)
        {
            var retorno = new DadosDespesa();
            retorno.Valor = 0D;
            retorno.Origem = String.Empty;
            retorno.Data = DateTime.MinValue;
            retorno.Tipo = "-"; //Padrão é despesa

            //Compra aprovada no seu CREDICARD GOLD VS final 7971 - VAREJAO ALVORADA valor RS 21,25 em 16/10, as 15h40
            //VAREJAO ALVORADA valor RS 21,25 em 16/10, as 15h40
            var s2 = sInput.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (s2.Length > 0 && s.Length > 0)
            {
                //Tenta localizar a origem
                var sOrigem = s2[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < sOrigem.Length; i++)
                {
                    if (sOrigem[i] != "valor")
                        retorno.Origem += sOrigem[i] + " ";
                    else
                        break;
                }

                //Tenta localizar o valor
                for (int i = 0; i < sOrigem.Length; i++)
                {
                    if (sOrigem[i] == "RS")
                    {
                        retorno.Valor = double.Parse(sOrigem[i + 1], System.Globalization.CultureInfo.CurrentCulture.NumberFormat);
                        break;
                    }
                }

                //Tenta localizar o valor
                for (int i = 0; i < sOrigem.Length; i++)
                {
                    if (sOrigem[i] == "em")
                    {
                        var data = sOrigem[i + 1].Replace(",", "") + "/" + DateTime.Now.Year.ToString() + " " + sOrigem[i + 3].Replace("h", ":");
                        Debug.WriteLine(data);
                        retorno.Data = DateTime.Parse(data, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
                        break;
                    }
                }

                retorno.Tipo = "-";

            }
            return retorno;
        }

        private DadosDespesa ProcessarSmsItau(string sInput)
        {
            var retorno = new DadosDespesa();
            retorno.Valor = 0D;
            retorno.Origem = String.Empty;
            retorno.Data = DateTime.MinValue;
            retorno.Tipo = "-"; //Padrão é despesa

            /*
            ITAU DEBITO: Cartao final 5737 COMPRA APROVADA 26/10 19:46:29 R$ 50,00 Local: POSTO AGU. Consulte tambem pelo celular www.itau.com.br.

            Compra aprovada no seu ITAU MULT 2.0 MC GOL final 5737 - NETFLIX.COM valor RS 19,90 em 24/10, as 08h47.

            Realizado pagamento de VIVO-SP 0246633009 no valor de R$ 34,99 na sua conta Itau XXX98-2 em 23/10 as 08:38.

            */

            if (sInput.ToUpper().StartsWith("ITAU DEBITO: CARTAO FINAL")) //Operação de cartão de débito
            {
                var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s.Length; i++)
                {
                    //O itau manda as vezes COMPRA APROVADA, SAQUE APROVADO
                    if (s[i] == "APROVADA" || s[i] == "APROVADO")
                    {
                        //Tenta obter a data                                               
                        retorno.Data = DateTime.Parse(s[i + 1] + "/" + DateTime.Now.Year.ToString() + " " + s[i + 2], new CultureInfo("pt-BR"));
                    }

                    if (s[i] == "R$")
                    {
                        retorno.Valor = double.Parse(s[i + 1], new CultureInfo("pt-BR"));
                    }

                    if (s[i] == "Local:")
                    {
                        for (int x = i + 1; x < s.Length; x++)
                        {
                            if (s[x] == "Consulte") break;

                            retorno.Origem += s[x] + " ";
                        }
                    }
                }
                return retorno;
            }
            else if (sInput.ToUpper().StartsWith("COMPRA APROVADA NO SEU ITAU")) //Operação de cartão de crédito
            {
                var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == "-")
                    {
                        for (int x = i + 1; x < s.Length; x++)
                        {
                            if (s[x] == "valor") break;

                            retorno.Origem += s[x] + " ";
                        }
                    }

                    if (s[i] == "RS")
                    {
                        retorno.Valor = double.Parse(s[i + 1], new CultureInfo("pt-BR"));
                    }

                    if (s[i] == "em")
                    {
                        var data = s[i + 1].Replace(",", "") + "/" + DateTime.Now.Year.ToString() + " ";
                        var hora = s[i + 3].Replace("h", ":").Replace(".", "");
                        //Tenta obter a data                                               
                        retorno.Data = DateTime.Parse(string.Concat(data, hora), new CultureInfo("pt-BR"));
                    }
                }
            }
            else if (sInput.ToUpper().StartsWith("REALIZADO PAGAMENTO")) //Pagamento de conta via Home Banking
            {
                var s = sInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == "de" && String.IsNullOrWhiteSpace(retorno.Origem))
                    {
                        for (int x = i + 1; x < s.Length; x++)
                        {
                            if (s[x] == "no") break;

                            retorno.Origem += s[x] + " ";
                        }


                    }

                    if (s[i] == "R$")
                    {
                        retorno.Valor = double.Parse(s[i + 1], new CultureInfo("pt-BR"));
                    }

                    if (s[i] == "em")
                    {
                        var data = s[i + 1].Replace(",", "") + "/" + DateTime.Now.Year.ToString() + " ";
                        var hora = s[i + 3].Replace("h", ":");
                        //Tenta obter a data                                               
                        retorno.Data = DateTime.Parse(string.Concat(data, hora), new CultureInfo("pt-BR"));
                    }
                }
            }
            return null;
        }

        private DadosDespesa ProcessarSMS(string sInput)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sInput))
                    return null;
                
                var retorno = new DadosDespesa();
                
                if (sInput.ToUpper().Contains("SANTANDER INFORMA: TRANSACAO")) //Santander
                {
                    retorno = ProcessarSmsSantander(sInput);
                }
                else if (sInput.ToUpper().Contains("SANTANDER INFORMA: DOC/TED"))
                {
                    retorno = ProcessarSmsSantanderDOCTED(sInput);
                }                
                else if (sInput.ToUpper().Contains("COMPRA APROVADA NO SEU CREDICARD")) //Credicard (Master e Visa)
                {
                    retorno = ProcessarSmsCredicard(sInput);
                }
                else if (sInput.ToUpper().Contains("ITAU"))
                {
                    retorno = ProcessarSmsItau(sInput);
                }
                else
                {
                    retorno = null;
                }

                if (retorno != null)
                {
                    AppLogs.WriteInfo("DEBUG", message: sInput);
                    Debug.WriteLine("Parse {0} /  {1} / {2:c2} / {3}", retorno.Tipo, retorno.Origem, retorno.Valor, retorno.Data);
                }
                return retorno;
            }
            catch(Exception ex)
            {
                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var stringElements = toastXml.GetElementsByTagName("text");

                stringElements.Item(0).AppendChild(toastXml.CreateTextNode("erro"));

                stringElements.Item(1).AppendChild(toastXml.CreateTextNode(ex.Message + " " + ex.StackTrace));

                ToastNotification notification = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(notification);

                AppLogs.WriteError("SMSToExpense", ex);
                return null;
            }
        }

        private void DisplayToast(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                SmsMessageReceivedTriggerDetails smsDetails = (SmsMessageReceivedTriggerDetails)taskInstance.TriggerDetails;
                if (smsDetails.MessageType == SmsMessageType.Text)
                {

                }


                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                var stringElements = toastXml.GetElementsByTagName("text");

                stringElements.Item(0).AppendChild(toastXml.CreateTextNode("De"));

                stringElements.Item(1).AppendChild(toastXml.CreateTextNode("Mensagem recebida"));

                ToastNotification notification = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(notification);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error displaying toast: " + ex.Message);
            }
        }


        // 
        // Handles background task cancellation. 
        // 
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // 
            // Indicate that the background task is canceled. 
            // 
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[sender.Task.TaskId.ToString()] = "Canceled";

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}