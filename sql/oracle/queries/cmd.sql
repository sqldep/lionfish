select
  src.TEXT as SourceCode,
  src.NAME as Name,
  src.TYPE as GroupName,
  'default' as Database
  src.OWNER as Schema
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
	TO_LOB(v.text) SourceCode,
	v.VIEW_NAME as Name,
	v.OWNER||'.'||v.VIEW_NAME as GroupName,
	v.OWNER as Schema,
	'default' as Database
  from all_views v
  where instr(:c_prohibited_schema,v.OWNER) = 0; -- black list of oracle default schemas

--split

select
	TO_LOB(mv.QUERY) SourceCode,
	mv.MVIEW_NAME as Name,
	mv.OWNER||'.'||mv.MVIEW_NAME as GroupName,
	mv.OWNER as Schema,
	'default' as Database
  from all_mviews mv
  where instr(:c_prohibited_schema,mv.OWNER) = 0; -- black list of oracle default schemas

