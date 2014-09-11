using Earthwatchers.Data;
using Earthwatchers.Models;
using Earthwatchers.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Earthwatchers.Services.Resources
{
    [ServiceContract]
    public class StatisticsQueryResource
    {
        private readonly IStatisticsQueryRepository statisticsQueryRepository;

        public StatisticsQueryResource(IStatisticsQueryRepository statisticsQueryRepository)
        {
            this.statisticsQueryRepository = statisticsQueryRepository;
        }

        //[WebGet(UriTemplate = "/getstats")]
        [BasicHttpAuthorization(Role.Admin)]
        [WebInvoke(UriTemplate = "/getstats", Method = "POST")]
        public HttpResponseMessage<List<StatisticsQuery>> GetStats(StatisticsQuery stat, HttpRequestMessage<StatisticsQuery> request)
        {
            var date = DateTime.Parse(stat.EndDate);
            //return new HttpResponseMessage<IEnumerable<StatisticsQuery>>(statisticsQueryRepository.GetStats(stat, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate)), System.Net.HttpStatusCode.OK);
            return null;
        }
        
    }
}