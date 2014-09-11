using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using NLog;

namespace Earthwatchers.Services.Resources
{
    public static class SendMails
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static List<System.Net.Mail.MailMessage> _messages;
        private static System.Net.Mail.SmtpClient smtpClient = null;
        private static Regex emailPattern = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                                               + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                               + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                               + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                               + @"[a-zA-Z]{2,}))$");

        public static void Send(List<System.Net.Mail.MailMessage> messages)
        {
            _messages = messages;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                InitSMTPClient();
                foreach (var message in _messages)
                {
                    if (emailPattern.IsMatch(message.To.First().Address))
                    {
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["smtp.overridetoaddress"]))
                        {
                            message.To.Clear();
                            message.To.Add(System.Configuration.ConfigurationManager.AppSettings["smtp.overridetoaddress"]);
                        }

                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["smtp.appendbccaddress"]))
                        {
                            message.To.Add(System.Configuration.ConfigurationManager.AppSettings["smtp.appendbccaddress"]);
                        }
                        smtpClient.Send(message);

                        //Logueo el envio
                        logger.Info(string.Format("{0} - [Envio Mail Exitoso] - Se envio exitosamente el mail a {1}", DateTime.Now.ToString(), message.To.First().Address));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("{0} - [Envio Mail con Error] - {1}", DateTime.Now.ToString(), ex.Message));
            }
        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private static void InitSMTPClient()
        {
            smtpClient = new System.Net.Mail.SmtpClient();
            smtpClient.UseDefaultCredentials = false;
            if (System.Configuration.ConfigurationManager.AppSettings["smtp.enableSSL"] != null)
            {
                smtpClient.EnableSsl =
                    Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["smtp.enableSSL"]);
            }
            smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings["smtp.host"];
            smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["smtp.port"]);

            string user = System.Configuration.ConfigurationManager.AppSettings["smtp.user"];
            string password = System.Configuration.ConfigurationManager.AppSettings["smtp.password"];
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(user, password);
            }
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        }
    }
}