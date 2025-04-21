using Dapper;
using DynamicAppGenerator.Data;
using DynamicAppGenerator.Models;

namespace DynamicAppGenerator.Repositories
{
    public class RoleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public RoleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryAsync<Role>("SELECT * FROM Roles");
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Role>("SELECT * FROM Roles WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateRoleAsync(Role role)
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = "INSERT INTO Roles (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int)";
            return await connection.QuerySingleAsync<int>(sql, role);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            using var connection = _dbConnection.CreateConnection();
            await connection.ExecuteAsync("UPDATE Roles SET Name = @Name, Description = @Description WHERE Id = @Id", role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            using var connection = _dbConnection.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Roles WHERE Id = @Id", new { Id = id });
        }

        public async Task EnsureRolesTableExistsAsync()
        {
            using var connection = _dbConnection.CreateConnection();
            var sql = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
                BEGIN
                    CREATE TABLE Roles (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(50) NOT NULL,
                        Description NVARCHAR(200) NULL
                    )
                END";
            await connection.ExecuteAsync(sql);
        }
    }
}
