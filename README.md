# mail-sender-microservice
Simple, high available microservices with RabbitMQ responsible for sending huge number of emails

# High-Level Architecture
![High-Level Architecture](https://user-images.githubusercontent.com/29948653/114420264-75efcb00-9bb4-11eb-8d8e-cee832b9f9c7.png)

# Technologies
* C#, Console App, Web API App
* .Net Core
* RabbitMQ, RabbitMQ Clustering
* SQL Server Database
* Docker
* Json 

# Prerequisites
 - Visual studio 2019, or Visual studio code
 - SQL Server 2019
 - .Net Core 5.0
 - Docker

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
|Consumer |.Net Core 5.0|run as a container|
|Producer  |.Net Core 5.0|run as a container|
|Database |SQL Server|run as a container|
         

# Good to know
  - Basically rabbit-1 node is the master and the other two nodes are a mirrors, if rabbit-1 die another node will be the master, and if it back again it will be a mirror
  - Each node has 2 queues and 1 exchange with different routing keys to handle mails and requests, for now the consumers only consume mails
  - You can run multiple consumers instances
  - Producer run as a web API with swagger documentation in http://localhost:7070/swagger/index.html

# Features!
  - Send Mail
  - Retry Sending mail
  - Notify with stuck mails
  - High availability Queue with RabbitMQ Clusters (Quorum  Queues)
  - Easy Deployment & Import customized definitions


