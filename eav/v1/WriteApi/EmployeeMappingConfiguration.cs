namespace WriteApi
{
    using Mapping;

    internal class EmployeeMappingConfiguration : EntityMappingConfiguration<Employee>
    {
        protected override void ConfigureMapping()
        {
            HasProperty(x => x.EmployeeCode, EavAttributes.EmployeeCode)
                .HasProperty(x => x.GlobalPersonCode, EavAttributes.GlobalPersonCode)
                .HasProperty(x => x.Initials, EavAttributes.Initials)
                .HasProperty(x => x.FirstNames, EavAttributes.FirstNames)
                .HasProperty(x => x.FirstNameToUse, EavAttributes.FirstNameToUse)
                .HasProperty(x => x.LastNameAtBirth, EavAttributes.LastNameAtBirth)
                .HasProperty(x => x.LastNameAtBirthPrefix, EavAttributes.LastNameAtBirthPrefix)
                .HasProperty(x => x.LastName, EavAttributes.LastName)
                .HasProperty(x => x.PartnerName, EavAttributes.PartnerName)
                .HasProperty(x => x.PartnerNamePrefix, EavAttributes.PartnerNamePrefix)
                .HasProperty(x => x.TitlePrefix, EavAttributes.TitlePrefix)
                .HasProperty(x => x.TitleSuffix, EavAttributes.TitleSuffix)
                .HasProperty(x => x.Gender, EavAttributes.Gender)
                .HasProperty(x => x.DateOfBirth, EavAttributes.DateOfBirth)
                .HasProperty(x => x.DateOfDeath, EavAttributes.DateOfDeath)
                .HasProperty(x => x.NationalIdentificationNumber, EavAttributes.NationalIdentificationNumber)
                .HasProperty(x => x.EmploymentIndicator, EavAttributes.EmploymentIndicator)
                .HasProperty(x => x.HomeAddressCountry, EavAttributes.HomeAddress.Country)
                .HasProperty(x => x.HomeAddressCity, EavAttributes.HomeAddress.City)
                .HasProperty(x => x.HomeAddressPostalCode, EavAttributes.HomeAddress.PostalCode)
                .HasProperty(x => x.HomeAddressStreetName, EavAttributes.HomeAddress.StreetName)
                .HasProperty(x => x.HomeAddressStreetNumber, EavAttributes.HomeAddress.StreetNumber)
                .HasProperty(x => x.HomeAddressStreetNumberAdditional,
                    EavAttributes.HomeAddress.StreetNumberAdditional)
                .HasProperty(x => x.BusinessPhoneNumber, EavAttributes.BusinessPhoneNumber)
                .HasProperty(x => x.PrivatePhoneNumber, EavAttributes.PrivatePhoneNumber)
                .HasProperty(x => x.MobilePhoneNumber, EavAttributes.MobilePhoneNumber)
                .HasProperty(x => x.BusinessEmailAddress, EavAttributes.BusinessEmailAddress)
                .HasProperty(x => x.PrivateEmailAddress, EavAttributes.PrivateEmailAddress)
                .HasProperty(x => x.MaritalStatus, EavAttributes.MaritalStatus)
                .HasProperty(x => x.EndDateEmployment, EavAttributes.EndDateEmployment)
                .HasProperty(x => x.FormattedName, EavAttributes.FormattedName)
                .HasProperty(x => x.SortName, EavAttributes.SortName)
                .HasProperty(x => x.DischargeDate, EavAttributes.DischargeDate)
                .HasProperty(x => x.Nationality, EavAttributes.Nationality)
                .HasProperty(x => x.Language, EavAttributes.Language);
        }
    }

}

