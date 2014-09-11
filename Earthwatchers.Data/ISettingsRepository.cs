using Earthwatchers.Models;

namespace Earthwatchers.Data
{
    public interface ISettingsRepository
    {
        Setting GetSetting(string name);
    }
}
