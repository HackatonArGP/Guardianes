using System;
using System.Collections.Generic;
using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface IEarthwatcherRepository
    {
        Earthwatcher GetEarthwatcher(int id);
        Earthwatcher GetEarthwatcher(string name, bool getLands);
        Earthwatcher GetEarthwatcherByGuid(Guid guid);
        List<Earthwatcher> GetAllEarthwatchers();
        Earthwatcher CreateEarthwatcher(Earthwatcher earthwatcher);
        void UpdateEarthwatcher(int id, Earthwatcher earthwatcher);
        void SetEarthwatcherAsPowerUser(int id, Earthwatcher earthwatcher);
        void DeleteEarthwatcher(int id);
        LandMini AssignLandToEarthwatcher(int earthwatcherid, string baseCamp, string geohexKey);
        bool EarthwatcherExists(string name);
        void SavePetitionSigned(PetitionsSigned petition);
        PetitionsSigned HasSigned(PetitionsSigned petition);

        ApiEw GetApiEw(string api, string userId);
        ApiEw GetApiEwById(int apiEwId);
        void LinkApiAndEarthwatcher(int apiEwId, int EwId);
        ApiEw CreateApiEwLogin(ApiEw ew);

        void UpdateAccessToken(int Id, string AccessToken);
    }
}
