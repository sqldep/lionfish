select 
  'CREATE VIEW '|| v.table_name || ' AS ' || v.view_definition as sourceCode,
  v.table_name as viewName,
  null as groupName,
  v.table_catalog as databaseName,
  v.table_schema as schemaName
from information_schema.views v
where v.table_schema not in ('pg_catalog', 'information_schema');