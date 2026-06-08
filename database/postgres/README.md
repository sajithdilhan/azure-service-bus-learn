# PostgreSQL Scripts

These scripts create the local tables for the `order_management` database.

Run them in order:

```sql
\i database/postgres/000_create_extensions_and_schemas.sql
\i database/postgres/001_create_orders_tables.sql
\i database/postgres/002_create_stocks_tables.sql
\i database/postgres/003_create_payments_tables.sql
```

Notes:

- Entity IDs are `uuid`. The app currently creates GUID v7 IDs, while the database defaults to `gen_random_uuid()` for manual inserts.
- Enum values are stored as readable strings, for example `Pending` and `Confirmed`, instead of integer values.
- `orders.orders` does not store `total_amount` because `Order.TotalAmount` is currently a computed C# property from order lines.
- `orders.order_lines` includes `order_id`, which should be added to the C# `OrderLine` entity when EF relationships are implemented.
- `payments.payments.order_id` is `uuid` to match `Order.Id` and the payment messages. The current `Payment.OrderId` C# property is still `int` and should be changed before EF mapping.
