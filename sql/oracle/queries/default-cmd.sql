/*
IMPORTANT: 
1) If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish). 
Only cmd.sql will be used when both exist.

2)  This file needs to contain exactly 3 queries divided by a splitter. 
If you do not wish to include the results from some queries, add '1=0' to WHERE condition
*/
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

/* Uncomment following line to narrow the export only to procedures                    */
/* AND src.NAME IN ('procedure_to _export')                                            */

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

/* Uncomment following line to narrow the export only to some views                    */
/* AND v.VIEW_NAME IN ('view_to _export')                                              */

--split

SELECT
	mv.QUERY sourceCode,
	mv.MVIEW_NAME as name,
	mv.OWNER||'.'||mv.MVIEW_NAME as groupName,
	'##DBNAME##' as databaseName,
	mv.OWNER as schemaName,
	c.columnList
FROM all_mviews mv
JOIN (
	SELECT OWNER, TABLE_NAME, listagg(COLUMN_NAME,',') within group(order by COLUMN_ID) columnList
	FROM all_tab_columns
	GROUP BY OWNER, TABLE_NAME
) c
ON c.OWNER = mv.OWNER AND c.TABLE_NAME = mv.MVIEW_NAME
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

/* Uncomment following line to narrow the export only to some views                    */
/* AND mv.MVIEW_NAME IN ('view_to _export')                                            */

