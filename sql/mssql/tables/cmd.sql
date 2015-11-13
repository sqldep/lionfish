SELECT
	'##DBNAME##' as database,
	s.name as schema,
	t.name as tableName,
	CASE WHEN t.type = 'U' THEN 'false' ELSE 'true' END as isView,
	c.name as columnName,
	tp.name + (CASE WHEN (CHARINDEX('char', tp.name) > 0) THEN '(' + CAST(c.max_length AS varchar(100)) + ')' ELSE '' END) as dataType,
	'' as Comment,
	c.column_id as colOrder
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

