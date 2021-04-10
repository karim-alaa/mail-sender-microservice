# mail-sender-microservice
Simple, high available microservices with RabbitMQ responsible for sending huge number of emails

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
# Features!
  - Send Mail
  - Retry Sending mail
  - Notify with stuck mails
  - High availability Queue with RabbitMQ Clusters (Quorum  Queues)
  - Easy Deployment & Import customized definitions


