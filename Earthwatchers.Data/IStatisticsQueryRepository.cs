using Earthwatchers.Models;
using System;
using System.Collections.Generic;

namespace Earthwatchers.Data
{
    public interface IStatisticsQueryRepository
    {
        IEnumerable<StatisticsQuery> GetStats(string stat, DateTime startDate, DateTime endDate);
    }
}
