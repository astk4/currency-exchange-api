namespace CurrencyExchangeApiTest
{
    public static class TestCredentialsUtils
    {
        public record ApiCredentials(string key, string baseUrl);

        public static readonly ApiCredentials OXR, EXRAPI;
        public static readonly string DbConnectionString;

        private static ApiCredentials ReadConfigLines(string configFileName)
        {
            string[] configLines = File.ReadAllLines(configFileName);
            return new ApiCredentials(configLines[0].Split('=')[1], configLines[1].Split('=')[1]);
        }

        static TestCredentialsUtils()
        {
            OXR = ReadConfigLines("test-config-oxr.txt");
            EXRAPI = ReadConfigLines("test-config-rates.txt");
            DbConnectionString = File.ReadAllText("test-db-connection-string.txt");
        }
    }
}
