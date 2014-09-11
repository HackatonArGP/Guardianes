using Earthwatchers.Models;
using System.Collections.Generic;

namespace Earthwatchers.Data
{
    public interface IJaguarRepository
    {
        List<JaguarGame> Get();
        JaguarGame Insert(JaguarGame jaguarPos);
        void Delete(int id);
        void Update(int earthWatcherId, int posId);
        JaguarGame GetPos(int id);
    }
}
