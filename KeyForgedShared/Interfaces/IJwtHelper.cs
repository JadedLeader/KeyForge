namespace KeyForgedShared.Interfaces
{
    public interface IJwtHelper
    {
        bool IsLongLivedKeyValid(string currentLongLivedKey);
        string ReturnAccountIdFromToken(string token);
        string ReturnRoleFromToken(string token);
    }
}