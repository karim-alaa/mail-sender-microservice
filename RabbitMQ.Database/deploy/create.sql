USE master
GO

CREATE DATABASE retry_service
GO

USE [retry_service]
GO


CREATE TABLE EmailRequests (
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