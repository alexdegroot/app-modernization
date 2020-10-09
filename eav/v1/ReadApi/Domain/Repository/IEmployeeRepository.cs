using ReadApi.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadApi.Domain.Repository
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployee(string tenantId, int employeeId);
        Task<IList<Employee>> GetEmployees(string tenantId, int companyId);
    }
}
