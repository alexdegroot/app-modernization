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
                _logger.LogError("Couldn't connect to database.", e);
            }

            return false;
        }

        public async IAsyncEnumerable<Change> GetChanges([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"SELECT Id, EntityId, DataElementId, FieldValue, StartDate,
                                    EndDate, Deleted FROM dbo.Mutations";
            var command = connection.CreateCommand();
            command.CommandText = sql;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var change = new Change
                {
                    MutationId = await GetColumnValue<int>(reader, "Id"),
                    EntityId = await GetColumnValue<int>(reader, "EntityId"),
                    FieldId = await GetColumnValue<int>(reader, "DataElementId"),
                    Value = await GetColumnValue<object>(reader, "DataElementId"),
                    StartDate = await GetColumnValue<DateTime>(reader, "StartDate"),
                    EndDate = await GetColumnValue<DateTime>(reader, "EndDate"),
                    IsDeleted = await GetColumnValue<bool>(reader, "Deleted"),
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