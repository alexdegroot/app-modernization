using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReadApi.Domain.Model;
using ReadApi.Domain.ModelMapping;
using ReadApi.Infrastructure.Database;
using ReadApi.Infrastructure.Mapping;

namespace ReadApi.Domain.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<EmployeeRepository> _logger;
        private readonly IDatabaseReader _databaseReader;
        private readonly IEntityMapper<Employee> _entityMapper;

        public EmployeeRepository(ILogger<EmployeeRepository> logger, IDatabaseReader databaseReader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseReader = databaseReader ?? throw new ArgumentNullException(nameof(databaseReader));
            _entityMapper = new FluentEntityMapper<Employee>(
                new EmployeeMappingConfiguration());
        }

        public async Task<Employee> GetEmployee(string tenantId, int employeeId)
        {
            _logger.LogInformation($"Reading employee {employeeId} from Mongo DB...");

            Entity entity = null;
            try
            {
                entity = await _databaseReader.ReadEntity(tenantId, employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred.");
            }

            var employee = new Employee();

            if (entity?.Mutations == null)
            {
                return employee;
            }

            foreach (var mutation in entity.Mutations)
            {
                _entityMapper.MapToEntity(employee, new DataElementRow(
                    mutation.FieldId,
                    // TODO: Take data element types into account.
                    mutation.Value != null ? mutation.Value.ToString() : string.Empty,
                    DataElementDataType.Undefined));
            }

            return employee;
        }

        public async Task<IList<Employee>> GetEmployees(string tenantId, int companyId)
        {
            _logger.LogInformation($"Reading employees for company {companyId} from Mongo DB...");

            IList<Entity> entities = null;
            try
            {
                entities = await _databaseReader.ReadEntities(tenantId, companyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred.");
            }

            var employees = new List<Employee>();

            if (entities == null)
            {
                return employees;
            }

            foreach (var entity in entities)
            {
                var employee = new Employee
                {
                    Id = entity.Id,
                    CompanyId = entity.ParentId
                };
                employees.Add(employee);
                MapToEmployee(entity, employee);
            }

            return employees;
        }

        private void MapToEmployee(Entity entity, Employee employee)
        {
            foreach (var mutation in entity.Mutations)
            {
                _entityMapper.MapToEntity(employee, new DataElementRow(
                    mutation.FieldId,
                    // TODO: Take data element types into account.
                    mutation.Value != null ? mutation.Value.ToString() : string.Empty,
                    DataElementDataType.Undefined));
            }

            if (entity.ChildEntities != null)
            {
                foreach (var childEntity in entity.ChildEntities)
                {
                    if (childEntity.TemplateId == (int)EntityType.Contract)
                    {
                        MapToEmployee(childEntity, employee);
                    }
                }
            }
        }
    }
}
