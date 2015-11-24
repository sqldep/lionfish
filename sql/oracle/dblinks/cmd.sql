select
	dbl.owner as owner,
	dbl.db_link as name,
	'username' as userName,
	dbl.host as hostName
from ALL_DB_LINKS dbl;
