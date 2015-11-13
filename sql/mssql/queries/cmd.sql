SELECT 
	d.definition as SourceCode
	p.name as Name,
	('##DBNAME##' + '.' + s.name) as GroupName,
	'##DBNAME##' as Database,
	s.name as Schema
FROM 
	[##DBNAME##].sys.procedures p
	INNER JOIN
	[##DBNAME##].sys.schemas s on p.schema_id = s.schema_id
	INNER JOIN
	[##DBNAME##].sys.sql_modules d on p.object_id = d.object_id

--split

SELECT
	d2.definition as SourceCode,
	v.name as Name,
	('##DBNAME##' + '.' + s2.name) as GroupName,
	'##DBNAME##' as Database,
	s.name as Schema
FROM
	[##DBNAME##].sys.views v
	INNER JOIN
		[##DBNAME##].sys.schemas s2 on v.schema_id = s2.schema_id
	INNER JOIN
		[##DBNAME##].sys.sql_modules d2 on v.object_id = d2.object_id


