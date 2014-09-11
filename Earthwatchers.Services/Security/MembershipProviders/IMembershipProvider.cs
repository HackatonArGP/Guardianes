namespace Earthwatchers.Services.Security.MembershipProviders
{
    public interface IMembershipProvider
    {
        bool ValidateUser(string username, string password, int apiEwId);
    }
}
