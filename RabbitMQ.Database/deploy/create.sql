USE master
GO

CREATE DATABASE retry_service
GO

USE [retry_service]
GO

CREATE TABLE Messages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ExchangeName varchar(100),
    SequenceNumber varchar(50),
    RoutingKey varchar(100),
    Body text,
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
    Body text,
    ReDeliveryTimes int,
    Status varchar(50),
    StuckReason varchar(50),
    CreatedAt datetime,
    UpdatedAt datetime
);
GO

CREATE TABLE MessagesErrorLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    MessageId UNIQUEIDENTIFIER,
    ExchangeName varchar(100),
    SequenceNumber varchar(50),
    RoutingKey varchar(100),
    Body text,
    ErrorMessage varchar(MAX),
    Status varchar(50),
    CreatedAt datetime,
    UpdatedAt datetime
);
GO