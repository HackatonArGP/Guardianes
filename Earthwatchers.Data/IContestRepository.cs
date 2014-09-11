using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
   public interface IContestRepository
    {
        Contest GetContest();
        Contest GetWinner();
        List<Contest> GetAllContests();
        Contest Insert(Contest contest);
        void Delete(int id);
    }
}
