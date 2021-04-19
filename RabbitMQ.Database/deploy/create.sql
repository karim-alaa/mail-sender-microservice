USE master
GO

CREATE DATABASE retry_service
GO

USE [retry_service]
GO



CREATE TABLE StuckEmailRequests (
    Id varchar(100),
    Subject varchar(max),
    Body text,
    FromEmail varchar(max),
    ToEmails varchar(max),
    CC varchar(max),
    BCC varchar(max),
    CtreatedAt datetime,
    UpdatedAt datetime
);
GO

CREATE TABLE Messages (
    Id varchar(100) primary key,
    ExchangeName varchar(100),
    RoutingKey varchar(100),
    Body Text,
    Status varchar(50),
    CreatedAt datetime,
    UpdatedAt datetime
);
GO