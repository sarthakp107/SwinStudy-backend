using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SwinStudy.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline existing tables created previously via Flyway.
            // Use IF NOT EXISTS so this migration is safe on databases
            // that already have the schema from the SQL migrations.
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS all_degrees (
  degree_id    BIGSERIAL PRIMARY KEY,
  degree_name  TEXT NOT NULL,
  degree_code  TEXT,
  created_at   TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_all_degrees_name_code
  ON all_degrees (degree_name, degree_code);

CREATE INDEX IF NOT EXISTS idx_all_degrees_degree_name
  ON all_degrees (degree_name);
");

            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS all_units (
  unit_id       BIGSERIAL PRIMARY KEY,
  unit_name     TEXT NOT NULL,
  created_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

ALTER TABLE all_units
  ADD COLUMN IF NOT EXISTS unit_code TEXT,
  ADD COLUMN IF NOT EXISTS credit_points NUMERIC(5,2);

CREATE UNIQUE INDEX IF NOT EXISTS ux_all_units_code
  ON all_units (unit_code);

CREATE INDEX IF NOT EXISTS idx_all_units_name
  ON all_units (unit_name);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP TABLE IF EXISTS all_degrees;
DROP TABLE IF EXISTS all_units;
");
        }
    }
}
