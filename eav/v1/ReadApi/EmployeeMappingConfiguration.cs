using ReadApi.Mapping;

namespace ReadApi
{
    internal class EmployeeMappingConfiguration : EntityMappingConfiguration<Employee>
    {
        protected override void ConfigureMapping()
        {
            HasProperty(x => x.EmployeeCode, EavAttributes.Employee.EmployeeCode)
                .HasProperty(x => x.GlobalPersonCode, EavAttributes.Employee.GlobalPersonCode)
                .HasProperty(x => x.Initials, EavAttributes.Employee.Initials)
                .HasProperty(x => x.FirstNames, EavAttributes.Employee.FirstNames)
                .HasProperty(x => x.FirstNameToUse, EavAttributes.Employee.FirstNameToUse)
                .HasProperty(x => x.LastNameAtBirth, EavAttributes.Employee.LastNameAtBirth)
                .HasProperty(x => x.LastNameAtBirthPrefix, EavAttributes.Employee.LastNameAtBirthPrefix)
                .HasProperty(x => x.LastName, EavAttributes.Employee.LastName)
                .HasProperty(x => x.PartnerName, EavAttributes.Employee.PartnerName)
                .HasProperty(x => x.PartnerNamePrefix, EavAttributes.Employee.PartnerNamePrefix)
                .HasProperty(x => x.TitlePrefix, EavAttributes.Employee.TitlePrefix)
                .HasProperty(x => x.TitleSuffix, EavAttributes.Employee.TitleSuffix)
                .HasProperty(x => x.Gender, EavAttributes.Employee.Gender)
                .HasProperty(x => x.DateOfBirth, EavAttributes.Employee.DateOfBirth)
                .HasProperty(x => x.DateOfDeath, EavAttributes.Employee.DateOfDeath)
                .HasProperty(x => x.NationalIdentificationNumber, EavAttributes.Employee.NationalIdentificationNumber)
                .HasProperty(x => x.EmploymentIndicator, EavAttributes.Employee.EmploymentIndicator)
                .HasProperty(x => x.HomeAddressCountry, EavAttributes.Employee.HomeAddress.Country)
                .HasProperty(x => x.HomeAddressCity, EavAttributes.Employee.HomeAddress.City)
                .HasProperty(x => x.HomeAddressPostalCode, EavAttributes.Employee.HomeAddress.PostalCode)
                .HasProperty(x => x.HomeAddressStreetName, EavAttributes.Employee.HomeAddress.StreetName)
                .HasProperty(x => x.HomeAddressStreetNumber, EavAttributes.Employee.HomeAddress.StreetNumber)
                .HasProperty(x => x.HomeAddressStreetNumberAdditional,
                    EavAttributes.Employee.HomeAddress.StreetNumberAdditional)
                .HasProperty(x => x.BusinessPhoneNumber, EavAttributes.Employee.BusinessPhoneNumber)
                .HasProperty(x => x.PrivatePhoneNumber, EavAttributes.Employee.PrivatePhoneNumber)
                .HasProperty(x => x.MobilePhoneNumber, EavAttributes.Employee.MobilePhoneNumber)
                .HasProperty(x => x.BusinessEmailAddress, EavAttributes.Employee.BusinessEmailAddress)
                .HasProperty(x => x.PrivateEmailAddress, EavAttributes.Employee.PrivateEmailAddress)
                .HasProperty(x => x.MaritalStatus, EavAttributes.Employee.MaritalStatus)
                .HasProperty(x => x.FormattedName, EavAttributes.Employee.FormattedName)
                .HasProperty(x => x.SortName, EavAttributes.Employee.SortName)
                .HasProperty(x => x.Nationality, EavAttributes.Employee.Nationality)
                .HasProperty(x => x.Language, EavAttributes.Employee.Language)
                // Employment/contract
                .HasProperty(x => x.EmploymentContractId, EavAttributes.Employee.Employment.ContractId)
                .HasProperty(x => x.EmploymentContractDescription, EavAttributes.Employee.Employment.ContractDescription)
                .HasProperty(x => x.EmploymentHireDate, EavAttributes.Employee.Employment.HireDate)
                .HasProperty(x => x.EmploymentFirstHireDate, EavAttributes.Employee.Employment.FirstHireDate)
                .HasProperty(x => x.EmploymentDischargeDate, EavAttributes.Employee.Employment.DischargeDate)
                .HasProperty(x => x.EndDateEmployment, EavAttributes.Employee.Employment.EndDateEmployment)
                .HasProperty(x => x.EmploymentType, EavAttributes.Employee.Employment.EmploymentType)
                .HasProperty(x => x.EmploymentClassification, EavAttributes.Employee.Employment.ClassificationIndicator)
                .HasProperty(x => x.EmploymentCostCenter, EavAttributes.Employee.Employment.CostCenter)
                .HasProperty(x => x.EmploymentContractType, EavAttributes.Employee.Employment.ContractType)
                .HasProperty(x => x.EmploymentLocation, EavAttributes.Employee.Employment.Location)
                .HasProperty(x => x.EmploymentCompanyCla, EavAttributes.Employee.Employment.CompanyCla)
                .HasProperty(x => x.EmploymentWorkPeriod, EavAttributes.Employee.Employment.WorkPeriod)
                .HasProperty(x => x.EmploymentDaysPeriod, EavAttributes.Employee.Employment.DaysPeriod)
                .HasProperty(x => x.EmploymentHoursPeriod, EavAttributes.Employee.Employment.HoursPeriod)
                .HasProperty(x => x.EmploymentJobProfile, EavAttributes.Employee.Employment.JobProfile)
                .HasProperty(x => x.EmploymentBenefitsCluster, EavAttributes.Employee.Employment.BenefitsCluster)
                .HasProperty(x => x.EmploymentDepartment, EavAttributes.Employee.Employment.Department)
                ;
        }
    }

}

