using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WriteApi.Mapping;

namespace WriteApi
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;
        private readonly IDataElementMapper<Employee> _dataElementMapper;

        public EmployeeRepository(ILogger<EmployeeRepository> logger)
        {
            _connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING");
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataElementMapper = new FluentDataElementMapper<Employee>(
                new EmployeeMappingConfiguration());
        }

        public async Task Update(int employeeId, Employee employee)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(employeeId));
            }

            var startDate = DateTime.Now.Date;
            var endDate = DateTime.MaxValue.Date;

            // TODO: Following commands should only be run when the employee data element values have actually changed.
            var dataElements = _dataElementMapper.MapToDataElements(employee);
            await SetMutationsDeleted(employeeId, startDate, dataElements);
            await InsertMutations(employeeId, startDate, endDate, dataElements);
        }

        public async Task Add(Employee employee)
        {
            var firstName = employee.FirstNames;
            var lastName = employee.LastName;
            var employeeCode = employee.EmployeeCode;

            // Perform some basic validations. Validation errors should probably be handled
            // in an alternative way.
            if (employee.Id <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(employee.Id));
            }

            if (employee.TenantId <= 0)
            {
                throw new ArgumentException("Invalid Tenant ID", nameof(employee.TenantId));
            }

            if (employee.CompanyId <= 0)
            {
                throw new ArgumentException("Invalid Company ID", nameof(employee.CompanyId));
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Invalid First Name", nameof(employee.FirstNames));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Invalid Last Name", nameof(employee.LastName));
            }

            if (string.IsNullOrWhiteSpace(employeeCode))
            {
                throw new ArgumentException("Invalid Employee Code", nameof(employee.EmployeeCode));
            }

            var fullName = $"{lastName}, {firstName}";
            const int templateId = (int) EntityType.Employee;

            var startDate = DateTime.Now.Date;
            var endDate = DateTime.MaxValue.Date;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"INSERT INTO dbo.Entities (Id, ParentId, Code, Description, TemplateId, TenantId, Deleted)
                VALUES (@Id, @ParentId, @EmployeeCode, @FullName, @TemplateId, @TenantId, @Deleted)";

            var cmd = new SqlCommand(sql);
            AddCommandParameter(cmd, "@Id", employee.Id, SqlDbType.Int);
            AddCommandParameter(cmd, "@ParentId", employee.CompanyId, SqlDbType.Int);
            AddCommandParameter(cmd, "@EmployeeCode", employee.EmployeeCode, SqlDbType.VarChar);
            AddCommandParameter(cmd, "@FullName", fullName, SqlDbType.VarChar);
            AddCommandParameter(cmd, "@TemplateId", templateId, SqlDbType.Int);
            AddCommandParameter(cmd, "@TenantId", employee.TenantId, SqlDbType.Int);
            AddCommandParameter(cmd, "@Deleted", 0, SqlDbType.Bit);
            cmd.Connection = connection;
            await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation($"Entity ID: {employee.Id}");

            var dataElements = _dataElementMapper.MapToDataElements(employee);
            await InsertMutations(employee.Id, startDate, endDate, dataElements);
        }

        /// <summary>
        /// Sets the 'Deleted' status of mutations that correspond to the specified entityId,
        /// reference date and data elements.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="referenceDate"></param>
        /// <param name="dataElements"></param>
        /// <returns></returns>
        private async Task SetMutationsDeleted(int entityId, DateTime referenceDate,
            IEnumerable<DataElement> dataElements)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            foreach (var dataElement in dataElements)
            {
                _logger.LogInformation($"Setting delete status for entity {entityId}, ref. date {referenceDate}, data element '{dataElement.Id}.");

                const string sql = @"
                UPDATE dbo.Mutations SET Deleted = 1, EndDate = @ReferenceDate
                    WHERE EntityId = @EntityId AND DataElementId = @DataElementId
                    AND @ReferenceDate BETWEEN StartDate and EndDate";

                var cmd = new SqlCommand(sql);
                AddCommandParameter(cmd, "@ReferenceDate", referenceDate, SqlDbType.DateTime);
                AddCommandParameter(cmd, "@EntityId", entityId, SqlDbType.Int);
                AddCommandParameter(cmd, "@DataElementId", dataElement.Id, SqlDbType.Int);
                cmd.Connection = connection;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task InsertMutations(int entityId,
            DateTime startDate, DateTime endDate, IEnumerable<DataElement> dataElements)
        {
            var mutations = new List<Mutation>();

            foreach (var dataElement in dataElements)
            {
                _logger.LogInformation($"Adding mutation for data element {dataElement.Id}, value '{dataElement.Value}'");
                mutations.Add(new Mutation
                {
                    EntityId = entityId,
                    DataElementId = dataElement.Id,
                    FieldValue = dataElement.Value.ToString(),
                    StartDate = startDate,
                    EndDate = endDate,
                    IsDeleted = false
                });
            }

            await InsertMutations(mutations);
        }

        private async Task InsertMutations(IEnumerable<Mutation> mutations)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var mutation in mutations)
            {
                const string sql = @"
                    INSERT INTO dbo.Mutations (EntityId, DataElementId, FieldValue, StartDate, EndDate, Deleted)
                        VALUES (@EntityId, @DataElementId, @FieldValue, @StartDate, @EndDate, @Deleted);";

                var cmd = new SqlCommand(sql);
                AddCommandParameter(cmd, "@EntityId", mutation.EntityId, SqlDbType.Int);
                AddCommandParameter(cmd, "@DataElementId", mutation.DataElementId, SqlDbType.Int);
                AddCommandParameter(cmd, "@FieldValue", mutation.FieldValue, SqlDbType.VarChar);
                AddCommandParameter(cmd, "@StartDate", mutation.StartDate, SqlDbType.DateTime);
                AddCommandParameter(cmd, "@EndDate", mutation.EndDate, SqlDbType.DateTime);
                AddCommandParameter(cmd, "@Deleted", mutation.IsDeleted, SqlDbType.Bit);
                cmd.Connection = connection;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static void AddCommandParameter(SqlCommand command, string paramName,
            object paramValue, SqlDbType paramType)
        {
            var parameter = command.Parameters.Add(paramName, paramType);
            parameter.Value = paramValue;
        }
    }
}