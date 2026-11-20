# $1 is root path

echo "writing db connection string..."
echo -n "User ID=postgres;Password=$PG_DB_PASSWORD" >> $1/test-db-connection-string.txt
echo -n ";Host=localhost;Port=5432;Database=CurrencyExchangePracticeTEST;" >> $1/test-db-connection-string.txt

echo "writing api 1 config (open exchange rates)"
echo "API_KEY=$CURRENCY_DATA_API_KEY" >> $1/test-config-oxr.txt
echo "BASE_URL=https://openexchangerates.org/api/" >> $1/test-config-oxr.txt

echo "writing api 2 config (exchange rates v6)"
echo "API_KEY=$EXCHANGE_RATE_API_KEY" >> $1/test-config-rates.txt
echo "BASE_URL=https://v6.exchangerate-api.com/v6/" >> $1/test-config-rates.txt