using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Microsoft.AspNet.SignalR;
using System.Web;
using System.Linq;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class LandResource
    {
        private readonly ILandRepository landRepository;

        public LandResource(ILandRepository landRepository)
        {
            this.landRepository = landRepository;
        }

        [WebGet(UriTemplate = "{id}")]
        public HttpResponseMessage<Land> Get(int id, HttpRequestMessage request)
        {
            try
            {
                var land = landRepository.GetLand(id);
                if (land == null)
                {
                    return new HttpResponseMessage<Land>(HttpStatusCode.NotFound);
                }

                //LandLinks.AddLinks(land, request);

                return new HttpResponseMessage<Land>(land) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/stats")]
        public HttpResponseMessage<List<Statistic>> GetStats()
        {
            try
            {
                return new HttpResponseMessage<List<Statistic>>(landRepository.GetStats()) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<Statistic>>(null) { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/verifiedlandscodes", Method = "POST")]
        public HttpResponseMessage<List<string>> GetVerifiedLandsGeoHexCodes(Earthwatcher earthwatcher, HttpRequestMessage<Earthwatcher> request)
        {
            try
            {
                return new HttpResponseMessage<List<string>>(landRepository.GetVerifiedLandsGeoHexCodes(earthwatcher.Id, earthwatcher.IsPowerUser)) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<string>>(new List<string> { ex.Message }) { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "/landstoconfirm/{page}/{showVerifieds}")]
        public HttpResponseMessage GetLandsToConfirm(int page, bool showVerifieds, HttpRequestMessage request)
        {
            return new HttpResponseMessage<List<LandCSV>>(landRepository.GetLandsToConfirm(page, showVerifieds)) { StatusCode = HttpStatusCode.OK };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "/landscsv")]
        public HttpResponseMessage GetLandsCSV(HttpRequestMessage request)
        {
            return new HttpResponseMessage<List<LandCSV>>(landRepository.GetLandsCSV()) { StatusCode = HttpStatusCode.OK };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/generateimages", Method = "POST")]
        public HttpResponseMessage GenerateImages(HttpRequestMessage request)
        {
            try
            {
                //Genero las imágenes
                ImagesGeneratorTool.Run(landRepository, true, null);

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(ex.Message);
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/generategameimages", Method = "POST")]
        public HttpResponseMessage GenerateGameImages(HttpRequestMessage request)
        {
            try
            {
                //Genero las imágenes
                ImagesGeneratorTool.Run(landRepository, false, null);

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(ex.Message);
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/updatelandsdemand", Method = "POST")]
        public HttpResponseMessage UpdateLandsDemand(List<Land> lands, HttpRequestMessage<List<Land>> request)
        {
            var connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings["EarthwatchersConnection"].ConnectionString;
            var earthwatcher = Session.HasLoggedUser() ? new EarthwatcherRepository(connectionstring).GetEarthwatcher(Session.GetCookieInfo().EarthwatcherName, false) : null;

            var landsToConfirm = lands.Where(l => l.Reset == null || l.Reset == false).ToList();
            var landsToReset = lands.Where(l => l.Reset.HasValue && l.Reset == true).ToList();

            try
            {
                if (landsToReset.Any())
                {
                    landRepository.ForceLandsReset(landsToReset, earthwatcher != null ? earthwatcher.Id : 0);
                }

                if(landsToConfirm.Any())
                {
                    if (landRepository.UpdateLandsDemand(lands, earthwatcher != null ? earthwatcher.Id : 0))
                    {
                        //Genero las imágenes
                        //ImagesGeneratorTool.Run(landRepository, true, null);

                        //Send notification emails
                        SendEmail_GreenpeaceConfirmation(lands, earthwatcher);
                    }
                }

                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(ex.Message);
            }

            
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/resetlands", Method = "POST")]
        public HttpResponseMessage ResetLands(HttpRequestMessage request)
        {
            if (landRepository.ResetLands())
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/mar", Method = "POST")]
        public HttpResponseMessage MassiveReassign(HttpRequestMessage request)
        {
            string basecamp = System.Configuration.ConfigurationManager.AppSettings["Basecamp"];
            if (landRepository.MassiveReassign(basecamp))
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/addpoll", Method = "POST")]
        public HttpResponseMessage AddPoll(LandMini land, HttpRequestMessage<LandMini> request)
        {
            try
            {
                landRepository.AddPoll(land);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [WebInvoke(UriTemplate = "/intersect", Method = "POST")]
        public HttpResponseMessage<List<Land>> GetLandByWkt(Earthwatcher earthwatcher, HttpRequestMessage<Earthwatcher> request)
        {
            if (earthwatcher != null && !string.IsNullOrEmpty(earthwatcher.Name))
            {
                // todo: check if wkt is valid...
                // we can use SqlGeometry but there are some issues on Azure with that assembly
                // other option is do a spatial database query for this

                var landCollection = landRepository.GetLandByIntersect(earthwatcher.Name, earthwatcher.Id);

                return new HttpResponseMessage<List<Land>>(landCollection) { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage<List<Land>>(null) { StatusCode = HttpStatusCode.BadRequest };
        }

        [WebInvoke(UriTemplate = "/all", Method = "POST")]
        public HttpResponseMessage<List<Land>> GetAll(int earthwatcherId, HttpRequestMessage<int> request)
        {
            try
            {
                var landCollection = landRepository.GetAll(earthwatcherId);
                return new HttpResponseMessage<List<Land>>(landCollection) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<Land>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/activity={id}")]
        public HttpResponseMessage<List<Score>> GetActivity(int id, HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<Score>>(landRepository.GetLastUsersWithActivityScore(id)) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<Score>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [WebGet(UriTemplate = "/status={landStatus}")]
        public HttpResponseMessage<List<Land>> GetAllByStatus(LandStatus landStatus, HttpRequestMessage request)
        {
            var landCollection = landRepository.GetLandByStatus(landStatus);
            foreach (var land in landCollection)
            {
                LandLinks.AddLinks(land, request);
            }
            return new HttpResponseMessage<List<Land>>(landCollection) { StatusCode = HttpStatusCode.OK };
        }

        /*
        [WebGet(UriTemplate = "/earthwatcher={earthwatcher}")]
        public HttpResponseMessage<List<Land>> GetLandByEarthwatcherName(string earthwatcherName, HttpRequestMessage request)
        {
            if (!String.IsNullOrEmpty(earthwatcherName))
            {
                var lands = landRepository.GetLandByEarthwatcherName(earthwatcherName);
                if (lands != null)
                {
                    return new HttpResponseMessage<List<Land>>(lands) { StatusCode = HttpStatusCode.OK };
                }
                return new HttpResponseMessage<List<Land>>(null) { StatusCode = HttpStatusCode.NotFound };
            }
            return new HttpResponseMessage<List<Land>>(null) { StatusCode = HttpStatusCode.BadRequest };
        }
         * */

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/updatestatus", Method = "POST")]
        public HttpResponseMessage<Land> UpdateStatusLand(Land land, HttpRequestMessage<Land> request)
        {
            if (landRepository.GetLand(land.Id) != null)
            {
                landRepository.UpdateLandStatus(land.Id, land.LandStatus);
                return new HttpResponseMessage<Land>(land) { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage<Land>(HttpStatusCode.BadRequest);
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/confirm", Method = "POST")]
        public HttpResponseMessage Confirm(LandMini land, HttpRequestMessage<LandMini> request)
        {
            try
            {
                LandMini landMini = landRepository.AddVerification(land, false);
                VerificationScoring(landMini);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/deconfirm", Method = "POST")]
        public HttpResponseMessage Deconfirm(LandMini land, HttpRequestMessage<LandMini> request)
        {
            try
            {
                LandMini landMini = landRepository.AddVerification(land, true);
                VerificationScoring(landMini);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = ex.Message };
            }
        }

        private void VerificationScoring(LandMini landMini)
        {
            if (landMini != null)
            {
                try
                {
                    //Mando los mails notificando
                    if (!string.IsNullOrEmpty(landMini.Email))
                    {
                        if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["smtp.enabled"]))
                        {
                            List<System.Net.Mail.MailMessage> messages = new List<System.Net.Mail.MailMessage>();

                            System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(landMini.Email);
                            System.Net.Mail.MailAddress addressFrom = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["smtp.user"], "Guardianes - Greenpeace");
                            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                            message.From = addressFrom;
                            message.To.Add(address);
                            message.Subject = "Llegaste a las 30 verificaciones en tu parcela";

                            string domain = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).GetLeftPart(UriPartial.Authority);

                            string htmlTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mail.html"));
                            message.Body = string.Format(htmlTemplate, "Felicitaciones, otros 30 guardianes han verificado el reporte de tu parcela." 
                                , @"A partir de ahora Greenpeace verificará la validez del reporte!"
                                , string.Format("{0}/index.html?geohexcode={1}", domain, landMini.GeohexKey), "Click acá para seguir cuidando el bosque", "Cuidar el Bosque", "Este mensaje se envío a", landMini.Email
                                , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                                , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                            message.IsBodyHtml = true;
                            message.BodyEncoding = System.Text.Encoding.UTF8;
                            message.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.None;
                            messages.Add(message);

                            SendMails.Send(messages);
                        }
                    }

                    //Genero la imagen de este land
                    ImagesGeneratorTool.Run(landRepository, true, landMini.GeohexKey);

                    //Notify the land owner if logged in
                    var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
                    context.Clients.All.LandVerified(landMini.EarthwatcherId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //Este mismo metodo existe en EarthwatcherResource y es el que se debe utilizar
        //Si se quiere utilizar el mismo no usar la misma cantidad de parametros.

        //private void NotificateUsers(LandMini newLand, int earthwatcherId)
        //{
        //    if (newLand != null && newLand.IsUsed)
        //    {
        //        //Comunico a los usuarios conectados si es que la nueva land es de un usuario existente
        //        var context = GlobalHost.ConnectionManager.GetHubContext<Hubs>();
        //        context.Clients.All.LandChanged(newLand.GeohexKey, earthwatcherId);
        //    }
        //}

        private void SendEmail_GreenpeaceConfirmation(List<Land> lands, Earthwatcher earthwatcher)
        {
                //Mando los mails notificando (solo envio si es que tengo habilitado via web.config el envio
                //TODO: Push Notifications para actualizar
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["smtp.enabled"]))
                {
                    List<System.Net.Mail.MailMessage> messages = new List<System.Net.Mail.MailMessage>();
                    foreach (var land in lands)
                    {
                        if (land.Confirmed.HasValue)
                        {
                            System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(land.EarthwatcherName);
                            System.Net.Mail.MailAddress addressFrom = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings["smtp.user"], "Guardianes - Greenpeace");
                            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                            message.From = addressFrom;
                            message.To.Add(address);
                            string domain = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).GetLeftPart(UriPartial.Authority);
                            string htmlTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mail.html"));
                            //Confirmados en Amarillo 
                            if (land.Confirmed.Value && land.LandStatus == LandStatus.Alert)
                            {
                                message.Subject = "Parcela Verificada";
                                message.Body = string.Format(htmlTemplate, "¡Felicitaciones!"
, @"Tu parcela ha sido verificada por Greenpeace y comprobamos que tu reporte fue correcto.<br /><br /><b>¿Qué significa esto?</b><br>
La parcela que reportaste en Amarillo (desmontes) <b>efectivamente ha sido desmontada</b>. Este hecho debe ser denunciado, no podemos permitir desmontes en zonas protegidas por la Ley de Bosques.<br /><br />
A partir de este momento, tu parcela estará habilitada para Denuncias. Aparecerá con un color Rojo dentro de Guardianes, y haciendo click sobre la misma y luego en [Reportar], tendrás habilitado el boton [Denunciar].<br /><br />
Cuando veas el formulario, crea tu denuncia y hacé click en [Enviar] para que las autoridades correspondientes reciban tu reclamo.<br /><br />
¡Continuá reportando parcelas para mantener los bosques a salvo!"
                                , string.Format("{0}/index.html?geohexcode={1}", domain, land.GeohexKey), "Hacé click acá para ingresar a Guardianes", "Ingresar a Guardianes", "Este mensaje se envío a", land.EarthwatcherName
                                , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                                , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                            }

                            //Confirmados en Verde
                            if (land.Confirmed.Value && land.LandStatus == LandStatus.Ok)
                            {
                                message.Subject = "Parcela Verificada";
                                message.Body = string.Format(htmlTemplate, "¡Felicitaciones!"
, @"Tu parcela ha sido verificada por Greenpeace y comprobamos que tu reporte fue correcto.<br /><br /><b>¿Qué significa esto?</b><br>
    La parcela que reportaste en Verde (a salvo) efectivamente <b>no contenía desmontes</b>. 
Cuando haya disponible una nueva imágen satelital, esta parcela volverá a estar disponible para los guardianes,
 de modo que se pueda revisar que no hayan habido desmontes.<br /><br />¡Continuá reportando parcelas para mantener los bosques a salvo!"
                                , string.Format("{0}/index.html?geohexcode={1}", domain, land.GeohexKey), "Hacé click acá para ingresar a Guardianes", "Ingresar a Guardianes", "Este mensaje se envío a", land.EarthwatcherName
                                , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                                , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                            }

                            //Rechazados en Verde 
                            if (!land.Confirmed.Value && land.LandStatus == LandStatus.Ok)
                            {
                                message.Subject = "Reporte de verificación";
                                message.Body = string.Format(htmlTemplate, "Reporte Incorrecto"
, @"Tu parcela ha sido verificada por Greenpeace y determinamos que el reporte fue incorrecto.<br /><br /><b>¿Qué significa esto?</b><br>
    La parcela que reportaste en Verde (a salvo) fue revisada por nuestros expertos quienes determinaron que <b>contenía un desmonte.</b><br /><br />
    Pero no te preocupes, con el tiempo vas a ganar más experiencia y entrenar tu ojo para identificar mejor los desmontes y las zonas a salvo.<br /><br />
    <b>¿Que pasa ahora?</b><br />
    A partir de este momento, la parcela estará habilitada para Denuncias. Aparecerá con un color Rojo dentro de Guardianes, 
 y haciendo click sobre la misma y luego en [Reportar], estará habilitado el boton [Denunciar].<br /><br />
    Cuando veas el formulario, crea tu denuncia y hacé click en [Enviar] para que las autoridades correspondientes reciban tu reclamo. <br /><br />
                                    <b>Tips para mejorar tus reportes</b><br />
                                    <ul>
                                        <li>Podés jugar el Minijuego I desde el menú [Ayuda] para aprender a diferenciar zonas a salvo de zonas con desmontes.</li>
                                        <li>Podés volver a ver el Pre-Tutorial desde el menú [Ayuda].</li>
                                        <li>Podés revisar otras parcelas verificadas (en Rojo) para comparar reportes correctos.</li>
                                    </ul>
                                    No bajes los brazos, ¡continuá reportando parcelas para mantener los bosques a salvo!
                                    <br /><br />
                                    <table style='padding: 10px; margin: 10px; border-collapse: collapse;' cellpadding='10' cellspacing='0'>
                                        <tr>
                                    <td><img src='" + domain + "/SatelliteImages/demand/" + land.GeohexKey + "-a.jpg' galleryimg='no' /></td><td><img src='" + domain + "/SatelliteImages/demand/" + land.GeohexKey + "-a.jpg' galleryimg='no' /></td></tr><tr><td style='font-family: Arial; font-size: 11px;'>2008</td><td style='font-family: Arial; font-size: 11px;'>Ahora</td></tr></table><br /><br />"
                                , string.Format("{0}/index.html?geohexcode={1}", domain, land.GeohexKey), "Hacé click acá para ingresar a Guardianes", "Ingresar a Guardianes", "Este mensaje se envío a", land.EarthwatcherName
                                , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                                , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                            }

                            //Rechazados en Amarillo 
                            if (!land.Confirmed.Value && land.LandStatus == LandStatus.Alert)
                            {
                                message.Subject = "Reporte de verificación";
                                message.Body = string.Format(htmlTemplate, "Reporte Incorrecto"
, @"Tu parcela ha sido verificada por Greenpeace y determinamos que el reporte fue incorrecto.<br /><br /><b>¿Qué significa esto?</b><br>
   La parcela que reportaste en Amarillo (desmontes) fue revisada por nuestros expertos quienes determinaron que <b>no contenía desmontes, o que los desmontes corresponden a períodos anteriores a 2008</b>, cuando no teníamos ley de bosques.<br /><br />
   <b>Pero hay un desmonte, ¿por qué tuve una parcela asignada con un desmonte previo a 2008?</b><br /><br />
   En algunos casos, Guardianes asigna parcelas muy cerca de los límites entre zonas protegidas y zonas desmontadas o en verde. Cuando esto sucede, las parcelas pueden parecer como que están en un área protegida, cuando en realidad la mayor parte está en una zona ya desmontada.<br /><br />
   Estamos trabajando para corregir este problema. Si este es tu caso, ¡te pedimos disculpas por el inconveniente!<br /><br />
   <b>¿Que pasa ahora?</b><br />
   A partir de este momento, la parcela quedará marcada en verde. Cuando haya disponible una nueva imágen satelital, esta parcela volverá a estar disponible para los guardianes, de modo que se pueda revisar que no hayan habido desmontes.<br /><br />
                                    <b>Tips para mejorar tus reportes</b><br />
                                    <ul>
                                        <li>Podés jugar el Minijuego I desde el menú [Ayuda] para aprender a diferenciar zonas a salvo de zonas con desmontes.</li>
                                        <li>Podés volver a ver el Pre-Tutorial desde el menú [Ayuda].</li>
                                        <li>Podés revisar otras parcelas verificadas (en Rojo) para comparar reportes correctos.</li>
                                    </ul>
                                    <br />
                                    No bajes los brazos, ¡continuá reportando parcelas para mantener los bosques a salvo!
                                    <br /><br />
                                    <table style='padding: 10px; margin: 10px; border-collapse: collapse;' cellpadding='10' cellspacing='0'>
                                        <tr>
                                    <td><img src='" + domain + "/SatelliteImages/demand/" + land.GeohexKey + "-a.jpg' galleryimg='no' /></td><td><img src='" + domain + "/SatelliteImages/demand/" + land.GeohexKey + "-a.jpg' galleryimg='no' /></td></tr><tr><td style='font-family: Arial; font-size: 11px;'>2008</td><td style='font-family: Arial; font-size: 11px;'>Ahora</td></tr></table><br /><br />"
                                , string.Format("{0}/index.html?geohexcode={1}", domain, land.GeohexKey), "Hacé click acá para ingresar a Guardianes", "Ingresar a Guardianes", "Este mensaje se envío a", land.EarthwatcherName
                                , ". Si no quieres recibir más notificaciones en un futuro podés acceder al Panel de Control del usuario y deshabilitar la opción de recibir notificaciones."
                                , "Greenpeace Argentina. Todos los derechos reservados.", domain);
                            }

                            message.IsBodyHtml = true;
                            message.BodyEncoding = System.Text.Encoding.UTF8;
                            message.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.None;
                            messages.Add(message);
                        }
                    }

                    SendMails.Send(messages);
                }
        }
    }
}