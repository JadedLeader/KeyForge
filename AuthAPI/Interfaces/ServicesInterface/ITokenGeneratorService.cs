namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface ITokenGeneratorService
    {

        public string GenerateShortLivedToken(string accountId, string role);

        public string GenerateLongLivedToken(string accountId, string role);

    }
}
