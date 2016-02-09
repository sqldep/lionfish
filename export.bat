set SAVESTAMP=%DATE:/=-%@%TIME::=-%
mkdir %SAVESTAMP%
./SQLDepCmd.exe -f %SAVESTAMP%/db1.json
./SQLDepCmd.exe -f %SAVESTAMP%/db1.json
./SQLDepCmd.exe -send SENDONLY -dir %SAVESTAMP%

