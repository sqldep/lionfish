/* Select databases to export                                                      */
SELECT
		d.name 
	FROM
		sys.databases d
	WHERE
		d.name not in ('master', 'model', 'msdb', 'tempdb', 'SSISDB')
/* AND d.name IN ('db_to_export')                                                  */
	ORDER BY
		d.name;

