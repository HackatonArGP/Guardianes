using Earthwatchers.Services.Resources;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Earthwatchers.Services
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void testButton_Click(object sender, EventArgs e)
        {
            List<System.Net.Mail.MailMessage> messages = new List<System.Net.Mail.MailMessage>();
            for (int i = 0; i < 2; i++)
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["smtp.enabled"]))
                {
                    System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress("lgorganchian@gmail.com");
                    System.Net.Mail.MailAddress addressFrom = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["smtp.user"], "Guardianes - Greenpeace");
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.From = addressFrom;
                    message.To.Add(address);
                    message.Subject = "Te damos la bienvenida a Guardianes";

                    string domain = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).GetLeftPart(UriPartial.Authority);

                    string htmlTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mail.html"));
                    message.Body = string.Format(htmlTemplate, "Ya estás protegiendo los bósques de Salta!"
                        , @"Para Comenzar a jugar apretá el botón de abajo!
                                <br></br>

                                <b>Tips para Jugar</b>
                                <ul>
                                <li> Podés obtener una nueva parcela apretando [Cambiar Parcela]</li>
                                <li> Comentá en las parcelas de los demás para ayudarlos con sus reportes</li>
                                <li> Completá todos los tutoriales y jugá los minijuegos</li>
                                <li> Entrá varias veces al dia para obtener mas puntos</li>
                                </ul>"
                        , string.Format("{0}/index.html", domain), "Click acá para comenzar a cuidar el bósque", "Cuidar el Bosque", "Este mensaje se envío a ", "lgorganchian@gmail.com"
                        , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                        , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.None;
                    messages.Add(message);
                }
            }

            SendMails.Send(messages);

            this.hexCode.Text = "terminó";

            //var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
            //context.Clients.All.LandChanged(hexCode.Text);
            /*
            Earthwatchers.Data.LandRepository repo = new Data.LandRepository(System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString);
            this.contentDiv.InnerHtml = string.Empty;
            foreach (var d in repo.MassiveReassign().OrderBy(x => x.Key))
            {
                //this.contentDiv.InnerHtml += d.Key.ToString() + "-" + d.Value + "<br>";
                this.contentDiv.InnerHtml += string.Format("'{0}',", d.Value);
            }
             * */
        }
    }
}