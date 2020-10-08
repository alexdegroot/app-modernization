using System;
using System.Text.Json.Serialization;

namespace ReadApi.Domain.Model
{
    public class Employee
    {
        private Employment _employment;

        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string EmployeeCode { get; set; }

        public string GlobalPersonCode { get; set; }

        public string Initials { get; set; }

        public string FirstNames { get; set; }

        public string FirstNameToUse { get; set; }

        public string LastNameAtBirth { get; set; }

        public string LastNameAtBirthPrefix { get; set; }

        public string LastName { get; set; }

        public string PartnerName { get; set; }

        public string PartnerNamePrefix { get; set; }

        public string TitlePrefix { get; set; }

        public string TitleSuffix { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfDeath { get; set; }

        public string NationalIdentificationNumber { get; set; }

        public string EmploymentIndicator { get; set; }

        public string MaritalStatus { get; set; }

        public string SortName { get; set; }

        public string Nationality { get; set; }

        public string Language { get; set; }

        public string BusinessPhoneNumber { get; set; }

        public string PrivatePhoneNumber { get; set; }

        public string MobilePhoneNumber { get; set; }

        public string BusinessEmailAddress { get; set; }

        public string PrivateEmailAddress { get; set; }

        public string FormattedName { get; set; }

        public string HomeAddressStreetName { get; set; }

        public string HomeAddressStreetNumber { get; set; }

        public string HomeAddressStreetNumberAdditional { get; set; }

        public string HomeAddressPostalCode { get; set; }

        public string HomeAddressCity { get; set; }

        public string HomeAddressCountry { get; set; }

        [JsonIgnore]
        public string EmploymentContractId { get; set; }

        [JsonIgnore]
        public string EmploymentContractDescription { get; set; }

        [JsonIgnore]
        public DateTime EmploymentHireDate { get; set; }

        [JsonIgnore]
        public DateTime EmploymentFirstHireDate { get; set; }

        [JsonIgnore]
        public DateTime EmploymentDischargeDate { get; set; }

        [JsonIgnore]
        public DateTime EndDateEmployment { get; set; }

        [JsonIgnore]
        public string EmploymentType { get; set; }

        [JsonIgnore]
        public string EmploymentClassification { get; set; }

        [JsonIgnore]
        public string EmploymentCostCenter { get; set; }

        [JsonIgnore]
        public string EmploymentContractType { get; set; }

        [JsonIgnore]
        public string EmploymentLocation { get; set; }

        [JsonIgnore]
        public string EmploymentCompanyCla { get; set; }

        [JsonIgnore]
        public string EmploymentWorkPeriod { get; set; }

        [JsonIgnore]
        public string EmploymentDaysPeriod { get; set; }

        [JsonIgnore]
        public string EmploymentHoursPeriod { get; set; }

        [JsonIgnore]
        public string EmploymentJobProfile { get; set; }

        [JsonIgnore]
        public string EmploymentBenefitsCluster { get; set; }

        [JsonIgnore]
        public string EmploymentDepartment { get; set; }

        public Employment Employment => _employment ?? (_employment = new Employment(this));
    }
}