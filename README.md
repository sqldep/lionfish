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

It is possible to manually adjust SQL queries that extract metadata.
You can limit the export only to certain databases, schemas or even 
to views that match a given pattern like:

```
name LIKE '%pattern%'
```

So it is completely in your hands. We have prepared `WHERE` statements
which you just comment out.

### MS SQL
Edit these files:
```
sql/mssql/tables/cmd.sql
sql/mssql/queries/cmd.sql
sql/mssql/dblinks/cmd.sql
sql/mssql/databases/cmd.sql
sql/mssql/synonyms/cmd.sql
```

### Oracle
Edit these files:
```
sql/oracle/tables/cmd.sql
sql/oracle/queries/cmd.sql
sql/oracle/dblinks/cmd.sql
sql/oracle/databases/cmd.sql
sql/oracle/synonyms/cmd.sql
```

### Teradata
Edit these files:
```
sql/teradata/tables/cmd.sql
sql/teradata/databases/cmd.sql
```

## Troubleshooting

Check the log file `SQLdepLog.txt` for detailed information.
Bugs can be reported at https://github.com/sqldep/lionfish/issues
or via form at http://www.sqldep.com.

Please attach relevant parts of log.
