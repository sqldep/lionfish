/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/

use database ##DBNAME##;

select
  t.table_catalog as dbName,
  t.table_schema as schemaName,
  t.table_name as tableName,
  case when t.table_type = 'VIEW' then 'Y'
       when t.table_type = 'BASE TABLE' then 'N'
       else t.table_type
  end as isView,
  c.column_name,
  c.data_type,
  null as comments
from
  information_schema.tables t,
  information_schema.columns c
where
  t.table_catalog = c.table_catalog
  and t.table_schema = c.table_schema
  and t.table_name = c.table_name
  and t.table_schema not in ('pg_catalog', 'information_schema')
order by t.table_catalog, t.table_schema, t.table_name, c.ordinal_position;