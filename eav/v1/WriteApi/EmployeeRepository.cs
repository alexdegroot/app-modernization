using System.Data;

namespace WriteApi
{
    public class EmployeeRepository
    {
        private IDbConnection _connection;

        public EmployeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Update(in int employeeId, Employee employee)
        {
            throw new System.NotImplementedException();
        }

        public int Add(Employee employee)
        {
            _connection.Open();
            return -1;
        }
    }
}