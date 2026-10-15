namespace CurrencyExchangeAPI.Models
{
    public interface ISchemaMigrations
    {
        public void CreateDatabase();

        public void CreateTables(string sqlFileName);
    }
}
