using CurrencyExchangeAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CurrencyExchangeApiTest.Integration
{
    public class TestWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                Dictionary<string, string?> inMemory = new Dictionary<string, string?>
                {
                    ["Data:Postgres:Main:ConnectionString"] = TestCredentialsUtils.DbConnectionString,
                    ["API:ExchangeRate-API:Key"] = TestCredentialsUtils.EXRAPI.key,
                    ["API:OpenExchangeRates:Key"] = TestCredentialsUtils.OXR.key,
                };

                configBuilder.AddInMemoryCollection(inMemory);
            });
        }
    }
}
