# Build rabbitMQ cluster 
Push-Location ./RabbitMQ.Infrastructure
./RunRabbitMQCluster.ps1
Pop-Location 

# Build database
Push-Location ./RabbitMQ.Database
./RunRabbitMQDatabase.ps1
Pop-Location 

# Build consumer
Push-Location ./RabbitMQ.Consumer
./RunRabbitMQConsumer.ps1
Pop-Location 

# Build producer
Push-Location ./RabbitMQ.Producer
./RunRabbitMQProducer.ps1
Pop-Location 