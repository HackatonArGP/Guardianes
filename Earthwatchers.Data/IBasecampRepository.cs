using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface IBasecampRepository
    {
        List<Basecamp> Get();
        List<Basecamp> GetBaseCamps();
        Basecamp Insert(Basecamp basecamp);
        void Delete(int id);
        Basecamp GetById(int id);
        Basecamp Edit(Basecamp basecamp);
        void RecalculateDistance(int id);
    }
}
