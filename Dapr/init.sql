CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY,
    customer_name VARCHAR(255) NOT NULL,
    total_amount DECIMAL(18,2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS dapr_outbox (
    id SERIAL PRIMARY KEY,
    state_id VARCHAR(255) NOT NULL,
    key VARCHAR(255) NOT NULL,
    data TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(state_id, key)
);

CREATE INDEX idx_dapr_outbox_created_at ON dapr_outbox(created_at);
