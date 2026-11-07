namespace CurrencyExchangeAPI.Models
{
    public interface ISchemaMigrations
    {
        public void CreateDatabaseIfNotExists();

        public void CreateTables(string sqlFileName);
    }
}
