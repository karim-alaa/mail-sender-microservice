docker container rm producer
docker build -t rabbitmq-apps/producer:latest -f ./RabbitMQ.Producer/Dockerfile .
docker run -d --rm --net rabbits -p 7070:80 -e TZ=Africa/Cairo -e "ASPNETCORE_ENVIRONMENT=Production" --name producer rabbitmq-apps/producer:latest
