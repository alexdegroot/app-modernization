using Microsoft.AspNetCore.Mvc;
using ReadApi.Domain.Repository;
using System.Threading.Tasks;

namespace ReadApi.Controllers
{
    [ApiController]
    [Route("employees")]
    public class EmployeeController : BaseController
    {
        [HttpGet("{employeeId:int:min(1):required}")]       
        public async Task<IActionResult> GetById(int employeeId, [FromServices] IEmployeeRepository employeeRepository)
        {
            string tenantHeader = GetTenantIdFromHeaders();
            var employee = await employeeRepository.GetEmployee(tenantHeader, employeeId).ConfigureAwait(false);
            return employee != null ? Ok(ToJson(employee)) : NotFound();
        }
    }
}
