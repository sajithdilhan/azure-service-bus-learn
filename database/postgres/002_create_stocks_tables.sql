-- Stocks service tables.
-- Run after 000_create_extensions_and_schemas.sql.

CREATE TABLE IF NOT EXISTS stocks.stocks
(
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id varchar(64) NOT NULL,
    product_name varchar(160) NOT NULL,
    quantity_available integer NOT NULL DEFAULT 0,
    quantity_reserved integer NOT NULL DEFAULT 0,
    last_restocked_at timestamp with time zone NOT NULL DEFAULT now(),
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),

    CONSTRAINT uq_stocks_product_id UNIQUE (product_id),
    CONSTRAINT ck_stocks_quantity_available CHECK (quantity_available >= 0),
    CONSTRAINT ck_stocks_quantity_reserved CHECK (quantity_reserved >= 0),
    CONSTRAINT ck_stocks_reserved_not_above_available CHECK (quantity_reserved <= quantity_available)
);

CREATE INDEX IF NOT EXISTS ix_stocks_product_name ON stocks.stocks (product_name);
