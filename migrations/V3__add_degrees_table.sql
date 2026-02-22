-- V3__add_degrees_table.sql

CREATE TABLE IF NOT EXISTS all_degrees (
  degree_id    BIGSERIAL PRIMARY KEY,
  degree_name  TEXT NOT NULL,
  degree_code  TEXT,
  created_at   TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Unique per (name, code) so duplicates in name alone don't break seeding
CREATE UNIQUE INDEX IF NOT EXISTS ux_all_degrees_name_code
  ON all_degrees (degree_name, degree_code);

-- Helpful if you search by name
CREATE INDEX IF NOT EXISTS idx_all_degrees_degree_name
  ON all_degrees (degree_name);