SELECT
  src.TEXT as sourceCode,
  src.NAME as name,
  src.TYPE as groupName,
  '##DBNAME##' as databaseName,
  src.OWNER as schemaName,
  src.line as lineNumber
FROM all_source src
WHERE
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
/* Uncomment following line to narrow the export only to some schemas                  */
/* AND src.OWNER IN ('schema_to _export')                                              */
ORDER BY src.OWNER,src.name,src.TYPE,src.line

--split

SELECT
	v.text sourceCode,
	v.VIEW_NAME as name,
	v.OWNER||'.'||v.VIEW_NAME as groupName,
	'##DBNAME##' as databaseName,
	v.OWNER as schemaName
FROM all_views v
WHERE v.OWNER NOT IN (
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
/* Uncomment following line to narrow the export only to some schemas                  */
/* AND v.OWNER IN ('schema_to _export')                                                */

--split

SELECT
	mv.QUERY sourceCode,
	mv.MVIEW_NAME as name,
	mv.OWNER||'.'||mv.MVIEW_NAME as groupName,
	'##DBNAME##' as databaseName,
	mv.OWNER as schemaName
FROM all_mviews mv
WHERE  mv.OWNER NOT IN (
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
/* Uncomment following line to narrow the export only to some schemas                  */
/* AND mv.OWNER IN ('schema_to _export')                                               */

