using CurrencyExchangeAPI.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CurrencyExchangeAPI.Infrastructure.Persistence
{
    public class SchemaMigrationsPostgres(IOptions<HostingSettings> options) : ISchemaMigrations
    {
        private readonly IOptions<HostingSettings> options = options;

        public void CreateDatabaseIfNotExists()
        {
            NpgsqlConnectionStringBuilder connStrBuilder = new NpgsqlConnectionStringBuilder(options.Value.ConnectionString);

            string defaultDbName = Environment.GetEnvironmentVariable("DEFAULT_DB_NAME") ?? "postgres";
            string savedConnDbName = connStrBuilder.Database!;

            if (defaultDbName == savedConnDbName) { return; } // connection string already leads to default DB

            connStrBuilder.Database = defaultDbName;
            string connectionToDefaultDb = connStrBuilder.ToString();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionToDefaultDb))
            {
                conn.Open();

                string checkDb = $"select oid from pg_database where datname = '{savedConnDbName}';";
                object? existingDbEntry = conn.ExecuteScalar(checkDb);

                if (existingDbEntry == null)
                {
                    string createDbQuery = $"CREATE DATABASE \"{savedConnDbName}\";";
                    conn.Execute(createDbQuery);
                }

                conn.Close();
            }
        }

        public void CreateTables(string sqlFileName)
        {
            string dir = Environment.CurrentDirectory;
            string readCommands = File.ReadAllText(sqlFileName);

            using (NpgsqlConnection conn = new NpgsqlConnection(options.Value.ConnectionString))
            {
                conn.Open();

                conn.Execute(readCommands);

                conn.Close();
            }
        }
    }
}
