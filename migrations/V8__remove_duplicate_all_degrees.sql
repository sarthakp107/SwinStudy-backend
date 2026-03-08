DELETE FROM all_degrees
WHERE degree_id NOT IN (
    SELECT MIN(degree_id)
    FROM all_degrees
    GROUP BY degree_name
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_all_degrees_degree_name
ON all_degrees(degree_name);