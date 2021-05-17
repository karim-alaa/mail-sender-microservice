# mail-sender-microservice
Simple, high available microservices with RabbitMQ responsible for sending huge number of emails

# High-Level Architecture
![High-Level Architecture](https://user-images.githubusercontent.com/29948653/114420264-75efcb00-9bb4-11eb-8d8e-cee832b9f9c7.png)

# Simple Sequence Digram
![RabbitMQSequenceDigram](https://user-images.githubusercontent.com/29948653/118473654-8262c880-b70a-11eb-85b9-079456999147.png)

# Technologies
* C#, Console App, Web API App
* .Net Core
* RabbitMQ, RabbitMQ Clustering
* SQL Server Database
* Docker

# Prerequisites
 - Visual studio 2019, or Visual studio code
 - SQL Server 2019
 - .Net Core 5.0
 - Docker
 - Powershell
 - Postman

# Instllation
- Open powershell terminal in the root folder
- On Windows OS, run:
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```
- To start all services:
```powershell
./Start.ps1
```
- To stop all services:
```powershell
./Stop.ps1
```

# Components
| Component  | Technology      | Deployment Approach
|------------|-----------------|---------------------|
| RabbitMQ | run as a cluster in three nodes (rabbit-1, rabbit-2, rabbit-3) | run as a containers
| Consumer |.Net Core 5.0 - Console App|run as a container|
| Producer  |.Net Core 5.0 - Web API App|run as a container|
| Database |SQL Server|run as a container|
         

# Good to know
  - Basically rabbit-1 node is the master and the other two nodes are a mirrors, if rabbit-1 die another node will be the master, and if it back again it will be a mirror.
  - Each node has 2 queues and 1 exchange with different routing keys to handle mails and requests, for now the consumers only consume mails.
  - You can run multiple consumers instances.
  - Producer run as a web API with swagger documentation in http://localhost:7070/swagger/index.html.
  - Currently, Nginx Load Balancer is not implemented yet, so we only use rabbit-1 node.
  - Producer will receive a message with an unique ID 'GUID', and will try to push the message to the queue if the ID is new with initial status 'INQUEUE', consumer should receive the message and change its status to 'PROCEED' or 'ERROR', then a cron job will be run every 20 min to check if there are stuck message with status 'INQUEUE' or 'ERROR' and it will try to re-deliver it again for 2 times, if it is still stuck, the message will be logged in 'StuckMessages' table in the database
  - There is a SQL Job that run every 2 weeks to delete data in messages and logs tables which are exceeded 2 weeks
  - All configs are in 'appsettings.json' file.

# Features! 
  :heavy_check_mark: Send Mail<br/>
  :heavy_check_mark: Retry Deliver mail to quque<br/>
  :heavy_check_mark: Retry Sending mail<br/>
  :heavy_check_mark: Log stuck mails<br/>
  :heavy_check_mark: Custom Logging using global middleware to log any http request.<br/>
  :heavy_check_mark: High availability Queue with RabbitMQ Clusters (Quorum  Queues)<br/>
  :heavy_check_mark: Easy Deployment & Import customized definitions<br/>
  :x: Nginx Load Balancer




  # How It Works
  in the few lines below we will demonstrate the message life cycle and how it affcted in the producer and consumer.

  ## Producer
  Message should sent to producer using https request 'with new Guid' for each new message as error will returned if your trying to resend the message "once the message is in producer, don't worry; it will take care about it's delivery".
  
  Message attchement will be sent as Base64 URL enconded such as 'data:@file/json;base64,CONTENT'
  
  The producer will try to send the message to the queue, save outstanding messages in cache and waiting async for message confirmation from the queue itself, the current status of message in this case will be inproducer and it will be changed to inqueue once the queue confirm delivery.
  
  ## Consumer
  Consumer should always listining for any new message, catch message and try to send it via mail server and change message status to procceed or error, for any mail server error a new log with error will be recorded.
  
  ## Background Job
  A job launching from producer to redelivery messages with status error or inproducer.