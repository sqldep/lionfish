drop procedure sp_extractMetaData;

go

create procedure sp_extractMetaData
	@UserAccountId nvarchar(max) = N'',
	@CustomSqlSetName nvarchar(max) = N'',
	@JSON nvarchar(max) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@DBName nvarchar(max), 
		@SQLQuery nvarchar(max),
		@NewLine nvarchar(10);

	SET @NewLine = CHAR(10);

	-- The #AllQueries temporary table stores views' and procedures' definitions 
	-- extracted from all databases on the server
	create table #AllQueries (
		Id			int identity(1,1) not null,
		Name		nvarchar(max) not null,
		GroupName	nvarchar(max) not null,
		SourceCode	nvarchar(max) not null
	);

	-- The #AllDbModels temporary table stores tables' and views' definitions 
	-- extracted from all databases on the server
	create table #AllDbModels (
		DbName		nvarchar(max) not null,
		SchemaName	nvarchar(max) not null,
		TabName		nvarchar(max) not null,
		IsView		nvarchar(5)	  not null,
		ColName		nvarchar(max) not null,
		DType		nvarchar(max) not null,
		Comment		nvarchar(max) not null,
		ColOrder	integer not null
	);

	-- The #AllSynonyms temporary table stores synonym definitions 
	-- extracted from all databases on the server
	create table #AllSynonyms (
		DbName		nvarchar(max) not null,
		SchemaName	nvarchar(max) not null,
		SynName		nvarchar(max) not null,
		SourceSchema	nvarchar(max) not null,
		SourceName	nvarchar(max) not null,
		SourceDbLinkName nvarchar(max) not null
	)

	-- The #RequestTable stores rows of the JSON request
	create table #RequestTable (
		LineNum bigint identity(1,1) not null,
		Payload nvarchar(max) not null
	);

	-- Iterate over all databases on the server

	DECLARE db_cursor CURSOR FOR 
	SELECT 
		d.name dbname
	FROM 
		sys.databases d
	WHERE 
		d.name not in ('master', 'model', 'msdb', 'tempdb', 'SSISDB')
	ORDER BY 
		d.name;

	OPEN db_cursor

	FETCH NEXT FROM db_cursor 
		INTO @DBName;
		
	WHILE @@FETCH_STATUS = 0
	BEGIN
	-- Build a query which pulls procedure and view DDLs from the current database 
	-- and stores the results in #AllQueries
		SET @SQLQuery = N'select ' + @NewLine +
						N'   p.name as Name,' + @NewLine +
						N'   (''' + @DBName + '.'' + s.name) as GroupName,' + @NewLine +
						N'   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(d.definition, ''\'', ''\\''), ''"'', ''\"''), CHAR(13), ''''), CHAR(10), ''\n''), CHAR(9), ''\t'') as SourceCode' + @NewLine +
						N'from' + @NewLine +
						N'   ' + @DBName + '.sys.procedures p'+ @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.schemas s on p.schema_id = s.schema_id' + @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.sql_modules d on p.object_id = d.object_id' + @NewLine +
						N'union all' + @NewLine +
						N'select ' + @NewLine +
						N'   v.name as Name,' + @NewLine +
						N'   (''' + @DBName + '.'' + s2.name) as GroupName,' + @NewLine +
						N'    REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(d2.definition, ''\'', ''\\''), ''"'', ''\"''), CHAR(13), ''''), CHAR(10), ''\n''), CHAR(9), ''\t'') as SourceCode' + @NewLine +
						N'from' + @NewLine +
						N'   ' + @DBName + '.sys.views v'+ @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.schemas s2 on v.schema_id = s2.schema_id' + @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.sql_modules d2 on v.object_id = d2.object_id' + @NewLine;

		INSERT INTO  #AllQueries (Name, GroupName, SourceCode)
		EXEC sp_executesql @SQLQuery;

		-- Build a query which pulls details on table and view columns from the current database 
		-- and stores the results in #AllDbModels
		SET @SQLQuery = N'select ' + @NewLine +
						N'   ''' + @DBName + ''' as DbName,' + @NewLine + 
						N'   s.name as SchemaName,' + @NewLine +
						N'   t.name as TabName,' + @NewLine +
						N'   CASE WHEN t.type = ''U'' THEN ''false'' ELSE ''true'' END as IsView,' + @NewLine +
						N'   c.name as ColName,' + @NewLine +
						N'   tp.name + (CASE WHEN (CHARINDEX(''char'', tp.name) > 0) THEN ''('' + CAST(c.max_length AS varchar(100)) + '')'' ELSE '''' END) as DType,' + @NewLine +
						N'   '''' as Comment,' + @NewLine +
						N'   c.column_id as ColOrder' + @NewLine +
						N'from' + @NewLine +
						N'   ' + @DBName + '.sys.objects t' + @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.schemas s on t.schema_id = s.schema_id' + @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.columns c on t.object_id = c.object_id' + @NewLine +
						N'   inner join ' + @NewLine +
						N'   ' + @DBName + '.sys.types tp on c.system_type_id = tp.system_type_id' + @NewLine + 
						N'where' + @NewLine +
						N'   t.type in (''U'',''V'')';

		INSERT INTO  #AllDbModels (DbName, SchemaName, TabName, IsView, ColName, DType, Comment, ColOrder)
		EXEC sp_executesql @SQLQuery;

		-- Build a query which pulls details on synonyms from the current database 
		-- and stores the results in #AllSynonyms

		SET @SQLQuery = N'select ' + @NewLine +
						N'   ''' + @DBName + ''' as DbName,' + @NewLine +
						N'   sch.name as SchemaName,' + @NewLine +
						N'   s.name as SynName,' + @NewLine +
						N'   (CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) = 3' + @NewLine +
						N'					THEN SUBSTRING(s.base_object_name, ' + @NewLine +
						N'								CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name) + 1) + 1,' + @NewLine +
						N'								CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name) + 1) + 1) - CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name) + 1) - 1)' + @NewLine +
						N'			WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) = 2' + @NewLine +
						N'					THEN SUBSTRING(s.base_object_name, ' + @NewLine +
						N'								CHARINDEX(''.'', s.base_object_name) + 1,' + @NewLine +
						N'								CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name) + 1) - CHARINDEX(''.'', s.base_object_name) - 1)' + @NewLine +
						N'			WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) = 1' + @NewLine +
						N'					THEN SUBSTRING(s.base_object_name,' + @NewLine +
						N'								1,' + @NewLine +
						N'								CHARINDEX(''.'', s.base_object_name) - 1)' + @NewLine +
						N'			ELSE ''''' + @NewLine +
						N'   END) as sourceSchema,' + @NewLine +
						N'	(CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) > 0' + @NewLine +
						N'					THEN REVERSE(SUBSTRING(REVERSE(s.base_object_name), ' + @NewLine +
						N'								1, ' + @NewLine +
						N'								CHARINDEX(''.'', REVERSE(s.base_object_name)) - 1)) ' + @NewLine +
						N'	    	ELSE s.base_object_name ' + @NewLine +
						N'	END) as SourceName,' + @NewLine +
						N'	(CASE WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) = 3 ' + @NewLine +
						N'					THEN SUBSTRING(s.base_object_name, 1, CHARINDEX(''.'', s.base_object_name, CHARINDEX(''.'', s.base_object_name) + 1) - 1)' + @NewLine +
						N'  		WHEN (len(s.base_object_name) - len(REPLACE(s.base_object_name, ''.'', ''''))) = 2' + @NewLine +
						N'					THEN SUBSTRING(s.base_object_name, 1, CHARINDEX(''.'', s.base_object_name) - 1)' + @NewLine +
						N'			ELSE ''''' + @NewLine +
						N'	END) as sourceDbLinkName' + @NewLine +
						N'from' + @NewLine +
						N'   ' + @DBName + '.sys.synonyms s' + @NewLine +
						N'   inner join' + @NewLine +
						N'   ' + @DBName + '.sys.schemas sch on s.schema_id = sch.schema_id';
		INSERT INTO  #AllSynonyms (DbName, SchemaName, SynName, SourceSchema, SourceName, SourceDbLinkName)
		EXEC sp_executesql @SQLQuery;
		
		FETCH NEXT FROM db_cursor 
			INTO @DBName;
	END

	CLOSE db_cursor;
	DEALLOCATE db_cursor;

	--PRINT 'Building the JSON request';

	-- Build the JSON request
	insert into #RequestTable values (N'{' + @NewLine);
	insert into #RequestTable values (N'   "userAccountId": "' + @UserAccountId + '",' + @NewLine); -- // * Required; will be provided to you by SQLdep team
	insert into #RequestTable values (N'   "dialect": "mssql",' + @NewLine); --                        // * Required; options: oracle/postgres/mssql/redshift/..
	insert into #RequestTable values (N'   "customSqlSetName": "' + @CustomSqlSetName + '",' + @NewLine); --  // Optional; when empty hash value will be generated; allowed characters [a-Z0-9_]
	insert into #RequestTable values (N'   "queries" : [' + @NewLine);

	-- First build the queries section of the request
	DECLARE
		@QueryId int,
		@QueryName nvarchar(max),
		@GroupName nvarchar(max),
		@SourceCode nvarchar(max),
		@QueryCnt int;

	SET @QueryCnt = 0;
		
	-- Iterate over all queries in the #AllQueries temp table 
	DECLARE query_cursor CURSOR FOR 
	SELECT 
		Id,
		Name,
		GroupName,
		SourceCode
	FROM 
		#AllQueries
	ORDER BY 
		GroupName,
		Name;

	OPEN query_cursor

	FETCH NEXT FROM query_cursor 
		INTO @QueryId, @QueryName, @GroupName, @SourceCode;
		
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		insert into #RequestTable values (CASE WHEN @QueryCnt > 0 THEN N',' + @NewLine ELSE N'' END);
		insert into #RequestTable values (N'      {' + @NewLine);
		insert into #RequestTable values (N'         "id": ' + CAST(@QueryId AS NVARCHAR(max)) + N',' + @NewLine); -- // * Required; must be unique across all queries
		insert into #RequestTable values (N'         "sourceCode": "' + @SourceCode + N'",' + @NewLine); --     // * Required; a) must be valid SQL command without futher editing
		insert into #RequestTable values (N'         "processStatusCode": "READY",' + @NewLine); --            // Optional; when empty READY will be used
		insert into #RequestTable values (N'         "name": "' + @QueryName + '",' + @NewLine); --            // Optional; for visualization purposes - use your internal name if available
		insert into #RequestTable values (N'         "groupName": "' + @GroupName + '",' + @NewLine); --       // Optional; for visualization purposes - use your internal name if available
		insert into #RequestTable values (N'         "executionUser": ""' + @NewLine); --                      // Optional; When empty DEFAULT will be used
		insert into #RequestTable values (N'      }');
		SET @QueryCnt = @QueryCnt + 1;

		FETCH NEXT FROM query_cursor 
				INTO @QueryId, @QueryName, @GroupName, @SourceCode;
	END

	CLOSE query_cursor;
	DEALLOCATE query_cursor;

	-- Now that we're done with the queries, build the databaseModel section

	insert into #RequestTable values (@NewLine);
	insert into #RequestTable values (N'   ],' + @NewLine);
	insert into #RequestTable values (N'   "databaseModel" : {' + @NewLine); --  // Optional; you can omit this section completely
	insert into #RequestTable values (N'      "databases" : [' + @NewLine);

	DECLARE
		@SchemaName	nvarchar(max),
		@TabName nvarchar(max),
		@IsView nvarchar(5),
		@ColName nvarchar(max),
		@DType nvarchar(max),
		@Comment nvarchar(max),
		@CurrentDb nvarchar(max),
		@CurrentSchema nvarchar(max),
		@CurrentTable nvarchar(max),
		@DbCnt int,
		@TableCnt int,
		@ColCnt int,
		@SynSchemaName nvarchar(max),
		@SynName nvarchar(max),
		@SynSourceSchema nvarchar(max),
		@SynSourceName nvarchar(max),
		@SynSourceDbLinkName nvarchar(max),
		@SynCnt int;

	SET @CurrentDb = N'';
	SET @CurrentSchema = N'';
	SET @CurrentTable = N'';
	SET @DbCnt = 0;
	SET @TableCnt = 0;
	SET @ColCnt = 0;

	-- Iterate over all columns (stored in rows) of the #AllDbModels temp table 
	DECLARE col_cursor CURSOR FOR 
	SELECT 
		DbName,
		SchemaName,
		TabName,
		IsView,
		ColName,
		DType,
		Comment
	FROM 
		#AllDbModels
	ORDER BY 
		DbName,
		SchemaName,
		TabName,
		ColOrder;

	OPEN col_cursor

	FETCH NEXT FROM col_cursor 
		INTO @DBName, @SchemaName, @TabName, @IsView, @ColName, @DType, @Comment;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		IF (@ColCnt > 0 AND (@DBName <> @CurrentDb OR @SchemaName <> @CurrentSchema OR @TabName <> @CurrentTable))
		BEGIN
			-- close a definition of the current table
			insert into #RequestTable values (N' ' + @NewLine);
			insert into #RequestTable values (N'                        ]' + @NewLine);
			insert into #RequestTable values (N'                    }');
			SET @ColCnt = 0;
		END
		IF (@TableCnt > 0 AND (@DBName <> @CurrentDb))
		BEGIN
			-- close a definition of the current database
			insert into #RequestTable values (@NewLine);
			insert into #RequestTable values (N'                ],' + @NewLine);

			-- add the synonyms 
			insert into #RequestTable values (N'                "synonyms" : [' + @NewLine);
			-- iterate over all synonyms in this database
			SET @SynCnt = 0;
			DECLARE syn_cursor CURSOR FOR 
			SELECT 		
				SchemaName,
				SynName,
				SourceSchema,
				SourceName,
				SourceDbLinkName
			FROM 
				#AllSynonyms
			WHERE 
				DbName = @CurrentDb
			ORDER BY 
				SchemaName,
				SynName;

			OPEN syn_cursor

			FETCH NEXT FROM syn_cursor 
				INTO @SynSchemaName, @SynName, @SynSourceSchema, @SynSourceName, @SynSourceDbLinkName;

			WHILE @@FETCH_STATUS = 0
			BEGIN 
				IF @SynCnt > 0 
					insert into #RequestTable values (N',' + @NewLine);
				insert into #RequestTable values (N'                    {' + @NewLine);
				insert into #RequestTable values (N'                        "schema": "' + @SynSchemaName + '",' + @NewLine);
				insert into #RequestTable values (N'                        "name": "' + @SynName + '",' + @NewLine);
				insert into #RequestTable values (N'                        "sourceSchema": "' + @SynSourceSchema + '",' + @NewLine);
				insert into #RequestTable values (N'                        "sourceName": "' + @SynSourceName + '",' + @NewLine);
				insert into #RequestTable values (N'                        "sourceDbLinkName": "' + @SynSourceDbLinkName + '"' + @NewLine);
				insert into #RequestTable values (N'                    }');

				SET @SynCnt = @SynCnt + 1;

				FETCH NEXT FROM syn_cursor 
				INTO @SynSchemaName, @SynName, @SynSourceSchema, @SynSourceName, @SynSourceDbLinkName;
			END

			CLOSE syn_cursor;
			DEALLOCATE syn_cursor;

			insert into #RequestTable values (@NewLine);
			insert into #RequestTable values (N'                ]' + @NewLine);
			insert into #RequestTable values (N'            }');

			SET @TableCnt = 0;
		END
		IF @DBName <> @CurrentDb
		BEGIN
			-- open a definition of a new database
			IF @DbCnt > 0
			BEGIN
				insert into #RequestTable values (N',' + @NewLine);
			END
			insert into #RequestTable values (N'            {' + @NewLine); 
			insert into #RequestTable values (N'                "name": "' + @DBName + '",' + @NewLine); -- // * Required; name of the database, avoid using duplicate names (only the first occurrence is going to be processed)
			insert into #RequestTable values (N'                "tables": [' + @NewLine);
			SET @CurrentDb = @DBName;
			SET @TableCnt = 0;
			SET @ColCnt = 0;
			SET @DbCnt = @DbCnt + 1;
			SET @CurrentSchema = N'';
			SET @CurrentTable = N'';
		END
		IF @TabName <> @CurrentTable
		BEGIN
			-- open a definition of a new table
			IF @TableCnt > 0
			BEGIN
				insert into #RequestTable values (N',' + @NewLine);
			END
			insert into #RequestTable values (N'                    {' + @NewLine);
			insert into #RequestTable values (N'                        "schema": "' + @SchemaName + '",' + @NewLine); -- // * Required; name of the schema for the table/view 
			insert into #RequestTable values (N'                        "name": "' + @TabName + '",' + @NewLine); -- // * Required; name of the table/view, avoid using duplicate names (only the first occurrence is going to be processed)
			insert into #RequestTable values (N'                        "isView": ' + @IsView + ','  + @NewLine); -- // Optional; true => view, otherwise => table
			insert into #RequestTable values (N'                        "columns": [' + @NewLine); -- // Columns are processed sequentially, provide them in the same order as in your table/view
			SET @CurrentTable = @TabName;
			SET @ColCnt = 0;
			SET @TableCnt = @TableCnt + 1;
			SET @CurrentSchema = @SchemaName;  
		END
		IF @ColCnt > 0
		BEGIN
			insert into #RequestTable values (N',' + @NewLine);
		END
		insert into #RequestTable values (N'                            {' + @NewLine);
		insert into #RequestTable values (N'                                "name": "' + @ColName + '",' + @NewLine); -- // * Required; name of the column, avoid using duplicate names (only the first occurrence is going to be processed)
		insert into #RequestTable values (N'                                "dataType": "' + @DType + '",' + @NewLine); -- // Optional; column data type
		insert into #RequestTable values (N'                                "comment": "' + @Comment + '"' + @NewLine); -- // Optional; column comment if available
		insert into #RequestTable values (N'                            }');
		SET @ColCnt = @ColCnt + 1;

		FETCH NEXT FROM col_cursor 
		INTO @DBName, @SchemaName, @TabName, @IsView, @ColName, @DType, @Comment;
	END

	CLOSE col_cursor;
	DEALLOCATE col_cursor;

	insert into #RequestTable values (@NewLine);
	insert into #RequestTable values (N'                        ]' + @NewLine); -- close columns
	insert into #RequestTable values (N'                    }' + @NewLine); -- close table
	insert into #RequestTable values (N'                ]' + @NewLine); -- close tables
	insert into #RequestTable values (N'            }' + @NewLine); -- close database
	insert into #RequestTable values (N'        ]' + @NewLine); -- close databases
	insert into #RequestTable values (N'    }' + @NewLine); -- close databaseModel
	insert into #RequestTable values (N'}'); -- close query

	-- combine all the lines from the #RequestTable into one string
	select @JSON = (select N'' + Payload from #RequestTable order by LineNum for xml path( N'' ), TYPE ).value('.','nvarchar(max)');
	
	-- clean up
	drop table #AllQueries;	
	drop table #AllDbModels;
	drop table #AllSynonyms;
	drop table #RequestTable;
END

GO

----------------------------------------------------------------------------------------------------------------------
-- drop table mytest;
declare @JSON nvarchar(max);
EXEC sp_extractMetaData 'myUserAccountId', '', @JSON OUT;
select @JSON as c into mytest;
select count(*) from mytest;
-- 2s
drop table mytest;



