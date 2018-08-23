/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/
-- Select databases to export
SELECT DISTINCT(databasename) FROM dbc.tablesv WHERE databasename NOT IN (
	'dbc',
	'sys_calendar',
	'systemfe',
	'tdwm',
	'sqlj',
	'syslib',
	'tdstats',
	'dbcmngr',
	'locklogshredder',
	'sysadmin',
	'sysbar',
	'sysspatial',
	'sysuif',
	'tdqcd',
	'td_sysxml',
	'sysudtlib',
	'td_sysgpl',
	'td_sysfnlib'
-- You can add more databases here

)

-- AND databasename IN ('db_to_export')

;