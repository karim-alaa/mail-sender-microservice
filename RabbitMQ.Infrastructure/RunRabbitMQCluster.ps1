# For Windows, run the below command before running the file with ./filename.ps1
# Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# Remove running docker containers
docker rm -f rabbit-1 >$null 2>&1
docker rm -f rabbit-2 >$null 2>&1
docker rm -f rabbit-3 >$null 2>&1

Write-Output "Docker Containers Cleaned"

# Create docker network
docker network create --driver bridge rabbits >$null 2>&1

#-v /config/rabbitmq-definitions.json:/etc/rabbitmq/definitions.json `
#-v /config/basic-config.config:/etc/rabbitmq/config.json `

# Run the three rabbitmq instances
docker run -d --rm --net rabbits `
-v ${PWD}/config/:/config/ `
-e RABBITMQ_CONFIG_FILE=/config/custom-config `
-e RABBITMQ_ERLANG_COOKIE=EPAYMENTRETRYSERVICECOOKIE `
-e TZ=Africa/Cairo `
--hostname rabbit-1 `
--name rabbit-1 `
-p 8081:15672 `
-p 9091:5672 `
rabbitmq:3.8-management

Write-Output "Node 1 Created. check http://localhost:8081"

docker run -d --rm --net rabbits `
-v ${PWD}/config/:/config/ `
-e RABBITMQ_CONFIG_FILE=/config/custom-config `
-e RABBITMQ_ERLANG_COOKIE=EPAYMENTRETRYSERVICECOOKIE `
-e TZ=Africa/Cairo `
--hostname rabbit-2 `
--name rabbit-2 `
-p 8082:15672 `
-p 9092:5672 `
rabbitmq:3.8-management

Write-Output "Node 2 Created. check http://localhost:8082"

docker run -d --rm --net rabbits `
-v ${PWD}/config/:/config/ `
-e RABBITMQ_CONFIG_FILE=/config/custom-config `
-e RABBITMQ_ERLANG_COOKIE=EPAYMENTRETRYSERVICECOOKIE `
-e TZ=Africa/Cairo `
--hostname rabbit-3 `
--name rabbit-3 `
-p 8083:15672 `
-p 9093:5672 `
rabbitmq:3.8-management

Write-Output "Node 3 Created. check http://localhost:8083"

# Wait till the instances being up and running
Write-Output "Wait till the instances being up and running..."
for($j = 1; $j -lt 80; $j++ )
    {
        Start-Sleep -s 1
        Write-Progress -Id 1 -Activity Building -Status 'Progress' -PercentComplete $j -CurrentOperation Running-RabbitMQ-Instances
    }

# Enable federation plugin
docker exec -it rabbit-1 rabbitmq-plugins enable rabbitmq_federation >$null 2>&1
docker exec -it rabbit-2 rabbitmq-plugins enable rabbitmq_federation >$null 2>&1
docker exec -it rabbit-3 rabbitmq-plugins enable rabbitmq_federation >$null 2>&1

Write-Output "Federation enabled."

$enableMirroringAndAutoSyncScript = Get-Content "./SetupNodesPolicies.sh"
docker exec -it rabbit-1 bash -c $enableMirroringAndAutoSyncScript

Write-Output "Queue Mirroring and Automatic Synchronization are enabled."

$loadBasicDefinitionScript = "rabbitmqctl import_definitions " + "/config/basic-definitions.json"
docker exec -it rabbit-1 bash -c $loadBasicDefinitionScript

$loadCustomDefinitionScript = "rabbitmqctl import_definitions " + "/config/custom-definitions.json"
docker exec -it rabbit-1 bash -c $loadCustomDefinitionScript

Write-Output "Exchanges, Queues, Users and other settings restored."

Write-Output "Services are running..."