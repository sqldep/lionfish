-- procedury

SELECT '##DBNAME##', creatorname, tablename from dbc.tables WHERE TABLEKIND = 'P'


--split

SHOW PROCEDURE ##DBNAME##.##PROCEDURENAME##
