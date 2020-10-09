using System;

namespace ReadApi.Domain.Model
{
    public class Employment
    {
        public Employment(Employee employee)
        {
            ContractId = employee.EmploymentContractId;
            ContractDescription = employee.EmploymentContractDescription;
            HireDate = employee.EmploymentHireDate;
            FirstHireDate = employee.EmploymentFirstHireDate;
            DischargeDate = employee.EmploymentDischargeDate;
            EmploymentType = employee.EmploymentType;
            Classification = employee.EmploymentClassification;
            CostCenter = employee.EmploymentCostCenter;
            ContractType = employee.EmploymentContractType;
            Location = employee.EmploymentLocation;
            CompanyCla = employee.EmploymentCompanyCla;
            WorkPeriod = employee.EmploymentWorkPeriod;
            DaysPeriod = employee.EmploymentDaysPeriod;
            HoursPeriod = employee.EmploymentHoursPeriod;
            JobProfile = employee.EmploymentJobProfile;
            BenefitsCluster = employee.EmploymentBenefitsCluster;
            Department = employee.EmploymentDepartment;
        }

        public string ContractId { get; set; }

        public string ContractDescription { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime FirstHireDate { get; set; }

        public DateTime DischargeDate { get; set; }

        public string EmploymentType { get; set; }

        public string Classification { get; set; }

        public string CostCenter { get; set; }

        public string ContractType { get; set; }

        public string Location { get; set; }

        public string CompanyCla { get; set; }

        public string WorkPeriod { get; set; }

        public string DaysPeriod { get; set; }

        public string HoursPeriod { get; set; }

        public string JobProfile { get; set; }

        public string BenefitsCluster { get; set; }

        public string Department { get; set; }
    }
}
