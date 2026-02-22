-- V2__flashcards_and_units.sql

-- 1) Stores saved flashcards per user (directly stores Q/A)
CREATE TABLE IF NOT EXISTS m_usersavedflashcards (
  m_usersavedflashcards_pkey BIGSERIAL PRIMARY KEY,
  userid      TEXT NOT NULL,
  question    TEXT NOT NULL,
  answer      TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Helps: getUserSavedFlashcard (WHERE userid = $1)
CREATE INDEX IF NOT EXISTS idx_m_usersavedflashcards_userid
  ON m_usersavedflashcards (userid);

-- Helps: getSpecificSavedFlashcard (WHERE userid AND question AND answer)
CREATE INDEX IF NOT EXISTS idx_m_usersavedflashcards_user_q_a
  ON m_usersavedflashcards (userid, question, answer);


-- 2) Master table of all flashcards ever created/generated
CREATE TABLE IF NOT EXISTS m_allflashcards (
  m_allflashcards_pkey BIGSERIAL PRIMARY KEY,
  question     TEXT NOT NULL,
  answer       TEXT NOT NULL,
  created_date TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Optional: avoid duplicate identical flashcards (enable if you want uniqueness)
-- CREATE UNIQUE INDEX IF NOT EXISTS ux_m_allflashcards_q_a
--   ON m_allflashcards (question, answer);


-- 3) Link table: user -> flashcard reference (for generated flashcards)
CREATE TABLE IF NOT EXISTS m_usergeneratedflashcards (
  m_usergeneratedflashcards_pkey BIGSERIAL PRIMARY KEY,
  userid       TEXT NOT NULL,
  qnareference BIGINT NOT NULL,
  created_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
  CONSTRAINT fk_m_usergeneratedflashcards_qnaref
    FOREIGN KEY (qnareference)
    REFERENCES m_allflashcards (m_allflashcards_pkey)
    ON DELETE CASCADE
);

-- Helps: getUserGeneratedFlashcards (WHERE m.userid = $1)
CREATE INDEX IF NOT EXISTS idx_m_usergeneratedflashcards_userid
  ON m_usergeneratedflashcards (userid);

-- Helps JOIN on qnareference
CREATE INDEX IF NOT EXISTS idx_m_usergeneratedflashcards_qnareference
  ON m_usergeneratedflashcards (qnareference);


-- 4) Units table (used by getAllUnits: SELECT * FROM all_units)
-- You didn’t show columns, so here’s a reasonable baseline.
-- Adjust columns to match your frontend expectations.
CREATE TABLE IF NOT EXISTS all_units (
  unit_id     BIGSERIAL PRIMARY KEY,
  unit_code   TEXT,
  unit_name   TEXT NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Optional helpful indexes:
CREATE INDEX IF NOT EXISTS idx_all_units_unit_name
  ON all_units (unit_name);

-- Optional uniqueness if you have stable codes:
-- CREATE UNIQUE INDEX IF NOT EXISTS ux_all_units_unit_code
--   ON all_units (unit_code);