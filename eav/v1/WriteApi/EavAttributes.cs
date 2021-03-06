﻿
namespace WriteApi
{
    public static class EavAttributes
    {
        public const int EmployeeCode = 22;
        public const int GlobalPersonCode = 7014;  // UPI
        public const int EmploymentIndicator = 38;  // Dienstverband indicatie
        public const int LastName = 524;
        public const int LastNameAtBirth = 24;
        public const int LastNameAtBirthPrefix = 26;
        public const int FirstNames = 51;
        public const int FirstNameToUse = 165;  // Roepnaam
        public const int FormattedName = 146;
        public const int SortName = 3000;
        public const int Initials = 25;
        public const int PartnerName = 27;
        public const int PartnerNamePrefix = 166;
        public const int TitlePrefix = 94;
        public const int TitleSuffix = 95;
        public const int Gender = 36;
        public const int MaritalStatus = 37;
        public const int DateOfBirth = 35;
        public const int DateOfDeath = 10302568;
        public const int NationalIdentificationNumber = 10000018;
        public const int BusinessEmailAddress = 7212;
        public const int PrivateEmailAddress = 7213;
        public const int BusinessPhoneNumber = 7374;
        public const int PrivatePhoneNumber = 7376;
        public const int MobilePhoneNumber = 7377;
        public const int EndDateEmployment = 40;  // Datum uitdienst
        public const int DischargeDate = 10520479;  // Geplande laatste datum indienst
        public const int Nationality = 10204761;
        public const int Language = 10;

        public static class HomeAddress
        {
            public const int StreetName = 391;
            public const int StreetNumber = 392;
            public const int StreetNumberAdditional = 393;
            public const int PostalCode = 394;
            public const int City = 395;
            public const int Country = 34;
        }
    }
}
