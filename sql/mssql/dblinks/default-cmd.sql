/* IMPORTANT: If you modify this file, rename it to cmd.sql (so it won't be overwritten when you upgrade lionfish).
Only cmd.sql will be used when both exist.*/
/* Select all database links (information about linked servers)                    */
/* To avoid exporting links uncomment the following SQL                            */
/* SELECT 1,1,1,1;                                                                 */
SELECT
   'unknown' "owner",
   s.name db_link,
   l.remote_name username,
   s.data_source host
FROM
   sys.servers s
   left outer join
   sys.linked_logins l on s.server_id = l.server_id
WHERE
   s.is_linked = 1;
