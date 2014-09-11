using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface IFlagRepository
    {
        void DeleteFlag(int flagId);
        List<Flag> GetFlags();
        Flag PostFlag(Flag flag);
    }
}
