/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/

-- Select a list of all tables, views and procedures - the list will be used
-- for generating SHOW .... statements that fetcg corresponding source codes
-- that will be filled in the "queries" key in JSON
SELECT '##DBNAME##', creatorname, tablename, TABLEKIND
FROM dbc.tablesv
WHERE (
	TABLEKIND = 'P' OR
	TABLEKIND = 'V' OR
	TABLEKIND = 'M') AND
	DatabaseName = '##DBNAME##'

-- AND tablename IN ('view_to_export', 'table_to_export')