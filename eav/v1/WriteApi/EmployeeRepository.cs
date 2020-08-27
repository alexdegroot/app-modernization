using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace WriteApi
{
    using System.Data;

    public class EmployeeRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(ILogger<EmployeeRepository> logger)
        {
            _connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING");
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString));
            }
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Update(int employeeId, Employee employee)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(employeeId));
            }

            var firstName = employee.FirstName;
            var lastName = employee.LastName;

            var startDate = DateTime.Now.Date;
            var endDate = DateTime.MaxValue;

            // TODO: Following commands should only be run when the employee data element values have actually changed.
            await SetMutationDeleted(startDate, employeeId, DataElement.LastName);
            await SetMutationDeleted(startDate, employeeId, DataElement.FirstNames);
            await InsertMutations(new List<Mutation>
            {
                new Mutation
                {
                    EntityId = employeeId,
                    DataElementId = DataElement.LastName,
                    FieldValue = lastName,
                    StartDate = startDate,
                    EndDate = endDate,

                },
                new Mutation
                {
                    EntityId = employeeId,
                    DataElementId = DataElement.FirstNames,
                    FieldValue = firstName,
                    StartDate = startDate,
                    EndDate = endDate,
                }
            });
        }

        public async Task<int> Add(Employee employee)
        {
            var firstName = employee.FirstName;
            var lastName = employee.LastName;

            // Perform some basic validations. Validation errors should probably be handled
            // in an alternative way.
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Invalid First Name", nameof(employee.FirstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Invalid Last Name", nameof(employee.LastName));
            }

            var fullName = $"{lastName}, {firstName}";
            // TODO: Replace by enum value or external constant.
            const int templateId = 21;

            var startDate = DateTime.Now.Date;
            var endDate = DateTime.MaxValue.Date;
            int newEmployeeId = -1;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"INSERT INTO dbo.Entities (Description, TemplateId)
                VALUES (@FullName, @TemplateId); SELECT @@IDENTITY";

            var cmd = new SqlCommand(sql);
            AddCommandParameter(cmd, "@FullName", fullName, SqlDbType.VarChar);
            AddCommandParameter(cmd, "@TemplateId", templateId, SqlDbType.Int);
            cmd.Connection = connection;

            var idObject = await cmd.ExecuteScalarAsync();
            if (idObject != DBNull.Value)
            {
                newEmployeeId = Convert.ToInt32(idObject);
            }

            if (newEmployeeId <= 0)
            {
                return newEmployeeId;
            }

            await InsertMutations(new List<Mutation>
            {
                new Mutation
                {
                    EntityId = newEmployeeId,
                    DataElementId = DataElement.LastName,
                    FieldValue = lastName,
                    StartDate = startDate,
                    EndDate = endDate

                },
                new Mutation
                {
                    EntityId = newEmployeeId,
                    DataElementId = DataElement.FirstNames,
                    FieldValue = firstName,
                    StartDate = startDate,
                    EndDate = endDate
                }
            });

            return newEmployeeId;
        }

        private async Task SetMutationDeleted(DateTime referenceDate,
            int entityId, int dataElementId)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE dbo.Mutations SET Deleted = 1, EndDate = @ReferenceDate
                    WHERE EntityId = @EntityId AND DataElementId = @DataElementId
                    AND @ReferenceDate BETWEEN StartDate and EndDate";

            var cmd = new SqlCommand(sql);
            AddCommandParameter(cmd, "@ReferenceDate", referenceDate, SqlDbType.DateTime);
            AddCommandParameter(cmd, "@EntityId", entityId, SqlDbType.Int);
            AddCommandParameter(cmd, "@DataElementId", dataElementId, SqlDbType.Int);
            cmd.Connection = connection;

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task InsertMutations(IEnumerable<Mutation> mutations)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var mutation in mutations)
            {
                const string sql = @"
                    INSERT INTO dbo.Mutations (EntityId, DataElementId, FieldValue, StartDate, EndDate)
                        VALUES (@EntityId, @DataElementId, @FieldValue, @StartDate, @EndDate);";

                var cmd = new SqlCommand(sql);
                AddCommandParameter(cmd, "@EntityId", mutation.EntityId, SqlDbType.Int);
                AddCommandParameter(cmd, "@DataElementId", mutation.DataElementId, SqlDbType.Int);
                AddCommandParameter(cmd, "@FieldValue", mutation.FieldValue, SqlDbType.VarChar);
                AddCommandParameter(cmd, "@StartDate", mutation.StartDate, SqlDbType.DateTime);
                AddCommandParameter(cmd, "@EndDate", mutation.EndDate, SqlDbType.DateTime);
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