-- Orders service tables.
-- Run after 000_create_extensions_and_schemas.sql.

CREATE TABLE IF NOT EXISTS orders.orders
(
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number varchar(32) NOT NULL,
    customer_id varchar(64) NOT NULL,
    customer_name varchar(120) NOT NULL,
    customer_phone varchar(32) NOT NULL DEFAULT '',
    customer_email varchar(254) NOT NULL,
    shipping_address varchar(500) NOT NULL,
    status varchar(24) NOT NULL,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),

    CONSTRAINT uq_orders_order_number UNIQUE (order_number),
    CONSTRAINT ck_orders_status CHECK (status IN ('Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled')),
    CONSTRAINT ck_orders_customer_email_not_blank CHECK (length(trim(customer_email)) > 0)
);

CREATE TABLE IF NOT EXISTS orders.order_lines
(
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id uuid NOT NULL,
    product_id varchar(64) NOT NULL,
    product_name varchar(160) NOT NULL,
    quantity integer NOT NULL,
    price numeric(12, 2) NOT NULL,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),

    CONSTRAINT fk_order_lines_order_id
        FOREIGN KEY (order_id)
        REFERENCES orders.orders (id)
        ON DELETE CASCADE,
    CONSTRAINT ck_order_lines_quantity CHECK (quantity > 0),
    CONSTRAINT ck_order_lines_price CHECK (price >= 0)
);

CREATE INDEX IF NOT EXISTS ix_orders_status ON orders.orders (status);
CREATE INDEX IF NOT EXISTS ix_orders_customer_id ON orders.orders (customer_id);
CREATE INDEX IF NOT EXISTS ix_orders_created_at ON orders.orders (created_at);
CREATE INDEX IF NOT EXISTS ix_order_lines_order_id ON orders.order_lines (order_id);
CREATE INDEX IF NOT EXISTS ix_order_lines_product_id ON orders.order_lines (product_id);
