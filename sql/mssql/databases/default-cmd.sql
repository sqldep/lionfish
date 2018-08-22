/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/
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

