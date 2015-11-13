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

