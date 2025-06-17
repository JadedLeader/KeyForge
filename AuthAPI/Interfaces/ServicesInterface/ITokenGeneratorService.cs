namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface ITokenGeneratorService
    {

        public string GenerateShortLivedToken(string accountId);

        public string GenerateLongLivedToken(string accountId);

    }
}
