using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WriteApi
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Update(int employeeId, Employee employee)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var c = connection.CreateCommand();
            c.CommandText = "SELECT 1";
            await c.ExecuteNonQueryAsync();
        }

        public async Task<int> Add(Employee employee)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var c = connection.CreateCommand();
            c.CommandText = "SELECT 1";
            return await c.ExecuteNonQueryAsync();
        }

    }
}