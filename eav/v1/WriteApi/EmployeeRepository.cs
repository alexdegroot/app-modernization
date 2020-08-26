using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WriteApi
{

    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString
                                ?? throw new ArgumentNullException(nameof(connectionString));
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
            SetMutationDeleted(startDate, employeeId, DataElement.LastName);
            SetMutationDeleted(startDate, employeeId, DataElement.FirstNames);
            InsertMutations(new List<Mutation>
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

            var insertEntityCommand = connection.CreateCommand();
            insertEntityCommand.CommandText = @$"INSERT INTO dbo.Entities (Description, TemplateId)
                VALUES ('{fullName}', {templateId}); SELECT @@IDENTITY";
            var idObject = await insertEntityCommand.ExecuteScalarAsync();
            if (idObject != DBNull.Value)
            {
                newEmployeeId = Convert.ToInt32(idObject);
            }

            if (newEmployeeId <= 0)
            {
                return newEmployeeId;
            }

            InsertMutations(new List<Mutation>
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

        private async void SetMutationDeleted(DateTime referenceDate,
            int entityId, int dataElementId)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var setMutationDeletedCommand = connection.CreateCommand();
            setMutationDeletedCommand.CommandText = @$"
                UPDATE dbo.Mutations SET Deleted = 1, EndDate = '{referenceDate}'
                    WHERE EntityId = {entityId} AND DataElementId = {dataElementId}
                    AND '{referenceDate}' BETWEEN StartDate and EndDate";
            await setMutationDeletedCommand.ExecuteNonQueryAsync();
        }

        private async void InsertMutations(IEnumerable<Mutation> mutations)
        {
            var sql = string.Empty;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var mutation in mutations)
            {
                sql += $@"
                    INSERT INTO dbo.Mutations (EntityId, DataElementId, FieldValue, StartDate, EndDate)
                        VALUES ({mutation.EntityId}, {mutation.DataElementId}, '{mutation.FieldValue}',
                                    '{mutation.StartDate}', '{mutation.EndDate}');";
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var insertMutationsCommand = connection.CreateCommand();
            insertMutationsCommand.CommandText = sql;
            await insertMutationsCommand.ExecuteNonQueryAsync();
        }
    }
}