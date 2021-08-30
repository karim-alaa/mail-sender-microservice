#docker build -t rabbitmq-apps/database:latest --build-arg SA_PASSWORD=WE_RETRY_SERVICE_DB .
#docker run -d --rm --net rabbits -p 1433:1433 --name database rabbitmq-apps/database:latest


docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssW0rdC00MP!!EX#" `
   --name database `
   -h database `
   --net rabbits `
   -e TZ=Africa/Cairo `
   -e MSSQL_AGENT_ENABLED=True `
   -p 2020:1433 `
   -v ./deploy/Create.sql `
   -v ./deploy/ScheduledJob.sql `
   -d mcr.microsoft.com/mssql/server:2019-latest

Write-Output "Wait till the sql server being up and running..."
for($j = 1; $j -lt 15; $j++ )
    {
        Start-Sleep -s 1
        Write-Progress -Id 1 -Activity Building -Status 'Progress' -PercentComplete $j -CurrentOperation Running-RabbitMQ-Instances
    }

docker exec -it database /opt/mssql-tools/bin/sqlcmd `
   -S localhost -U SA -P "P@ssW0rdC00MP!!EX#" `
   -Q "ALTER LOGIN SA WITH PASSWORD='W0rd@Pass#WE1#'"

Write-Output "SA Password Changed"

docker cp ./deploy/create.sql database:/tmp/
docker cp ./deploy/scheduledJob.sql database:/tmp/

docker exec -it database /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "W0rd@Pass#WE1#" -i ./tmp/create.sql
docker exec -it database /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "W0rd@Pass#WE1#" -i ./tmp/scheduledJob.sql

Write-Output "Database imported!"