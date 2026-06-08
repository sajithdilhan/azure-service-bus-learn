-- Payments service tables.
-- Run after 000_create_extensions_and_schemas.sql and 001_create_orders_tables.sql.

CREATE TABLE IF NOT EXISTS payments.payments
(
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id uuid NOT NULL,
    amount numeric(12, 2) NOT NULL,
    payment_method varchar(24) NOT NULL,
    payment_date timestamp with time zone NOT NULL DEFAULT now(),
    status varchar(24) NOT NULL,
    transaction_id varchar(80) NOT NULL,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),

    CONSTRAINT fk_payments_order_id
        FOREIGN KEY (order_id)
        REFERENCES orders.orders (id)
        ON DELETE RESTRICT,
    CONSTRAINT uq_payments_transaction_id UNIQUE (transaction_id),
    CONSTRAINT ck_payments_amount CHECK (amount >= 0),
    CONSTRAINT ck_payments_payment_method CHECK (payment_method IN ('CreditCard', 'DebitCard', 'PayPal', 'BankTransfer')),
    CONSTRAINT ck_payments_status CHECK (status IN ('Pending', 'Confirmed', 'Failed'))
);

CREATE INDEX IF NOT EXISTS ix_payments_order_id ON payments.payments (order_id);
CREATE INDEX IF NOT EXISTS ix_payments_status ON payments.payments (status);
CREATE INDEX IF NOT EXISTS ix_payments_payment_date ON payments.payments (payment_date);
