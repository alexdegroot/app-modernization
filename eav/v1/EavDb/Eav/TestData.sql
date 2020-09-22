PRINT 'Inserting test clients, companies and data elements...'

-- Insert entity types.
INSERT INTO dbo.Templates
(
    Id, Name
)
VALUES
    (12, 'Land'),
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
    (10, 'Language'),
    (22, 'Employee Code'),
    (7014, 'Global Person Code'),
    (524, 'Last Name'),
    (24, 'Last Name At Birth'),
    (26, 'Last Name At Birth Prefix'),
    (44, 'Customer Name'),
    (45, 'Company Name'),
    (51, 'First Names'),
    (165, 'First Name To Use'),
    (146, 'Formatted Name'),
    (3000, 'Sort Name'),       
    (25, 'Initials'),
    (27, 'Partner Name'),
    (166, 'Partner Name Prefix'),
    (94, 'Title Prefix'),     -- E.g. Drs
    (95, 'Title Suffix'),     -- E.g. MA
    (35, 'Date Of Birth'),
    (10302568, 'Date Of Death'),
    (36, 'Gender'),
    (37, 'Marital Status'),
    (38, 'Employment Indicator'),  -- In/uitdienst
    (39, 'Hire Date'),  -- Datum indienst
    (308, 'First Hire Date'),  -- Eerste datum indienst
    (10000018, 'National Identification Number'),
    (40, 'End Date Employment'),
    (10520479, 'Discharge Date'),  -- Geplande laatste datum indienst
    (7212, 'Business Email Address'),
    (7213, 'Private Email Address'),
    (7374, 'Business Phone Number'),
    (7376, 'Private Phone Number'),
    (7377, 'Mobile Phone Number'),
    (10204761, 'Nationality'),
    -- Address fields
    (391, 'Street Name'),
    (392, 'Street Number'),
    (393, 'Street Number Additional'),
    (394, 'Postal Code'),
    (395, 'City'),
    (34,  'Country'),
    -- Contract elements
    (90, 'Contract Description'),
    (91, 'Contract Code')
GO

-- Insert a country, test clients (template 15) and some companies below them (template 17).
INSERT INTO dbo.Entities
(
    Id, ParentId, Code, Description, TemplateId, TenantId
)
VALUES
    (10000001, 0, 'NL', 'Nederland', 12, 0),
    (10029872, 10000001, '4024898', 'Metatech Nederland', 15, 10029872),
    (10496315, 10000001, '5031084', 'Volvo Nederland N.V.', 15, 10496315),
    (10618376, 10000001, '4000306', 'Vodafone Libertel', 15, 10618376),
    (10028636, 10029872, '030', 'Metatech Administratie B.V.', 17, 10029872),
    (10028637, 10029872, '800', 'Metatech Constructie', 17, 10029872),
    (10028504, 10029872, '020', 'Metatech Horeca Services', 17, 10029872),
    (10183634, 10029872, '040', 'Metatech Wonen', 17, 10029872),
    (10496382, 10496315, 'DD41', 'Volvo Nederland B.V.', 17, 10496315),
    (10496389, 10496315, 'HH48', 'VFS Financial Services B.V.', 17, 10496315),
    (10496410, 10496315, 'HH55', 'Volvo Truck Center B.V.', 17, 10496315),
    (10619998, 10618376, 'VL99', 'Vodafone Libertel B.V.', 17, 10618376)
GO

-- Insert some test employees (template 21) and contracts (template 49).
INSERT INTO dbo.Entities
(
    Id, ParentId, Code, Description, TemplateId, TenantId
)
VALUES
    (10512861, 10503058, '98310', 'Basiscontract', 49, 10029872),
    (10512862, 10503060, '99020', 'Basiscontract', 49, 10029872),
    (10503058, 10028636, '99020', 'Janssen, J.P.', 21, 10029872),
    (10503060, 10028636, '98310', 'Bloemendaal, B.', 21, 10029872)

GO

-- Insert a (default) language value on country level and client and company name values.
INSERT INTO dbo.Mutations
(
    EntityId, DataElementId, FieldValue, StartDate, EndDate, Deleted
)
VALUES
    (10000001, 10, 'NL', '2001-01-01', '9999-12-31', 0),
    (10029872, 44, 'Metatech Nederland', '2001-01-01', '9999-12-31', 0),
    (10496315, 44, 'Volvo Nederland N.V.', '2001-01-01', '9999-12-31', 0),
    (10618376, 44, 'Vodafone Libertel', '2001-01-01', '9999-12-31', 0),
    (10028636, 45, 'Metatech Administratie B.V.', '2001-01-01', '9999-12-31', 0),
    (10028637, 45, 'Metatech Constructie', '2001-01-01', '9999-12-31', 0),
    (10028504, 45, 'Metatech Horeca Services', '2001-01-01', '9999-12-31', 0),
    (10183634, 45, 'Metatech Wonen', '2001-01-01', '9999-12-31', 0),
    (10496382, 45, 'Volvo Nederland B.V.', '2001-01-01', '9999-12-31', 0),
    (10496389, 45, 'VFS Financial Services B.V.', '2001-01-01', '9999-12-31', 0),
    (10496410, 45, 'Volvo Truck Center B.V.', '2001-01-01', '9999-12-31', 0),
    (10619998, 45, 'Vodafone Libertel B.V.', '2001-01-01', '9999-12-31', 0)
GO

-- Insert employee and contract values.
INSERT INTO dbo.Mutations
(
    EntityId, DataElementId, FieldValue, StartDate, EndDate, Deleted
)
VALUES
    (10000001, 10, 'NL', '2001-01-01', '9999-12-31', 0),
    (10503058, 22, N'99020', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0),
    (10503060, 22, N'98310', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0),
    (10512861, 90, N'001', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0),
    (10512862, 90, N'001', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0),
    (10512861, 91, N'Basiscontract', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0),
    (10512862, 91, N'Basiscontract', CAST(N'2002-01-01T00:00:00.000' AS DateTime), CAST(N'9999-12-31T00:00:00.000' AS DateTime), 0)
GO