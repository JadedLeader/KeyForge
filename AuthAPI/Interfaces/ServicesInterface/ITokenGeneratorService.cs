namespace AuthAPI.Interfaces.ServicesInterface
{
    public interface ITokenGeneratorService
    {

        public string GenerateShortLivedToken(string username);

        public string GenerateLongLivedToken(string username);

    }
}
