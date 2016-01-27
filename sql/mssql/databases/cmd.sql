/* Select databases to export                                                      */
SELECT
		d.name 
	FROM
		sys.databases d
	WHERE
		d.name not in ('master', 'model', 'msdb', 'tempdb', 'SSISDB')
/* Uncomment following line to filter out databases:                               */
/* AND d.name NOT IN ('pattern')                                                   */
	ORDER BY
		d.name;

