using System;
using System.Collections.Generic;
using System.Linq;
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
            _connectionString = config.Value.Sql_Connectionstring ?? throw new ArgumentException(nameof(config.Value.Sql_Connectionstring));
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

        public IAsyncEnumerable<Change> GetChanges(CancellationToken cancellationToken)
        {
            // TODO: Fetch changes
            return Enumerable.Range(0, 10).Select(x => new Change()).ToAsyncEnumerable();
        }
    }
}