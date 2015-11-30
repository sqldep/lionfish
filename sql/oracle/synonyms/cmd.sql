select
  '##DBNAME##' as databaseName,
  s.OWNER as schemaName,
  s.SYNONYM_NAME as name,
  s.TABLE_OWNER as sourceSchema,
  s.TABLE_NAME as sourceName,
  s.DB_LINK as sourceDbLinkName 
from all_synonyms s,
	 all_objects o
where s.table_owner = o.owner 
  and s.table_name = o.object_name
  and o.object_type in  
					  ('MATERIALIZED VIEW'
					  ,'TABLE'
					  ,'SYNONYM'
					  ,'VIEW');
