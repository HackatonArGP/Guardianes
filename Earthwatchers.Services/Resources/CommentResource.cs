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
    public class CommentResource
    {
        private readonly ICommentRepository commentRepository;

        public CommentResource(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        [WebGet(UriTemplate = "/land={landid}")]
        public HttpResponseMessage<List<Comment>> GetCommentsByLandId(int landid, HttpRequestMessage request)
        {
            var commentCollection = commentRepository.GetCommentsByLand(landid);
            return commentCollection != null ? 
                new HttpResponseMessage<List<Comment>>(commentCollection) { StatusCode = HttpStatusCode.OK } : 
                new HttpResponseMessage<List<Comment>>(null) { StatusCode = HttpStatusCode.BadRequest};
        }

        [WebGet(UriTemplate = "/user={userid}")]
        public HttpResponseMessage<List<Comment>> GetCommentByUserId(int userid, HttpRequestMessage request)
        {
            var commentCollection = commentRepository.GetCommentsByUserId(userid);

            return commentCollection != null ? 
                new HttpResponseMessage<List<Comment>>(commentCollection) { StatusCode = HttpStatusCode.OK } : 
                new HttpResponseMessage<List<Comment>>(null) { StatusCode = HttpStatusCode.BadRequest};
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<Comment> PostComment(Comment comment, HttpRequestMessage<Comment> request)
        {
            if (comment.EarthwatcherId != 0 && comment.LandId != 0 & comment.UserComment != null)
            {
                var newcomment=commentRepository.PostComment(comment);
                var response = new HttpResponseMessage<Comment>(newcomment) { StatusCode = HttpStatusCode.Created };
                response.Headers.Location = new Uri(newcomment.Uri, UriKind.Relative);
                return response;
            }
            return null;
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/del", Method = "POST")]
        public HttpResponseMessage<Comment> DeleteComment(Comment comment, HttpRequestMessage<Comment> request)
        {
            if (comment.Id != 0)
            {
                commentRepository.DeleteComment(comment.Id);
                return new HttpResponseMessage<Comment>(null) { StatusCode = HttpStatusCode.OK };
            }
            return new HttpResponseMessage<Comment>(null) { StatusCode = HttpStatusCode.NotFound };
        }
    }
}