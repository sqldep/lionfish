
SELECT
		d.name dbname
	FROM
		sys.databases d
	WHERE
		d.name not in ('master', 'model', 'msdb', 'tempdb', 'SSISDB')
	ORDER BY
		d.name;

-- procedure and view DDLs

GO--

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


-- UNION ALL .. zkus to prosim jeste jednou, jestli to ted neprojde

GO--

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


GO-- 



--details on table and view columns

SELECT
	'##DBNAME##' as Database,
	s.name as Schema,
	t.name as TableName,
	CASE WHEN t.type = 'U' THEN 'false' ELSE 'true' END as IsView,
	c.name as ColumnName,
	tp.name + (CASE WHEN (CHARINDEX('char', tp.name) > 0) THEN '(' + CAST(c.max_length AS varchar(100)) + ')' ELSE '' END) as DataType,
	'' as Comment,
	c.column_id as ColOrder
FROM
	[##DBNAME##].sys.objects t
	INNER JOIN
	[##DBNAME##].sys.schemas s on t.schema_id = s.schema_id
	INNER JOIN
	[##DBNAME##].sys.columns c on t.object_id = c.object_id
	INNER JOIN
	[##DBNAME##].sys.types tp on c.system_type_id = tp.system_type_id
WHERE
	t.type in ('U','V');

-- synonyms

GO--

SELECT
	'##DBNAME##' as Database,
	sch.name as Schema,
	s.name as Name,
	(CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) = 3
			THEN SUBSTRING(s.base_object_name,
			CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name) + 1) + 1,
			CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name) + 1) + 1) - CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name) + 1) - 1)
		  WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) = 2
			THEN SUBSTRING(s.base_object_name,
			CHARINDEX('.', s.base_object_name) + 1,
			CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name) + 1) - CHARINDEX('.', s.base_object_name) - 1)
		  WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) = 1
			THEN SUBSTRING(s.base_object_name,
							  1,
							 CHARINDEX('.', s.base_object_name) - 1)
		  ELSE ''
   END) as SourceSchema,
   (CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) > 0
		  THEN REVERSE(SUBSTRING(REVERSE(s.base_object_name),
							 1,
							 CHARINDEX('.', REVERSE(s.base_object_name)) - 1))
		  ELSE s.base_object_name
  END) as SourceName,
  (CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) = 3
				  THEN SUBSTRING(s.base_object_name, 1, CHARINDEX('.', s.base_object_name, CHARINDEX('.', s.base_object_name) + 1) - 1)
		WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, '.', ''))) = 2
				  THEN SUBSTRING(s.base_object_name, 1, CHARINDEX('.', s.base_object_name) - 1)
		ELSE ''
  END) as SourceDbLinkName
FROM
	[##DBNAME##].sys.synonyms s
	INNER JOIN
	[##DBNAME##].sys.schemas sch on s.schema_id = sch.schema_id;

