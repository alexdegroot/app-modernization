IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EavDB')
    BEGIN
        DROP DATABASE EavDb
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
    Id INT PRIMARY KEY,
    ParentId INT NOT NULL,
    Code VARCHAR(100) NOT NULL,  -- Custom code for the entity (contrary to the Id, which is technical key).
    Description VARCHAR(100),
    TemplateId INT FOREIGN KEY REFERENCES Templates(ID),
    TenantId INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Deleted BIT
)

-- Contains data elements for which values can be stored in the Mutations table.
CREATE TABLE dbo.DataElements 
(
    Id INT PRIMARY KEY,
    Description VARCHAR(100),
    Deleted BIT
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
    FieldValue VARCHAR(100),
    MutationDateTime DATETIME,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Deleted BIT NOT NULL
)
GO

-- See https://www.mssqltips.com/sqlservertip/1543/using-sqlcmd-to-execute-multiple-sql-server-scripts/
:r /usr/src/app/Eav/TestData.sql