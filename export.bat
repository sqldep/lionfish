REM This is an example batch file that exports metadata from more databases
REM and sends them as one zip file to SQLdep.com.

REM Set you API key  <------ Edit here
set APIKEY=".........................."

REM Prepare export directory
set SAVESTAMP=%TIME::=-%
mkdir %SAVESTAMP%

REM Examples of exports <------ Edit here 
SQLDepCmd.exe -s localhost -p 1521  -dbType oracle -d orcl -u "c##sedlacek" -pwd kapitanjaros -k %APIKEY% -f %SAVESTAMP%\oracle.json
SQLDepCmd.exe -s DESKTOP-CCKT19A  -dbType mssql -d master -a win_auth -k %APIKEY% -f %SAVESTAMP%\mssql.json
SQLDepCmd.exe -s 192.168.1.6  -dbType teradata -u dbc -pwd dbc -k %APIKEY% -f %SAVESTAMP%\td.json

REM All exports are done - send all files from export directory
SQLDepCmd.exe  -f %SAVESTAMP% -send SENDONLY  -k %APIKEY%
