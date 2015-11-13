select
  s.OWNER as Schema,
  s.SYNONYM_NAME as Name,
  s.TABLE_OWNER as SourceSchema,
  s.TABLE_NAME as SourceName,
  s.DB_LINK as SourceDbLinkName 
from all_synonyms s,
	 all_objects o
where s.table_owner = o.owner 
  and s.table_name = o.object_name
  and o.object_type in  
					  ('MATERIALIZED VIEW'
					  ,'TABLE'
					  ,'SYNONYM'
					  ,'VIEW') 
  and instr(
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
	,s.TABLE_OWNER) = 0;-- black list of oracle default schemas 
