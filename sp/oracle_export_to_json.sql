/*********************************************************************
/* Export DB structures to JSON format
/* ver 1.02 20151028
/*
/* code works with system views: 
/*    all_col_comments
/*    all_mviews
/*    all_objects
/*    all_source
/*    all_synonyms
/*    all_tab_cols
/*    all_views
/* 
/* note: script can run for several minutes as accesing database catalog is slow
/* 
/********************************************************************/
declare
  --Constants
  c_meta_data_table constant varchar2(100) := 'SQLDEP_METADATA'; -- name for result table with JSON metadata
  c_prohibited_schema constant varchar2(32000) := -- prohibited schemas

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
     XDB';
  
  --Variables
  oclob clob;
  
  --Functions
  /* get_db_name - return DB name */
  function get_db_name return varchar2 is
    Result varchar2(1000);
  begin
    select sys_context('userenv','db_name') into Result from dual;
    return(Result);
  end get_db_name;
  
  /* get_curr_user_name - return current user name */
  function get_curr_user_name return varchar2 is
    Result varchar2(1000);
  begin
    select sys_context('userenv','current_user') into Result from dual;
    return(Result);
  end get_curr_user_name;
  
  /* get_curr_schema - return current schema */
  function get_curr_schema return varchar2 is
    Result varchar2(1000);
  begin
    select sys_context('userenv','current_schema') into Result from dual;
    return(Result);
  end get_curr_schema;
  
  /* escString - function escapes string to be in compliance with json specification */
  function escString(str varchar2) return varchar2 as
    sb varchar2(32767 char) := '';
    buf varchar2(40 char);
    num number;
  begin
    if(str is null) then 
      return ''; 
    end if;
    for i in 1 .. length(str) loop
      buf := substr(str, i, 1);
      case buf
      when chr( 8) then buf := '\b';--[Backspace]
      when chr( 9) then buf := '\t';--[Tab]
      when chr(10) then buf := '\n';--[Line Feed Return]
      when chr(12) then buf := '\f';--[Page Break]
      when chr(13) then buf := '\r';--[Carriage Return]
      when chr(34) then buf := '\"';--["]
      when chr(47) then if(true) then buf := '\/'; end if;--[/]
      when chr(92) then buf := '\\';--[\]
      else 
        if(ascii(buf) < 32) then --NonPrintingChars
          buf := '\u'||replace(substr(to_char(ascii(buf), 'XXXX'),2,4), ' ', '0');
        elsif (false)  then --  rewrite to true for escpaing everything not ascii 
          buf := replace(asciistr(buf), '\', '\u');
        end if;
      end case;
      sb := sb || buf;
    end loop;
    return sb;
  end escString;

  /* json_tag - write JSON tag name + value */
  function json_tag(av_tag_name varchar2, av_tag_value varchar2, add_separator boolean default true
                  , add_eol boolean default true) return varchar2 
  is
    v_value varchar2(32767 char) := '';  
  begin
    v_value := '"'||av_tag_name||'": "'||escString(av_tag_value) || '"' ;
    if (add_separator) then
      v_value := v_value || ',' ;
    end if ; 
    if ( add_eol ) then
      v_value := v_value ||  chr(13) || chr(10)  ;
    end if ; 
    return v_value ; 
  end json_tag; 
  
  /* json_tag - write JSON tag name + value - overloaded for boolean value*/
  ----------------------------------------------------------------------------------------------------------------------------------------------------
  function json_tag(av_tag_name varchar2, av_tag_value boolean, add_separator boolean default true, add_eol boolean default true) return varchar2
  is
    v_value varchar2(32767 char) := '' ;  
  begin
    v_value := json_tag(av_tag_name,case when av_tag_value then 'true' else 'false' end, add_separator, add_eol);
    return v_value; 
  end json_tag; 
  
  /* json_tag_begin */
  function json_tag_begin(av_tag_name varchar2) return varchar2 
  is
    v_value varchar2(32767 char) := '';
  begin
    v_value := '"'||av_tag_name||'": "';
    return v_value;
  end json_tag_begin;
  
  /* json_tag_begin - overloaded next argument*/
  function json_tag_begin(av_tag_name varchar2, av_tag_value varchar2) return varchar2 
  is
    v_value varchar2(32767 char) := '';
  begin
    v_value := json_tag_begin(av_tag_name)  ||escString(av_tag_value) ;
    return v_value ; 
  end json_tag_begin;
  
  /* json_tag_end */
  function json_tag_end(add_separator boolean default true, add_eol boolean default true) return varchar2 
  is
    v_value varchar2(30 char) := '"';  
  begin
    if (add_separator) then
      v_value := v_value || ',';
    end if;
    if (add_eol) then
      v_value := v_value || chr(13) || chr(10);
    end if ; 
    return v_value ; 
  end json_tag_end; 
  
  /* json_tag_end - overloaded next argument */
  function json_tag_end(av_tag_name varchar2, av_tag_value varchar2, 
                        add_separator boolean default true, add_eol boolean default true) return varchar2 
  is
    v_value varchar2(32767 char) := '';
  begin
    v_value := escString(av_tag_value) || '"' ; 
    if (add_separator) then
      v_value := v_value || ',';
    end if;
    if (add_eol) then
      v_value := v_value || chr(13) || chr(10);
    end if;
    return v_value;
  end json_tag_end; 
  
  /* append_json_value */
  function append_json_value(input_clob in out clob, av_tag_value_part varchar2) return clob
  is
  begin
    dbms_lob.append(input_clob,escString(av_tag_value_part));
    return input_clob;
  end append_json_value;
  
  --Procedures
  
    /* create_table - drop if table exists*/
  procedure create_table(t_owner varchar2,t_tab_name varchar2,t_part_ddl_code varchar2) is
  begin
    execute immediate 'drop table ' || t_owner || '.' || t_tab_name;
    execute immediate 'create table ' || t_owner || '.' || t_tab_name || ' ' || t_part_ddl_code;
    commit;
    exception when others then --Ignore error when table not exists
    execute immediate 'create table ' || t_owner || '.' || t_tab_name || ' ' || t_part_ddl_code;
    commit;
  end create_table;
  
  /* create_meta_data_table - create temp table for result in JSON format */
  procedure create_meta_data_table is
    table_owner varchar2(100) := get_curr_schema();
    table_name varchar2(100) := c_meta_data_table;
  begin
    create_table(get_curr_schema(),c_meta_data_table,
                 '(META_ID NUMBER NOT NULL,META_VALUE CLOB,CONSTRAINT '||c_meta_data_table||'_PK PRIMARY KEY(META_ID))');
  end create_meta_data_table;
  
  /* escapeClobAdd -  escpaing for clob */ 
  procedure escapeClobAdd(iclob clob ) is
    offset number := 1;
    amount number := 8000;
    len    number := DBMS_LOB.getLength(iclob);
    buf    varchar2(16000 char);
  begin
    if(iclob is null) then return ; end if;
    while offset < len loop
      dbms_lob.read(iclob, amount, offset, buf);
      offset := offset + amount + 1;
      dbms_lob.append(oclob, escString(buf));
    end loop;
  end escapeClobAdd;

  /* append_clob - append to clob */
  procedure append_clob(av_value varchar2 ) 
  is
  begin
    dbms_lob.append(oclob,av_value ) ; 
    --  if it is greater then 1 MB perform update and commit and reallocate new clob 
    --  we update and commit metadata can be veeery large so to avoid exhausing memmory consumtion 
    if (dbms_lob.getlength(oclob) > 1024*1024) then
      execute immediate 'update ' || get_curr_schema() || '.' || c_meta_data_table || ' set meta_value = meta_value || 
                        :oclob where meta_id = 1' using oclob; 
      commit ;
      dbms_lob.freetemporary(oclob) ;
      dbms_lob.createtemporary(oclob,true);
    end if ; 
  end append_clob;
  
  /* add_queries_body */
  procedure add_queries_body
  is
    n_counter number := 0 ;
    TYPE View_SRC_Typ IS RECORD (
      fullviewname VARCHAR2(100),
      owner        VARCHAR2(100),
      view_name    VARCHAR2(100),
      sourcecode   CLOB);
    View_SRC_Cur_rec View_SRC_Typ;
    view_src_cur sys_refcursor;
    query_str VARCHAR2(4000) :=
      'select
        v.FullViewName,
        v.OWNER,
        v.VIEW_NAME,
        v.SourceCode
      from SQLDEP_TMP_LONG_FIX v';
    v_sql_view varchar2(4000) :=
      q'[INSERT INTO SQLDEP_TMP_LONG_FIX 
      select
        v.OWNER||'.'||v.VIEW_NAME as FullViewName,
        v.OWNER,
        v.VIEW_NAME,
        TO_LOB(v.text) SourceCode
      from all_views v
      where instr(:c_prohibited_schema,v.OWNER) = 0-- black list of oracle default schemas
      ]';
    v_sql_mview varchar2(4000) :=
      q'[INSERT INTO SQLDEP_TMP_LONG_FIX 
      select
        mv.OWNER||'.'||mv.MVIEW_NAME as FullViewName,
        mv.OWNER,
        mv.MVIEW_NAME VIEW_NAME,
        TO_LOB(mv.QUERY) SourceCode
      from all_mviews mv
      where instr(:c_prohibited_schema,mv.OWNER) = 0-- black list of oracle default schemas
      ]';
  begin
    -- Views - source code
    create_table(get_curr_schema(),'SQLDEP_TMP_LONG_FIX',
                 '(fullviewname VARCHAR2(100),owner VARCHAR2(100),view_name VARCHAR2(100),sourcecode CLOB)');
    execute immediate v_sql_view using c_prohibited_schema;
    commit;
    open view_src_cur for query_str;
    loop
      fetch view_src_cur into View_SRC_Cur_rec;
      exit when view_src_cur%NOTFOUND;
      n_counter := n_counter + 1;
      if (n_counter > 1 ) then
        append_clob( ', ' || chr(13) || chr(10)); 
      end if ; 
      append_clob('{ ' );
      append_clob('"id" : ' || n_counter || ' ,'||chr(13)||chr(10));
      append_clob(json_tag_begin('sourceCode')); 
      escapeClobAdd('CREATE OR REPLACE FORCE VIEW ');
      escapeClobAdd(View_SRC_Cur_rec.FullViewName);
      escapeClobAdd(' as '||chr(10)||chr(13));
      escapeClobAdd(View_SRC_Cur_rec.SourceCode); 
      append_clob(json_tag_end(true,true));
      append_clob(json_tag('name', View_SRC_Cur_rec.VIEW_NAME)) ;
      append_clob(json_tag('groupName', View_SRC_Cur_rec.OWNER)) ;
      append_clob(json_tag('database', get_db_name())) ;
      append_clob(json_tag('schema', View_SRC_Cur_rec.OWNER)) ;
      append_clob(json_tag('executionUser', View_SRC_Cur_rec.OWNER, false,true)) ;
      append_clob('} ');
    end loop;
    -- Materialized views - source code
    create_table(get_curr_schema(),'SQLDEP_TMP_LONG_FIX',
                 '(fullviewname VARCHAR2(100),owner VARCHAR2(100),view_name VARCHAR2(100),sourcecode CLOB)');
    execute immediate v_sql_mview using c_prohibited_schema;
    commit;
    loop
      fetch view_src_cur into View_SRC_Cur_rec;
      exit when view_src_cur%NOTFOUND;
      n_counter := n_counter + 1;
      if (n_counter > 1 ) then
        append_clob( ', ' || chr(13) || chr(10)); 
      end if ; 
      append_clob('{ ' );
      append_clob('"id" : ' || n_counter || ' ,'||chr(13)||chr(10));
      append_clob(json_tag_begin('sourceCode')); 
      escapeClobAdd('CREATE MATERIALIZED VIEW ');
      escapeClobAdd(View_SRC_Cur_rec.FullViewName);
      escapeClobAdd(' as '||chr(10)||chr(13));
      escapeClobAdd(View_SRC_Cur_rec.sourcecode); 
      append_clob(json_tag_end(true,true));
      append_clob(json_tag('name', View_SRC_Cur_rec.VIEW_NAME)) ;
      append_clob(json_tag('groupName', View_SRC_Cur_rec.OWNER)) ;
      append_clob(json_tag('database', get_db_name())) ;
      append_clob(json_tag('schema', View_SRC_Cur_rec.OWNER)) ;
      append_clob(json_tag('executionUser', View_SRC_Cur_rec.OWNER, false,true)) ;
      append_clob('} ');
    end loop;
    execute immediate 'drop table SQLDEP_TMP_LONG_FIX';
    commit;
    -- Function, procedure, package body, package - source code
    for rec in (select
                  src.OWNER,
                  src.NAME,
                  src.TYPE,
                  src.LINE,
                  src.TEXT,
                  case when src.line = 1 
                       then 'Y' 
                       else 'N'
                  end as FL,
                  case when lead(src.line) over (partition by src.OWNER,src.name,src.TYPE order by src.line) is null
                       then 'Y'
                       else 'N'
                  end as LL
                from all_source src
                where instr(c_prohibited_schema,src.OWNER) = 0-- black list of oracle default schemas
                  and src.TYPE in ('FUNCTION','PROCEDURE','PACKAGE BODY','PACKAGE')
                order by src.OWNER,src.name,src.TYPE,src.line) 
    loop
      if (rec.fl = 'Y') then 
        n_counter := n_counter + 1;
        if (n_counter > 1 ) then
          append_clob( ', ' || chr(13) || chr(10)); 
        end if ; 
        append_clob('{ ' );
        append_clob('"id" : ' || n_counter || ' ,'||chr(13)||chr(10));
        append_clob(json_tag_begin('sourceCode')); 
        escapeClobAdd(rec.text);
      else
         escapeClobAdd(rec.text);
      end if;
      if (rec.ll = 'Y') then
        append_clob(json_tag_end(true,true));
        append_clob(json_tag('name', rec.name)) ;
        append_clob(json_tag('groupName', rec.type)) ;
        append_clob(json_tag('database', get_db_name())) ;
        append_clob(json_tag('schema', rec.OWNER)) ;
        append_clob(json_tag('executionUser', rec.OWNER, false,true)) ;
        append_clob('} ');
      end if;
    end loop;    
  end add_queries_body;
  
  /* add_tables */
  procedure add_tables
  is
    n_counter number := 0 ; 
  begin
    --we use this "all in one" select to avoid nested multiply selection ... for table colums
    for rec in
    (
    select
      x.column_name,
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
          end /*||decode(x.nullable, 'N', ' NOT NULL')*/ as xdata_type,
      c.comments,  
      nvl2(w.object_name,'Y','N') as is_view, -- type materialized view is a view too
      x.table_name as object_name,
      x.owner,
      x.column_id, 
      max (x.column_id) over ( partition by x.owner, x.table_name ) as max_column_id,
      min (x.column_id) over ( partition by x.owner, x.table_name ) as min_column_id, 
      count(*) over() as total_count 
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
      and instr(c_prohibited_schema,x.OWNER) = 0 -- black list of oracle default schemas
    order by x.owner, x.table_name, x.column_id
    )
    loop
      n_counter := n_counter + 1 ; 
      if ( rec.column_id = rec.min_column_id ) then
        --  we add new table  ---  we or on the first column for the table
        append_clob('{ ') ; --- opening table
        append_clob(json_tag('schema', rec.owner)) ;
        append_clob(json_tag('name', rec.object_name)) ;
        if (rec.is_view = 'Y') then
          append_clob(json_tag('isView', true)) ;
        else
          append_clob(json_tag('isView', false)) ;
        end if ;
        append_clob('"columns": [ ') ; -- opening column list
      end if;  
      --- we add new column 
      append_clob('{ '); 
      append_clob(json_tag('name', rec.column_name)) ;
      append_clob(json_tag('dataType', rec.xdata_type)) ;
      append_clob(json_tag('comment', rec.comments, false,true)) ;
      --- 
      if ( rec.column_id = rec.max_column_id ) then -- last column for table we close all table
        append_clob(' } ') ;   -- closing one column wihout comma
        append_clob( ' ] ' ) ; --  closing columns list
        append_clob( ' } ' ) ; --  closing table 
        if (rec.total_count <> n_counter)  then
          append_clob( ',' ) ; --  we continue with new table  so add a comma   
        end if; 
      else -- normal  close one column with comma
        append_clob('}, ') ;   
      end if;     
      --
    end loop ;
  end add_tables;
  
  /* add_synonyms */
  procedure add_synonyms
  is
    n_counter number := 0 ; 
  begin
    for rec in  
    (
    select
      s.OWNER,
      s.SYNONYM_NAME,
      s.TABLE_OWNER,
      s.TABLE_NAME,
      s.DB_LINK 
    from all_synonyms s,
         all_objects o
    where s.table_owner = o.owner 
      and s.table_name = o.object_name
      and o.object_type in  
                          ('MATERIALIZED VIEW'
                          ,'TABLE'
                          ,'SYNONYM'
                          ,'VIEW') 
      and instr(c_prohibited_schema,s.TABLE_OWNER) = 0-- black list of oracle default schemas 
    )
    loop
      n_counter := n_counter + 1; 
      if (n_counter > 1 ) then
        append_clob(', ' || chr(13) || chr(10)); 
      end if; 
      append_clob('{ ' ); 
      append_clob(json_tag('schema', rec.owner));
      append_clob(json_tag('name', rec.synonym_name));
      append_clob(json_tag('sourceName', rec.table_name));
      append_clob(json_tag('sourceSchema', rec.table_owner));
      append_clob(json_tag('sourceDbLinkName', rec.db_link, false,true ));
      append_clob(' } '); 
    end loop;
  end add_synonyms;

  /* add_dblinks */
  procedure add_dblinks
  is
    n_counter number := 0 ; 
  begin
    for rec in  
    (
      select
        dbl.owner,
        dbl.db_link,
        dbl.username,
        dbl.host 
      from ALL_DB_LINKS dbl
      where instr(c_prohibited_schema,dbl.owner) = 0-- black list of oracle default schemas
    )
    loop
      n_counter := n_counter + 1; 
      if (n_counter > 1 ) then
        append_clob(', ' || chr(13) || chr(10)); 
      end if; 
      append_clob('{ ' ); 
      append_clob(json_tag('owner', rec.owner));
      append_clob(json_tag('name', rec.db_link));
      append_clob(json_tag('userName', rec.username));
      append_clob(json_tag('host', rec.host, false,true));
      append_clob(' } '); 
    end loop;
  end add_dblinks;
