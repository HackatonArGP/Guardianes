using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface ISatelliteImageRepository
    {
        void Delete(int id);
        SatelliteImage Get(int id);
        List<SatelliteImage> GetAll();
        SatelliteImage Insert(SatelliteImage satelliteImage);
        List<SatelliteImage> Intersects(string wkt);
        void Update(int id, SatelliteImage satelliteImage);
        void UpdateList(List<SatelliteImage> images);
    }
}
