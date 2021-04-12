docker build -t rabbitmq-apps/consumer:latest -f ./RabbitMQ.Consumer/Dockerfile .
docker run -d -dt --rm --net rabbits --name consumer -e "ASPNETCORE_ENVIRONMENT=Production" rabbitmq-apps/consumer:latest
docker logs consumer

