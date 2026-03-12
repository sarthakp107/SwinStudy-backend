using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwinStudy.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var baseDir = AppContext.BaseDirectory;

            SeedDegrees(migrationBuilder, Path.Combine(baseDir, "Data", "Seed", "degrees.csv"));
            SeedUnits(migrationBuilder, Path.Combine(baseDir, "Data", "Seed", "units.csv"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        private static void SeedDegrees(MigrationBuilder migrationBuilder, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found at '{path}'. Make sure it is included as Content and copied to output.");

            var rows = ReadCsvRows(path);
            const int chunkSize = 200;

            for (var i = 0; i < rows.Count; i += chunkSize)
            {
                var sb = new StringBuilder();
                sb.AppendLine("INSERT INTO all_degrees (degree_id, degree_name, degree_code, created_at) VALUES");

                var end = Math.Min(i + chunkSize, rows.Count);
                for (var j = i; j < end; j++)
                {
                    var r = rows[j];
                    // degree_id,degree_name,degree_code,created_at
                    var id = r[0];
                    var name = r[1];
                    var code = r.Length > 2 ? r[2] : "";
                    var createdAt = r.Length > 3 ? r[3] : "";

                    sb.Append('(')
                      .Append(id).Append(", ")
                      .Append(SqlQuote(name)).Append(", ")
                      .Append(string.IsNullOrWhiteSpace(code) ? "NULL" : SqlQuote(code)).Append(", ")
                      .Append(SqlQuote(createdAt))
                      .Append(')');

                    sb.AppendLine(j == end - 1 ? "" : ",");
                }

                sb.AppendLine("ON CONFLICT (degree_id) DO NOTHING;");
                migrationBuilder.Sql(sb.ToString());
            }

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('all_degrees','degree_id'), (SELECT COALESCE(MAX(degree_id), 1) FROM all_degrees));");
        }

        private static void SeedUnits(MigrationBuilder migrationBuilder, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found at '{path}'. Make sure it is included as Content and copied to output.");

            var rows = ReadCsvRows(path);
            const int chunkSize = 200;

            for (var i = 0; i < rows.Count; i += chunkSize)
            {
                var sb = new StringBuilder();
                sb.AppendLine("INSERT INTO all_units (unit_id, unit_name, unit_code, credit_points, created_at) VALUES");

                var end = Math.Min(i + chunkSize, rows.Count);
                for (var j = i; j < end; j++)
                {
                    var r = rows[j];
                    // unit_id,unit_name,unit_code,credit_points,created_at
                    var id = r[0];
                    var name = r[1];
                    var code = r.Length > 2 ? r[2] : "";
                    var creditPoints = r.Length > 3 ? r[3] : "";
                    var createdAt = r.Length > 4 ? r[4] : "";

                    sb.Append('(')
                      .Append(id).Append(", ")
                      .Append(SqlQuote(name)).Append(", ")
                      .Append(string.IsNullOrWhiteSpace(code) ? "NULL" : SqlQuote(code)).Append(", ");

                    if (string.IsNullOrWhiteSpace(creditPoints))
                    {
                        sb.Append("NULL");
                    }
                    else
                    {
                        if (!decimal.TryParse(creditPoints, NumberStyles.Number, CultureInfo.InvariantCulture, out var cp))
                            sb.Append("NULL");
                        else
                            sb.Append(cp.ToString(CultureInfo.InvariantCulture));
                    }

                    sb.Append(", ")
                      .Append(SqlQuote(createdAt))
                      .Append(')');

                    sb.AppendLine(j == end - 1 ? "" : ",");
                }

                sb.AppendLine("ON CONFLICT (unit_id) DO NOTHING;");
                migrationBuilder.Sql(sb.ToString());
            }

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('all_units','unit_id'), (SELECT COALESCE(MAX(unit_id), 1) FROM all_units));");
        }

        private static List<string[]> ReadCsvRows(string path)
        {
            var lines = File.ReadAllLines(path);
            var result = new List<string[]>(Math.Max(0, lines.Length - 1));

            // skip header
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                result.Add(ParseCsvLine(line));
            }

            return result;
        }

        // Minimal RFC4180-ish parsing (handles commas inside quotes and escaped quotes).
        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>(8);
            var sb = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == ',')
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            result.Add(sb.ToString());
            return result.ToArray();
        }

        private static string SqlQuote(string value) => $"'{value.Replace("'", "''")}'";
    }
}
