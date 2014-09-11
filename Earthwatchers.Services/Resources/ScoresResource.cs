using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Models.Portable;
using Earthwatchers.Services.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class ScoresResource
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IScoreRepository scoreRepository;

        public ScoresResource(IScoreRepository scoreRepository)
        {
            this.scoreRepository = scoreRepository;
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "?user={userid}")]
        public HttpResponseMessage<List<Score>> GetScoresByUserId(int userid, HttpRequestMessage request)
        {
            var scoreCollection = scoreRepository.GetScoresByUserId(userid);

            return scoreCollection != null ?
                new HttpResponseMessage<List<Score>>(scoreCollection) { StatusCode = HttpStatusCode.OK } :
                new HttpResponseMessage<List<Score>>(null) { StatusCode = HttpStatusCode.BadRequest };
        }

        [WebGet(UriTemplate = "/servertime")]
        public HttpResponseMessage<Score> GetServerTime()
        {
            try
            {
                return new HttpResponseMessage<Score>(new Score { Published = DateTime.UtcNow }) { StatusCode = HttpStatusCode.OK };

            }
            catch (Exception ex)
            {
                logger.ErrorException("Excepcion, no se ejecuto servertime", ex);
                return new HttpResponseMessage<Score>(null) { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/leaderboard?user={userid}")]
        public HttpResponseMessage<List<Score>> GetLeaderBoard(int userid, HttpRequestMessage request)
        {
            try
            {
                //var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<Hubs>();
                //context.Clients.All.LandVerified(1050);

                var leaderBoard = scoreRepository.GetLeaderBoard(false);
                var scoreCollection = leaderBoard.Take(10).ToList();
                if (!scoreCollection.Any(x => x.EarthwatcherId == userid))
                {
                    scoreCollection.Add(leaderBoard.Where(x => x.EarthwatcherId == userid).First());
                }
                return new HttpResponseMessage<List<Score>>(scoreCollection) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<Score>>(null) { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebGet(UriTemplate = "/contestleaderboard?user={userid}")]
        public HttpResponseMessage<List<Score>> GetContestLeaderBoard(int userid, HttpRequestMessage request)
        {
            try
            {
                var leaderBoard = scoreRepository.GetLeaderBoard(true);
                var scoreCollection = leaderBoard.Take(10).ToList();
                if (!scoreCollection.Any(x => x.EarthwatcherId == userid))
                {
                    scoreCollection.Add(leaderBoard.Where(x => x.EarthwatcherId == userid).First());
                }

                return new HttpResponseMessage<List<Score>>(scoreCollection) { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage<List<Score>>(null) { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "/update", Method = "POST")]
        public HttpResponseMessage<Score> UpdateScore(Score score, HttpRequestMessage<Score> request)
        {
            if (score.EarthwatcherId != 0 &&
                !String.IsNullOrEmpty(score.Action) &&
                score.Points != 0)
            {
                var newScore = scoreRepository.UpdateScore(score);
                var response = new HttpResponseMessage<Score>(newScore) { StatusCode = HttpStatusCode.OK };
                response.Headers.Location = new Uri(newScore.Uri, UriKind.Relative);
                return response;
            }
            else
            {
                var response = new HttpResponseMessage<Score>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "request parameters not correct" };
                return response;
            }
        }

        [BasicHttpAuthorization(Role.Earthwatcher)]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        public HttpResponseMessage<List<Score>> PostScore(List<Score> scores, HttpRequestMessage<List<Score>> request)
        {
            try
            {
                List<Score> newScores = new List<Score>();
                foreach (var score in scores)
                {
                    if (score.EarthwatcherId != 0 && !String.IsNullOrEmpty(score.Action))
                    {
                        newScores.Add(scoreRepository.PostScore(score));

                    }
                }

                var response = new HttpResponseMessage<List<Score>>(newScores) { StatusCode = HttpStatusCode.Created };
                return response;
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage<List<Score>>(null) { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = "request parameters not correct" };
                return response;
            }
        }
    }
}