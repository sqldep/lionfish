-- Select a list of all views and tables
SELECT '##DBNAME##', tablekind, creatorname, tablename
FROM dbc.tablesv
WHERE (TABLEKIND = 'T' OR TABLEKIND = 'V') AND
    DatabaseName = '##DBNAME##'
-- AND tablename IN ('view_to_export', 'table_to_export')

--split

-- Select a list of all columns of tables and views
-- Together with the result of previous SELECT the result will be used to fill
-- "databaseModel" key in JSON
SELECT
'##DBNAME##',
TABLENAME,
TRIM(COLUMNNAME),
TRIM(COLUMNTYPE)||'('||TRIM(COLUMNNUM)||')'
FROM (
SELECT
DATABASENAME,
TABLENAME,
COLUMNNAME,
CASE
	WHEN COLUMNTYPE= 'A1' THEN 'ARRAY'
	WHEN COLUMNTYPE= 'AN' THEN 'MULTI-DIMENSIONAL ARRAY'
	WHEN COLUMNTYPE= 'AT' THEN 'TIME'
	WHEN COLUMNTYPE= 'BF' THEN 'BYTE'
	WHEN COLUMNTYPE= 'BO' THEN 'BLOB'
	WHEN COLUMNTYPE= 'BV' THEN 'VARBYTE'
	WHEN COLUMNTYPE= 'CF' THEN 'CHARACTER'
	WHEN COLUMNTYPE= 'CO' THEN 'CLOB'
	WHEN COLUMNTYPE= 'CV' THEN 'VARCHAR'
	WHEN COLUMNTYPE= 'D' THEN 'DECIMAL'
	WHEN COLUMNTYPE= 'DA' THEN 'DATE'
	WHEN COLUMNTYPE= 'DH' THEN 'INTERVAL DAY TO HOUR'
	WHEN COLUMNTYPE= 'DM' THEN 'INTERVAL DAY TO MINUTE'
	WHEN COLUMNTYPE= 'DS' THEN 'INTERVAL DAY TO SECOND'
	WHEN COLUMNTYPE= 'DY' THEN 'INTERVAL DAY'
	WHEN COLUMNTYPE= 'F' THEN 'FLOAT'
	WHEN COLUMNTYPE= 'HM' THEN 'INTERVAL HOUR TO MINUTE'
	WHEN COLUMNTYPE= 'HS' THEN 'INTERVAL HOUR TO SECOND'
	WHEN COLUMNTYPE= 'HR' THEN 'INTERVAL HOUR'
	WHEN COLUMNTYPE= 'I' THEN 'INTEGER'
	WHEN COLUMNTYPE= 'I1' THEN 'BYTEINT'
	WHEN COLUMNTYPE= 'I2' THEN 'SMALLINT'
	WHEN COLUMNTYPE= 'I8' THEN 'BIGINT'
	WHEN COLUMNTYPE= 'JN' THEN 'JSON'
	WHEN COLUMNTYPE= 'MI' THEN 'INTERVAL MINUTE'
	WHEN COLUMNTYPE= 'MO' THEN 'INTERVAL MONTH'
	WHEN COLUMNTYPE= 'MS' THEN 'INTERVAL MINUTE TO SECOND'
	WHEN COLUMNTYPE= 'N' THEN 'NUMBER'
	WHEN COLUMNTYPE= 'PD' THEN 'PERIOD(DATE)'
	WHEN COLUMNTYPE= 'PM' THEN 'PERIOD(TIMESTAMP WITH TIME ZONE)'
	WHEN COLUMNTYPE= 'PS' THEN 'PERIOD(TIMESTAMP)'
	WHEN COLUMNTYPE= 'PT' THEN 'PERIOD(TIME)'
	WHEN COLUMNTYPE= 'PZ' THEN 'PERIOD(TIME WITH TIME ZONE)'
	WHEN COLUMNTYPE= 'SC' THEN 'INTERVAL SECOND'
	WHEN COLUMNTYPE= 'SZ' THEN 'TIMESTAMP WITH TIME ZONE'
	WHEN COLUMNTYPE= 'TS' THEN 'TIMESTAMP'
	WHEN COLUMNTYPE= 'TZ' THEN 'TIME WITH TIME ZONE'
	WHEN COLUMNTYPE= 'UT' THEN 'UDT Type'
	WHEN COLUMNTYPE= 'XM' THEN 'XML'
	WHEN COLUMNTYPE= 'YM' THEN 'INTERVAL YEAR TO MONTH'
	WHEN COLUMNTYPE= 'YR' THEN 'INTERVAL YEAR'
	WHEN COLUMNTYPE= '++' THEN 'TD_ANYTYPE'
	ELSE COLUMNTYPE
END AS COLUMNTYPE,
CASE WHEN COLUMNTYPE='CF' THEN COLUMNLENGTH
     WHEN COLUMNTYPE='CV' THEN COLUMNLENGTH
     WHEN COLUMNTYPE='D' THEN (DECIMALTOTALDIGITS||','||DECIMALFRACTIONALDIGITS)
     WHEN COLUMNTYPE='TS' THEN COLUMNLENGTH
     WHEN COLUMNTYPE='I' THEN DECIMALTOTALDIGITS
     WHEN COLUMNTYPE='I2' THEN DECIMALTOTALDIGITS
     WHEN COLUMNTYPE='DA' THEN NULL
	ELSE NULL
END AS COLUMNNUM
FROM DBC.COLUMNSV
WHERE DATABASENAME='##DBNAME##'

-- AND tablename IN ('view_to_export', 'table_to_export')

) TBL;

--split

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