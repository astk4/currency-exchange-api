CREATE TABLE IF NOT EXISTS currencies
(
    code character varying(3)  NOT NULL PRIMARY KEY,
    name text  NOT NULL
);

CREATE TABLE IF NOT EXISTS exchanges
(
    base_code character varying(3)  NOT NULL,
    price_code character varying(3)  NOT NULL,
    rate real NOT NULL,
    measured_at timestamp without time zone,
    CONSTRAINT "Exchanges_pkey" PRIMARY KEY (base_code, price_code),
    CONSTRAINT "FK_BaseCode_Currencies_Code" FOREIGN KEY (base_code)
        REFERENCES currencies (code)
        ON DELETE CASCADE,
    CONSTRAINT "FK_PriceCode_Currencies_Code" FOREIGN KEY (price_code)
        REFERENCES currencies (code)
        ON DELETE CASCADE
);