set SAVESTAMP=%TIME::=-%
mkdir %SAVESTAMP%
SQLDepCmd.exe -s "localhost" -p 1521  -db oracle -d "sid" -u "user" -pwd "password" -k "63b95df9-..." -f %SAVESTAMP%/oracle.json
SQLDepCmd.exe  -f %SAVESTAMP% -send SENDONLY  -k "63b95df9-..."


