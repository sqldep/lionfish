-- all databases
select ora_database_name as Database from dual;

-- rest of SQL presumes 'default' is the name of the database

-- DDL 1/3 ("queries" in JSON): Function, procedure, package body, package
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

-- DDL 2/3 for views

select
	TO_LOB(v.text) SourceCode,
	v.VIEW_NAME as Name,
	v.OWNER||'.'||v.VIEW_NAME as GroupName,
	v.OWNER as Schema,
	'default' as Database
  from all_views v
  where instr(:c_prohibited_schema,v.OWNER) = 0; -- black list of oracle default schemas

-- DDL 3/3 for views

select
	TO_LOB(mv.QUERY) SourceCode,
	mv.MVIEW_NAME as Name,
	mv.OWNER||'.'||mv.MVIEW_NAME as GroupName,
	mv.OWNER as Schema,
	'default' as Database
  from all_mviews mv
  where instr(:c_prohibited_schema,mv.OWNER) = 0; -- black list of oracle default schemas

-- DbDef: ("databaseModel" in JSON) we use this "all in one" select to avoid nested multiply selection ... for table colums
select
  'default' as Database,
  x.owner as Schema,
  x.table_name as TableName,
  nvl2(w.object_name,'Y','N') as IsView, -- type materialized view is a view too
  x.column_name as ColumnName,
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
	  end /*||decode(x.nullable, 'N', ' NOT NULL')*/ as DataType,
  c.comments as Comments,  
  x.column_id as ColOrder
from all_tab_cols x,
	 all_col_comments c,
	(select 
	   owner,
	   object_name,
	   object_type
	 from all_objects
	 where object_type in ('MATERIALIZED VIEW', 'VIEW')) w, --  VIEWS
	(select
	   owner,
	   object_name,
	   object_type
	 from all_objects 
	 where  object_type in ('TABLE') ) t -- TABLES
where c.column_name = x.column_name -- joining cols and colls somments
  and c.owner =x.owner
  and c.table_name = x.table_name
  and x.owner =t.owner(+) -- outer join tables
  and x.table_name = t.object_name(+)
  and x.owner =w.owner(+) -- outer join views 
  and x.table_name = w.object_name(+) 
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
	,x.OWNER) = 0 -- black list of oracle default schemas
order by x.owner, x.table_name, x.column_id;


-- synonyms
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

-- dblinks
select
	dbl.owner as Owner,
	dbl.db_link as Name,
	dbl.username as UserName,
	dbl.host as Host
from ALL_DB_LINKS dbl
where instr(
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
	,dbl.owner) = 0; -- black list of oracle default schemas
