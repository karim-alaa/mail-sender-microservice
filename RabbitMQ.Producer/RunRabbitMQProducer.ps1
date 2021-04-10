docker build -t rabbitmq-apps/producer:latest -f ./RabbitMQ.Producer/Dockerfile .
docker run -d --rm --net rabbits -p 7070:80 --name producer rabbitmq-apps/producer:latest
