/* Result of this query will be used to fill the databaseModel key in JSON         */
/* which contains a list of all tables and views and their columns.                */

SELECT
	'##DBNAME##' as mydatabase,
	s.name as myschema,
	t.name as tableName,
	CASE WHEN t.type = 'U' THEN 'false' ELSE 'true' END as isView,
	c.name as columnName,
	tp.name + (CASE WHEN (CHARINDEX('char', tp.name) > 0) THEN '(' + CAST(c.max_length AS varchar(100)) + ')' ELSE '' END) as dataType,
	'' as Comment
FROM
	[##DBNAME##].sys.objects t
	INNER JOIN
	[##DBNAME##].sys.schemas s on t.schema_id = s.schema_id
	INNER JOIN
	[##DBNAME##].sys.columns c on t.object_id = c.object_id
	INNER JOIN
	[##DBNAME##].sys.types tp on c.system_type_id = tp.system_type_id
WHERE
	t.type in ('U','V')
/* Uncomment following line to filter out tables or views:                         */
/* AND tableName LIKE '%MyPattern%'                                                */
ORDER BY
	myschema,
	tableName,
	isView,
	c.column_id;

