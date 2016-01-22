SELECT DISTINCT(databasename) FROM dbc.tables WHERE databasename NOT IN (
	'dbc',
	'sys_calendar',
	'systemfe',
	'tdwm',
	'sqlj',
	'syslib',
	'tdstats'
);
