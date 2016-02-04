/* Select databases to export                                                      */
SELECT DISTINCT(databasename) FROM dbc.tables WHERE databasename NOT IN (
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
/* You can add more databases here */

)

/* AND databasename IN ('db_to_export')                                           */

;
