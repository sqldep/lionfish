select
   'unknown' "owner",
   s.name db_link,
   l.remote_name username,
   s.data_source host
from
   sys.servers s
   left outer join
   sys.linked_logins l on s.server_id = l.server_id
where
   s.is_linked = 1;