begin
  /* Main code*/
  dbms_lob.createtemporary(oclob,true);
  create_meta_data_table;
  execute immediate 'insert into ' || get_curr_schema() || '.' || c_meta_data_table || '(meta_id, meta_value) values (1,:oclob )' using oclob;
  commit;
  append_clob( '{ ') ; --TOP LEVEL
  append_clob( json_tag('userAccountId', '7f3e205e-34c9-4791-a343-83988b230d0e')) ; --TODO rewrite correct ID
  append_clob( json_tag('createdBy', 'oracle_export_to_json.sql')) ;
  append_clob( json_tag('dialect', 'oracle')) ;
  append_clob( json_tag('customSqlSetName', '')) ;
  append_clob( '"queries" : [ ') ;
  add_queries_body();
  append_clob( '  ], ') ;
  append_clob('"databaseModel" : {') ;
  append_clob('"databases" : [') ;
  append_clob(' {') ;
  append_clob(json_tag('name', get_db_name())) ;
  append_clob(' "tables": [ ') ;
  add_tables();
  append_clob( '  ], ') ; -- END tables
  append_clob( ' "synonyms" : [') ; 
  add_synonyms() ;
  append_clob( '  ], ') ; -- END synonyms
  append_clob( ' "dblinks" : [') ; 
  add_dblinks() ;
  append_clob(' ]') ; --    END dblinks
  append_clob(' }');  --   END one database
  append_clob(' ]');  --  END databases array
  append_clob(' }' ); -- END database model
  append_clob(' }');  -- TOP LEVEL
  
  execute immediate 'update ' || get_curr_schema() || '.' || c_meta_data_table || ' set meta_value = meta_value || 
                    :oclob where meta_id = 1' using oclob;
  commit;
end;
