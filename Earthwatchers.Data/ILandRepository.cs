using System.Collections.Generic;
using Earthwatchers.Models;
using System;

namespace Earthwatchers.Data
{
    public interface ILandRepository
    {
        LandMini AddVerification(LandMini land, bool isAlert);
        List<Land> GetAll(int earthwatcherId);
        List<LandCSV> GetLandsToConfirm(int page, bool showVerifieds);
        Land GetLand(int id);
        List<Land> GetLandByIntersect(string wkt, int landId);
        List<Land> GetLandByStatus(LandStatus status);
        void UnassignLand(int id);
        void UpdateLandStatus(int id, LandStatus landStatus);
        bool UpdateLandsDemand(List<Land> lands, int earthwatcherId = 0);
        List<Land> GetLandByEarthwatcherName(string earthwatcherName);
        void AssignLandToEarthwatcher(string geohex, int earthwatcherid);
        bool ResetLands();
        //void ForceLandReset(int landId, int earthwatcherId); Obsoleto, no existe ni el metodo
        LandMini ReassignLand(int earthwatcherId, string basecamp);//Nuevo
        LandMini ReassignLand(Land land, string basecamp); //Obsoleto
        Land GetLandByGeoHexKey(string geoHexKey);
        bool MassiveReassign(string basecamp);
        List<Score> GetLastUsersWithActivityScore(int landId);
        List<LandCSV> GetLandsCSV();
        List<Statistic> GetStats();
        List<string> GetVerifiedLandsGeoHexCodes(int earthwatcherId, bool isPoll);
        string GetImageBaseUrl(bool isBase);
        void AddPoll(LandMini land);
        bool ForceLandsReset(List<Land> lands, int earthwatcherId = 0);
        void LoadLandBasecamp();

        List<Land> GetAllLands();
    }
}
