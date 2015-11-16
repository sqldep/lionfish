select
  src.TEXT as sourceCode,
  src.NAME as name,
  src.TYPE as groupName,
  'default' as databaseName
  src.OWNER as schemaName
from all_source src
where instr(
	-- blacklist of schemas to export
	'ADAMS
     ANONYMOUS
     AURORA$ORB$UNAUTHENTICATED
     AWR_STAGE
     BLAKE
     CLARK
     CLOTH
     CSMIG
     CTXSYS
     DBSNMP
     DEMO
     DIP
     DMSYS
     DSSYS
     EXFSYS
     HR
     JONES
     LBACSYS
     MDSYS
     OE
     ORACLE_OCM
     ORDPLUGINS
     ORDSYS
     OUTLN
     PAPER
     PERFSTAT
     SCOTT
     SH
     STEEL
     SYS
     SYSTEM
     TRACESVR
     TSMSYS
     WMSYS
     WOOD
     XDB'
	,src.OWNER) = 0-- black list of oracle default schemas
 	and src.TYPE in ('FUNCTION','PROCEDURE','PACKAGE BODY','PACKAGE')
	and lead(src.line) over (partition by src.OWNER,src.name,src.TYPE order by src.line) is null
order by src.OWNER,src.name,src.TYPE,src.line;

--split

select
	TO_LOB(v.text) sourceCode,
	v.VIEW_NAME as name,
	v.OWNER||'.'||v.VIEW_NAME as groupName,
	v.OWNER as schemaName,
	'default' as databaseName
  from all_views v
  where instr(:c_prohibited_schema,v.OWNER) = 0; -- black list of oracle default schemas

--split

select
	TO_LOB(mv.QUERY) sourceCode,
	mv.MVIEW_NAME as name,
	mv.OWNER||'.'||mv.MVIEW_NAME as groupName,
	mv.OWNER as schema,
	'default' as database
  from all_mviews mv
  where instr(:c_prohibited_schema,mv.OWNER) = 0; -- black list of oracle default schemas

