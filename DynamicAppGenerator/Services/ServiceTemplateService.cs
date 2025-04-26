namespace DynamicAppGenerator.Services
{
    public class ServiceTemplateService
    {
        private readonly Dictionary<string, string> _templates;

        public ServiceTemplateService()
        {
            _templates = new Dictionary<string, string>
            {
                ["UserService"] = @"using Dapper;
using {{Namespace}}.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace {{Namespace}}.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(""DefaultConnection"");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(""SELECT * FROM Users WHERE Id = @Id"", new { Id = id });
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(""SELECT * FROM Users WHERE Email = @Email"", new { Email = email });
        }

        public async Task<int> CreateUserAsync(User user, string password)
        {
            using var connection = CreateConnection();
            CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = DateTime.Now;

            var sql = @""
                INSERT INTO Users (FirstName, LastName, Email, PasswordHash, PasswordSalt, RoleId, CreatedAt)
                VALUES (@FirstName, @LastName, @Email, @PasswordHash, @PasswordSalt, @RoleId, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)"";

            return await connection.QuerySingleAsync<int>(sql, user);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            using var connection = CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(""SELECT * FROM Users WHERE Email = @Email"", new { Email = email });

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
}",
                ["RoleService"] = @"using Dapper;
using {{Namespace}}.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace {{Namespace}}.Services
{
    public class RoleService
    {
        private readonly string _connectionString;

        public RoleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(""DefaultConnection"");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Role>(""SELECT * FROM Roles"");
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Role>(""SELECT * FROM Roles WHERE Id = @Id"", new { Id = id });
        }
    }
}"
            };
        }

        public string GetServiceTemplate(string serviceName)
        {
            if (_templates.TryGetValue(serviceName, out string template))
            {
                return template;
            }

            throw new KeyNotFoundException($"Template for service '{serviceName}' not found.");
        }

        public Dictionary<string, string> GetAllTemplates()
        {
            return new Dictionary<string, string>(_templates);
        }

        public string RenderTemplate(string serviceName, string namespaceName)
        {
            string template = GetServiceTemplate(serviceName);
            return template.Replace("{{Namespace}}", namespaceName);
        }

        public async Task GenerateServicesAsync(string appPath)
        {
            string namespaceName = Path.GetFileName(appPath);
            string servicesPath = Path.Combine(appPath, "Services");

            // Ensure the directory exists
            Directory.CreateDirectory(servicesPath);

            // Generate all services
            foreach (var serviceName in _templates.Keys)
            {
                string content = RenderTemplate(serviceName, namespaceName);
                await File.WriteAllTextAsync(Path.Combine(servicesPath, $"{serviceName}.cs"), content);
            }
        }

        // Generate specific service
        public async Task GenerateServiceAsync(string appPath, string serviceName)
        {
            string namespaceName = Path.GetFileName(appPath);
            string servicesPath = Path.Combine(appPath, "Services");

            // Ensure the directory exists
            Directory.CreateDirectory(servicesPath);

            // Generate the requested service
            string content = RenderTemplate(serviceName, namespaceName);
            await File.WriteAllTextAsync(Path.Combine(servicesPath, $"{serviceName}.cs"), content);
        }
    }
}
