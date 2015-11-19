# lionfish
Windows GUI app to export database structure to SQLDep

## How to run & use (compiled version)

- execute the .exe file.
- select database driver
- fill in connection string (see examples at https://www.connectionstrings.com/)
- fill in your userAccountId from https://sqldep.com/accounts/api/ or  https://sqldep.com/browser/upload/api/
- name your export
- click on Run

## Prerequesities for Oracle

Download and install the Oracle Instant Client for ODBC

## What does it do?

After you specify a connection string the app will run SELECT
statements against your DB catalog. It will dump the
structure of your database into a JSON file and then POST it
via HTTP to sqldep.com.

No data are SELECTed, only structure of the database.

You can inspect the SQL code in the `/sql` directory.

The outgoing JSON file will also be stored locally for your convenience.
