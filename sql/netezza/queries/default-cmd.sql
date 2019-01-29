/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/



select 
  v.view_definition as sourceCode,
  v.table_name as viewName,
  null as groupName,
  v.table_catalog as databaseName,
  v.table_schema as schemaName
from "##DBNAME##".information_schema.views v
where upper(v.table_schema) not in ('INFORMATION_SCHEMA');