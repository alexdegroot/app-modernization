using Microsoft.AspNetCore.Mvc;
using ReadApi.Domain.Repository;
using System.Threading.Tasks;

namespace ReadApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : BaseController
    {
        [HttpGet("{companyId:int:min(1):required}/employees")]
        public async Task<IActionResult> GetEmployeesByCompany(int companyId, [FromServices] IEmployeeRepository employeeRepository)
        {
            string tenantHeader = GetTenantIdFromHeaders();            
            var employees = await employeeRepository.GetEmployees(tenantHeader, companyId).ConfigureAwait(false);
            return employees != null ? Ok(ToJson(employees)) : NotFound();
        }
    }
}
