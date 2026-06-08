-- Common setup for the local order_management database.
-- Safe to run more than once.

CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE SCHEMA IF NOT EXISTS orders;
CREATE SCHEMA IF NOT EXISTS stocks;
CREATE SCHEMA IF NOT EXISTS payments;
