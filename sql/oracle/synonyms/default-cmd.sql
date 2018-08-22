/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/
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
					  ,'VIEW')
  and s.OWNER NOT IN (
	'APEX_040200',
	'CTXSYS',
	'DVF',
	'DVSYS',
	'GSMADMIN_INTERNAL',
	'LBACSYS',
	'MDSYS',
	'OLAPSYS',
	'ORCL',
	'ORDDATA',
	'ORDPLUGINS',
	'ORDSYS',
	'PUBLIC',
	'SI_INFORMTN_SCHEMA',
	'SYS',
	'SYSTEM',
	'WMSYS',
	'XDB'
	)

