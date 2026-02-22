CREATE TABLE IF NOT EXISTS unit_messages (
  id          BIGSERIAL PRIMARY KEY,
  unit_name   TEXT NOT NULL,
  sender      TEXT NOT NULL,
  message     TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS idx_unit_messages_unit_name_created_at
  ON unit_messages (unit_name, created_at);

CREATE TABLE IF NOT EXISTS indiv_messages (
  id          BIGSERIAL PRIMARY KEY,
  sender_id   TEXT NOT NULL,
  receiver_id TEXT NOT NULL,
  message     TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS idx_indiv_messages_pair_created_at
  ON indiv_messages (sender_id, receiver_id, created_at);