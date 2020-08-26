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

-- Contains entity types, e.g. Company, Employee.
CREATE TABLE dbo.Templates
(
    Id INT PRIMARY KEY,
    Name VARCHAR(50)
)
GO

-- Contains Entity instances, e.g. specific companies or employees.
CREATE TABLE dbo.Entities
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ParentId INT NULL,
    Description VARCHAR(100) NULL,
    TemplateId INT FOREIGN KEY REFERENCES Templates(ID),
    TenantId INT NULL,
    Deleted BIT NULL
)

-- Contains data elements for which values can be stored in the Mutations table.
CREATE TABLE dbo.DataElements 
(
    Id INT PRIMARY KEY,
    Description VARCHAR(100),
    TenantId INT NULL,
    Deleted BIT NULL
)

-- Defines which data elements are allowed for certain templates / entity types.
CREATE TABLE dbo.TemplateFields
(
    TemplateId INT,
    DataElementId INT FOREIGN KEY REFERENCES DataElements(ID)
)
GO

-- Contains data element changes in time (valid from start to end dates).
CREATE TABLE dbo.Mutations 
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EntityId INT FOREIGN KEY REFERENCES Entities(ID),
    DataElementId INT FOREIGN KEY REFERENCES DataElements(ID),
    FieldValue VARCHAR(100) NULL,
    StartDate DATETIME,
    EndDate DATETIME,
    Deleted BIT NULL
)
GO

INSERT INTO dbo.Templates
(
    Id, Name
)
VALUES
    (15, 'Client'),
    (17, 'Company'),
    (21, 'Employee'),
    (49, 'Contract')
GO

INSERT INTO dbo.DataElements
(
    Id, Description
)
VALUES
    (24, 'Last Name'),
    (51, 'First Names')