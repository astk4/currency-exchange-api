using CurrencyExchangeAPI;
using CurrencyExchangeAPI.Infrastructure;
using CurrencyExchangeAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using System.Data;
using static Dapper.SqlMapper;

namespace CurrencyExchangeApiTest.Integration
{
    public sealed class RefreshBackgroundServiceTest : IClassFixture<TestWebAppFactory>, IDisposable
    {
        private const string cleanDoubleQuery = "DELETE FROM currencies; DELETE FROM exchanges;";

        private readonly WebApplicationFactory<Program> factory;

        private readonly Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
        private readonly Mock<ILogger<RatesRefreshService>> mockLogger = new Mock<ILogger<RatesRefreshService>>();
        private readonly IDbConnection dbConnection;

        public RefreshBackgroundServiceTest(TestWebAppFactory factory)
        {
            this.factory = factory;

            dbConnection = new NpgsqlConnection(TestCredentialsUtils.DbConnectionString);
            dbConnection.Open();
        }

        [Fact]  

        public async Task NotEnoughCurrenciesNoRefreshesTest()
        {
            mockConfig.Setup(a => a["RefreshMinutes"]).Returns("1");

            IServiceScopeFactory scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();   
            IMemoryCache cache = factory.Services.GetRequiredService<IMemoryCache>();

            RatesRefreshService refreshService = new RatesRefreshService(mockConfig.Object, scopeFactory, cache, mockLogger.Object);

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Currency entity = new Currency() { Code = "UAH", Name = "" };
            await dbConnection.ExecuteAsync("INSERT INTO currencies (code, name) VALUES (@Code, @Name)", entity);

            await refreshService.StartAsync(token);

            using (PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(2.5)))
            {
                await timer.WaitForNextTickAsync(token);
                await cancelTokenSource.CancelAsync();
            }

            IEnumerable<Exchange> exchanges = await dbConnection.QueryAsync<Exchange>("SELECT * FROM EXCHANGES");

            cancelTokenSource.Dispose();

            Assert.Empty(exchanges);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        public async Task EnoughCurrenciesRefreshesTest(string minutesInterval)
        {
            mockConfig.Setup(a => a["RefreshMinutes"]).Returns(minutesInterval);

            IServiceScopeFactory scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
            IMemoryCache cache = factory.Services.GetRequiredService<IMemoryCache>();

            RatesRefreshService refreshService = new RatesRefreshService(mockConfig.Object, scopeFactory, cache, mockLogger.Object);

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Currency[] testCurrencies = new Currency[3]
            {
                new Currency() { Code = "UAH", Name="Ukrainian Hryvnia" },
                new Currency() { Code = "CAD", Name="Canadian Dollar" },
                new Currency() { Code = "HKD", Name="Hong Kong Dollar" }
            };
            await dbConnection.ExecuteAsync("INSERT INTO currencies (code, name) VALUES (@Code, @Name)", testCurrencies);

            await refreshService.StartAsync(token);

            using (PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(Convert.ToInt32(minutesInterval)*2.5)))
            {
                await timer.WaitForNextTickAsync(token);
                await cancelTokenSource.CancelAsync();
            }

            IEnumerable<Exchange> exchanges = await dbConnection.QueryAsync<Exchange>("SELECT * FROM EXCHANGES");

            int tableSize = testCurrencies.Length*(testCurrencies.Length-1);

            Assert.NotEmpty(exchanges);
            Assert.Equal(tableSize, exchanges.Count());

            cancelTokenSource.Dispose();
        }

        public void Dispose()
        {
            using (IDbCommand cleanCommand = dbConnection.CreateCommand())
            {
                cleanCommand.CommandText = cleanDoubleQuery;
                cleanCommand.ExecuteNonQuery();
            }

            dbConnection.Close();
            dbConnection.Dispose();
        }
    }
}
