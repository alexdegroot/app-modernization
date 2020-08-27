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

-- Insert entity types.
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

-- Define a basic set of data elements.
INSERT INTO dbo.DataElements
(
    Id, Description
)
VALUES
    (24, 'Last Name'),
    (25, 'Initials'),
    (26, 'Last Name At Birth Prefix'),
    (27, 'Partner Name'),
    (35, 'BirthDate'),
    (36, 'Gender'),
    (38, 'Employment Indicator'),
    (39, 'Hire Date'),  -- Datum indienst
    (51, 'First Names'),
    (94, 'Prefix Titles'),  -- E.g. Drs
    (95, 'Suffix Titles'),  -- E.g. MA
    (165, 'First Name To Use'),  -- Roepnaam
    (166, 'Partner Name Prefix'),
    (308, 'First Hire Date'),  -- Eerste datum indienst
    (7014, 'UPI'),
    (7213, 'Private Email Address'),
    (10520479, 'DischargeDate')  -- Geplande laatste datum indienst
GO

-- Insert a test client and some companies below it.
INSERT INTO dbo.Entities
(
    ParentId, Description, TemplateId
)
VALUES
    (NULL, 'Metatech Nederland', 15),
    (1, 'Metatech Administratie BV', 17),
    (1, 'Metatech Constructie', 17),
    (1, 'Metatech Horeca Services', 17),
    (1, 'Metatech Wonen', 17)
GO