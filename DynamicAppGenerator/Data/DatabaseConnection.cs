using System.Data;
using System.Data.SqlClient;

namespace DynamicAppGenerator.Data
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
