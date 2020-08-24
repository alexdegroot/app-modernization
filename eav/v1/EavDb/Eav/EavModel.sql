IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EavDB')
    BEGIN
        DROP DATABASE EavDB
    END
GO 

CREATE DATABASE EavDb
GO

USE EavDb
GO

IF OBJECT_ID('dbo.Mutations', 'U') IS NOT NULL
    DROP TABLE dbo.Mutations;

IF OBJECT_ID('dbo.TemplateFields', 'U') IS NOT NULL
    DROP TABLE dbo.TemplateFields;

IF OBJECT_ID('dbo.Entities', 'U') IS NOT NULL
    DROP TABLE dbo.Entities;    
    
IF OBJECT_ID('dbo.Templates', 'U') IS NOT NULL
    DROP TABLE dbo.Templates;
    
GO

CREATE TABLE dbo.Templates
(
    Id   UNIQUEIDENTIFIER PRIMARY KEY default NEWID(),
    Name VARCHAR(50),
    Deleted BIT
)
GO

CREATE TABLE dbo.Entities 
(
    Id UNIQUEIDENTIFIER PRIMARY KEY default NEWID(),
    TemplateId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Templates(ID),
    TenantId NUMERIC,
    Deleted BIT
)

CREATE TABLE dbo.TemplateFields 
(
    Id UNIQUEIDENTIFIER PRIMARY KEY default NEWID(),
    Name VARCHAR(50),
    TemplateId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Templates(ID),
    TenantId NUMERIC NULL,
    Deleted BIT
)
GO
        
CREATE TABLE dbo.Mutations 
(
    Id UNIQUEIDENTIFIER PRIMARY KEY default NEWID(),
    EntityId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Entities(ID),
    Value VARCHAR(4000) NULL,
    [Start] datetime2,
    [End] datetime2,
    Deleted BIT
)
GO