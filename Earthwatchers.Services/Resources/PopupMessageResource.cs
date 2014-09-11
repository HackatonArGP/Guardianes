using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class PopupMessageResource
    {
        private readonly IPopupMessageRepository messageRepository;

        public PopupMessageResource(IPopupMessageRepository _messageRepository)
        {
            this.messageRepository = _messageRepository;
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebGet(UriTemplate = "/getall")]
        public HttpResponseMessage<List<PopupMessage>> GetAllMessages(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<PopupMessage>>(messageRepository.GetAllMessages()) { StatusCode = HttpStatusCode.OK };
            }
            catch
            {
                return new HttpResponseMessage<List<PopupMessage>>(null) { StatusCode = HttpStatusCode.BadRequest };
            }
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/add", Method = "POST")]
        public HttpResponseMessage<PopupMessage> Post(PopupMessage message, HttpRequestMessage<PopupMessage> request)
        {
            if (message != null)
            {
                var messageDB = messageRepository.Insert(message);

                var response = new HttpResponseMessage<PopupMessage>(messageDB) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            return new HttpResponseMessage<PopupMessage>(null) { StatusCode = HttpStatusCode.NotFound, };
        }

        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage Delete(int id, HttpRequestMessage<int> request)
        {
            if (id != 0)
            {
                messageRepository.Delete(id);

                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }



        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage<List<PopupMessage>> GetMessage(HttpRequestMessage request)
        {
            try
            {
                return new HttpResponseMessage<List<PopupMessage>>(messageRepository.GetMessage()) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<PopupMessage>>(null) { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}