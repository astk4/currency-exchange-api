using CurrencyExchangeAPI.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CurrencyExchangeAPI.Infrastructure.Persistence
{
    public class SchemaMigrationsPostgres(IOptions<HostingSettings> options) : ISchemaMigrations
    {
        private readonly IOptions<HostingSettings> options = options;

        public void CreateDatabase()
        {
            string[] connectionParts = options.Value.ConnectionString.Split(';');
            Console.WriteLine($"conn str length {options.Value.ConnectionString.Length} characters, array length {connectionParts.Length}");

            string noDbConnStr = string.Join(';', connectionParts.Take(4)) + ";";

            string databaseNameIs = connectionParts[4];
            string onlyDbName = databaseNameIs.Split('=')[1];

            using (NpgsqlConnection conn = new NpgsqlConnection(noDbConnStr))
            {
                conn.Open();

                string checkDb = $"select oid from pg_database where datname = '{onlyDbName}';";
                object? existingDbEntry = conn.ExecuteScalar(checkDb);

                if (existingDbEntry == null)
                {
                    string createDbQuery = $"CREATE DATABASE \"{onlyDbName}\";";
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
