using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface ICollectionsRepository
    {
        List<CollectionItem> GetCollectionItemsByEarthwatcher(int earthwatcherId);
        CollectionItem GetNewCollectionItem(int earthwatcherId);
        int GetTotalItems(int ewId);
    }
}
