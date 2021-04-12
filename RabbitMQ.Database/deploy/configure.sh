#!/bin/bash

# wait for MSSQL server to start
export STATUS=1
i=0

while [[ $STATUS -ne 0 ]] && [[ $i -lt 300 ]]; do
  i=$i+1
  /opt/mssql-tools/bin/sqlcmd -t 1 -U sa -P $SA_PASSWORD -Q "select 1" >> /dev/null
  STATUS=$?
done

if [ $STATUS -ne 0 ]; then 
  echo "Error: MSSQL SERVER took more than 60 seconds to start up."
  echo $SA_PASSWORD
  exit 1
fi

echo "======= MSSQL SERVER STARTED ========" #| tee -a /scripts/config.log
# Run the setup script to create the DB and the schema in the DB
#/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Abcdefg1234 -d master -i /scripts/test.sql
for script in /scripts/*.sql;
do
  /opt/mssql-tools/bin/sqlcmd -S127.0.0.1 -Usa -P${SA_PASSWORD}  -i$script;
done

echo "======= MSSQL CONFIG COMPLETE =======" #| tee -a /scripts/config.log