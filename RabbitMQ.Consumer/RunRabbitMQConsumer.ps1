docker build -t rabbitmq-apps/consumer:latest -f ./RabbitMQ.Consumer/Dockerfile .
docker run -d --rm --net rabbits -p 6060:80 --name consumer rabbitmq-apps/consumer:latest
