-- Creates the main 'payments' table.
-- 'IF NOT EXISTS' prevents an error if the script is run multiple times.
CREATE TABLE IF NOT EXISTS payments (
    -- BIGSERIAL is an 8-byte integer, safer for high-volume systems than a standard 4-byte SERIAL.
    id BIGSERIAL PRIMARY KEY,

    -- UUID to uniquely identify each payment request, preventing duplicates.
    correlation_id UUID NOT NULL,

    -- The payment amount.
    amount DECIMAL(10, 2) NOT NULL,

    -- The timestamp of the request, with timezone. Defaults to the current time.
    requested_at TIMESTAMPTZ NOT NULL DEFAULT (now()),

    -- The type of service used, based on the ENUM defined above.
    service_used INTEGER NOT NULL -- 1=Default, 2=Fallback
);

-- Creates a unique index on 'correlation_id' to enforce business rule of no duplicate payments.
-- 'CONCURRENTLY' builds the index without locking the table from writes.
-- 'IF NOT EXISTS' makes the operation idempotent.
CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS uq_correlation_id ON payments(correlation_id);

---

-- Creates a composite index to speed up the summary query ('GET /payments-summary').
-- This index is crucial for efficiently filtering by a date range.
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_payments_requested_at_service_used ON payments(requested_at, service_used);