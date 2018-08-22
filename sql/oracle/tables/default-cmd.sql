/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/
select
  '##DBNAME##' as databaseName,
  x.owner as schemaName,
  x.table_name as tableName,
  decode(obj.object_type, 'VIEW','Y','MATERIALIZED_VIEW','Y','N') as isView,
  x.column_name as columnName,
  x.data_type||
    case
      when x.data_type ='RAW' then '('|| x.data_length ||')'
      when x.data_precision is not null and nvl(x.data_scale,0)>0 then '('||x.data_precision||','||x.data_scale||')'
      when x.data_precision is not null and nvl(x.data_scale,0)=0 then '('||x.data_precision||')'
      when x.data_precision is null and x.data_scale is not null then '(38,'||x.data_scale||')'
      when x.char_length>0 then '('||x.char_length|| case x.char_used 
                                                       when 'B' then ' byte'
                                                       when 'C' then ' char'
                                                       else null 
                                                     end||')'
      end /*||decode(x.nullable, 'N', ' NOT NULL')*/ as dataType,
  c.comments as comments
from all_tab_cols x,
     all_col_comments c,
     all_objects obj
where 
  c.column_name = x.column_name -- joining cols and colls somments
  and c.owner =x.owner
  and c.table_name = x.table_name
  and x.owner =obj.owner
  and x.table_name = obj.object_name 
  and obj.object_type in ('TABLE','MATERIALIZED VIEW', 'VIEW')
  and x.owner NOT IN (
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
order by x.owner, x.table_name, isView, x.column_id
