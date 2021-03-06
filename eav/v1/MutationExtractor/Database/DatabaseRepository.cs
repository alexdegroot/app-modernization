using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MutationExtractor.Database
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly ILogger<DatabaseRepository> _logger;
        private readonly string _connectionString;

        public DatabaseRepository(IOptions<Configuration> config, ILogger<DatabaseRepository> logger)
        {
            _logger = logger;
            _connectionString = config.Value.Sql_Connectionstring ??
                                throw new ArgumentException(nameof(config.Value.Sql_Connectionstring));
        }

        public async Task<bool> Verify(CancellationToken cancellationToken)
        {
            for(var i = 1; i <= 100; i++)
            {
                _logger.LogInformation("Connection to database, attempt: " + i);
                if (await CanConnectToDatabase(cancellationToken))
                {
                    return true;
                }
                await Task.Delay(1000, cancellationToken);
            }

            return false;
        }

        private async Task<bool> CanConnectToDatabase(CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result.ToString() == "1";
            }
            catch (SqlException e)
            {
                _logger.LogError("Can't verify the database connection.", e);
                _logger.LogError(e.Message, e);
            }

            return false;
        }

        public async IAsyncEnumerable<Change> GetChanges([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"SELECT 
                                    e.TenantId,
                                    e.Id as EntityId,
                                    e.ParentId as EntityParentId,
                                    e.TemplateId as EntityTemplateId,
                                    e.StartDate as EntityStartDate,
                                    e.EndDate as EntityEndDate,
                                    e.Deleted as EntityDeleted,
                                    m.Id as MutationId,
                                    m.DataElementId as FieldId,
                                    m.FieldValue,
                                    m.MutationDateTime,
                                    m.StartDate as MutationStartDate,
                                    m.EndDate as MutationEndDate,
                                    m.Deleted as MutationDeleted
                                FROM dbo.Entities as e
                                LEFT JOIN dbo.Mutations as m
                                ON m.EntityId = e.Id";
            var command = connection.CreateCommand();
            command.CommandText = sql;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var change = new Change
                {
                    TenantId = await GetColumnValue<int>(reader, nameof(Change.TenantId)),
                    EntityId = await GetColumnValue<int>(reader, nameof(Change.EntityId)),
                    EntityParentId = await GetColumnValue<int>(reader, nameof(Change.EntityParentId)),
                    EntityTemplateId = await GetColumnValue<int>(reader, nameof(Change.EntityTemplateId)),
                    EntityStartDate = await GetColumnValue<DateTime>(reader, nameof(Change.EntityStartDate)),
                    EntityEndDate = await GetColumnValue<DateTime>(reader, nameof(Change.EntityEndDate)),
                    EntityDeleted = await GetColumnValue<bool>(reader, nameof(Change.EntityDeleted)),
                    MutationId = await GetColumnValue<int>(reader, nameof(Change.MutationId)),
                    FieldId = await GetColumnValue<int>(reader, nameof(Change.FieldId)),
                    FieldValue = await GetColumnValue<object>(reader, nameof(Change.FieldValue)),
                    MutationDateTime = await GetColumnValue<DateTime>(reader, nameof(Change.MutationDateTime)),
                    MutationStartDate = await GetColumnValue<DateTime>(reader, nameof(Change.MutationStartDate)),
                    MutationEndDate = await GetColumnValue<DateTime>(reader, nameof(Change.MutationEndDate)),
                    MutationDeleted = await GetColumnValue<bool>(reader, nameof(Change.MutationDeleted))
                };
                yield return change;
            }
        }

        private async Task<TReturnType> GetColumnValue<TReturnType>(SqlDataReader dataReader, string columnName)
        {
            try
            {
                var columnOrdinal = dataReader.GetOrdinal(columnName);
                bool isNull = await dataReader.IsDBNullAsync(columnOrdinal);
                if (!isNull)
                {
                    var fieldValue = await dataReader.GetFieldValueAsync<TReturnType>(columnOrdinal);
                    return fieldValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred!", ex);
            }
            return default;
        }
    }
}