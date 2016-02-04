# lionfish
Windows GUI app to export database structure to SQLdep

## What does it do?

The utility exports all

- CREATE TABLE ...
- CREATE VIEW ...
- CREATE PROCEDURE ...
- CREATE SYNONYM ...
- CREATE LINK ...

statements into a file. Optionally the utility allows you to
send data directly to SQLdep.com under your Account Id.

## How to use

1. select Database and Driver
1. fill in Username, Password and Database fields
1. fill in your userAccountId (or Key) you got by email or from sqldep.com
1. click on `Test Connection`
1. fill in the name of your export
1. click on `Extract to file` and wait for it to finish (might take some time)
1. (optional) click on `Send data to SQLdep.com`

## What will it do to my database?

The utility will run SELECT statements located under `/sql` directory.
These SELECTs pull data out of DB catalog or dictionary. No temporary
tables are created during this process.

If you are concerned about safety you can even check the source code written
in C#.

https://github.com/sqldep/lionfish

## How to limit what is being exported?

##### MS SQL - only export some databases
1. open file `sql/mssql/databases/cmd.sql`
2. find this section and modify WHERE clause at your will
```sql
        WHERE
        		d.name not in ('master', 'model', 'msdb', 'tempdb', 'SSISDB')
        /* Uncomment following line to filter out databases:                               */
        /* AND d.name NOT IN ('pattern')                                                   */
```

##### MS SQL - only export some views/tables
1. open file `sql/mssql/queries/cmd.sql`
2. find this section and modify WHERE clause at your will
```sql
        /* Uncomment following line to filter out views:                                   */
        /* WHERE name LIKE '%MyPattern%'                                                   */
```

##### MS SQL - only export some procedures
1. open file `sql/mssql/queries/cmd.sql`
2. find this section and modify WHERE clause at your will
```sql
        /* Uncomment following line to filter out procedures:                              */
        /* WHERE name LIKE '%MyPattern%'                                                   */
```

##### Limiting Oracle exports
Edit file `sql/oracle/queries/cmd.sql`

##### Limiting Teradata exports
Edit file `sql/teradata/tables/cmd.sql`

## Troubleshooting

Check the log file `SQLdepLog.txt` for detailed information.
Bugs can be reported at https://github.com/sqldep/lionfish/issues
or via form at http://www.sqldep.com.

Please attach relevant parts of log.
