namespace CurrencyExchangeApiTest.Integration
{
    public abstract class ExternalApiTestBase : IDisposable
    {
        protected const string contentTypeStr = "application/json";

        protected readonly HttpClient httpClient;

        protected ExternalApiTestBase() => httpClient = new HttpClient();

        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
