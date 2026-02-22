CREATE TABLE IF NOT EXISTS all_units (
  unit_id BIGSERIAL PRIMARY KEY,
  unit_name TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

ALTER TABLE all_units
  ADD COLUMN IF NOT EXISTS unit_code TEXT,
  ADD COLUMN IF NOT EXISTS credit_points NUMERIC(5,2);

CREATE UNIQUE INDEX IF NOT EXISTS ux_all_units_code
  ON all_units (unit_code);

CREATE INDEX IF NOT EXISTS idx_all_units_name
  ON all_units (unit_name);