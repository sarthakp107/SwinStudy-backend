using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwinStudy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLegacyFlashcardTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS m_allflashcards (
  m_allflashcards_pkey BIGSERIAL PRIMARY KEY,
  question TEXT NOT NULL,
  answer TEXT NOT NULL,
  created_date TIMESTAMPTZ NOT NULL DEFAULT now()
);
");

            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS m_usersavedflashcards (
  m_usersavedflashcards_pkey BIGSERIAL PRIMARY KEY,
  userid TEXT NOT NULL,
  question TEXT NOT NULL,
  answer TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
");

            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS m_usergeneratedflashcards (
  m_usergeneratedflashcards_pkey BIGSERIAL PRIMARY KEY,
  userid TEXT NOT NULL,
  qnareference BIGINT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  CONSTRAINT fk_m_usergeneratedflashcards_qnaref
    FOREIGN KEY (qnareference)
    REFERENCES m_allflashcards (m_allflashcards_pkey)
    ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_m_usergeneratedflashcards_qnareference
  ON m_usergeneratedflashcards (qnareference);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DROP TABLE IF EXISTS m_usergeneratedflashcards;
DROP TABLE IF EXISTS m_usersavedflashcards;
DROP TABLE IF EXISTS m_allflashcards;
");
        }
    }
}
