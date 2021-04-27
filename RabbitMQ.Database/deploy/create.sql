USE master
GO

CREATE DATABASE retry_service
GO

USE [retry_service]
GO

CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ExchangeName varchar(100),
    RoutingKey varchar(100),
    Body Text,
    ReDeliveryTimes int,
    Status varchar(50),
    CreatedAt datetime,
    UpdatedAt datetime
);
GO

CREATE TABLE StuckMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ExchangeName varchar(100),
    RoutingKey varchar(100),
    Body Text,
    ReDeliveryTimes int,
    Status varchar(50),
    CreatedAt datetime,
    UpdatedAt datetime
);
GO