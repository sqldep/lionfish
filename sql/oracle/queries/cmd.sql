select
  src.TEXT as sourceCode,
  src.NAME as name,
  src.TYPE as groupName,
  '##DBNAME##' as databaseName,
  src.OWNER as schemaName,
  src.line as lineNumber
from all_source src
where
  src.TYPE in ('FUNCTION','PROCEDURE','PACKAGE BODY','PACKAGE')
  and src.OWNER NOT IN (
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
order by src.OWNER,src.name,src.TYPE,src.line

--split

select
	v.text sourceCode,
	v.VIEW_NAME as name,
	v.OWNER||'.'||v.VIEW_NAME as groupName,
	v.OWNER as schemaName,
	'##DBNAME##' as databaseName
from all_views v
where v.OWNER NOT IN (
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

--split

select
	mv.QUERY sourceCode,
	mv.MVIEW_NAME as name,
	mv.OWNER||'.'||mv.MVIEW_NAME as groupName,
	mv.OWNER as schemaName,
	'##DBNAME##' as databaseName
from all_mviews mv
where  mv.OWNER NOT IN (
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

