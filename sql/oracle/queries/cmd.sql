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
order by src.OWNER,src.name,src.TYPE,src.line;

--split

select
	v.text sourceCode,
	v.VIEW_NAME as name,
	v.OWNER||'.'||v.VIEW_NAME as groupName,
	v.OWNER as schemaName,
	'##DBNAME##' as databaseName
from all_views v;

--split

select
	mv.QUERY sourceCode,
	mv.MVIEW_NAME as name,
	mv.OWNER||'.'||mv.MVIEW_NAME as groupName,
	mv.OWNER as schemaName,
	'##DBNAME##' as databaseName
from all_mviews mv;
