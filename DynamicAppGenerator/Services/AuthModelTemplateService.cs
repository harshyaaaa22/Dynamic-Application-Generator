namespace DynamicAppGenerator.Services
{
    public class AuthModelTemplateService
    {
        private readonly Dictionary<string, string> _templates;

        public AuthModelTemplateService()
        {
            _templates = new Dictionary<string, string>
            {
                ["User"] = @"using System;
using System.ComponentModel.DataAnnotations;
namespace {{Namespace}}.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string PasswordSalt { get; set; }
        
        public int RoleId { get; set; }
        
        public Role Role { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}",
                ["Role"] = @"using System.ComponentModel.DataAnnotations;
namespace {{Namespace}}.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(200)]
        public string Description { get; set; }
    }
}",
                ["RegisterViewModel"] = @"using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace {{Namespace}}.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = ""First Name"")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = ""Last Name"")]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = ""Email"")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = ""Role"")]
        public int RoleId { get; set; }
        
        public List<Role> AvailableRoles { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = ""The {0} must be at least {2} characters long."", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = ""Password"")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = ""Confirm password"")]
        [Compare(""Password"", ErrorMessage = ""The password and confirmation password do not match."")]
        public string ConfirmPassword { get; set; }
    }
}",
                ["LoginViewModel"] = @"using System.ComponentModel.DataAnnotations;
namespace {{Namespace}}.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Display(Name = ""Remember me?"")]
        public bool RememberMe { get; set; }
    }
}"
            };
        }

        public string GetModelTemplate(string modelName)
        {
            if (_templates.TryGetValue(modelName, out string template))
            {
                return template;
            }

            throw new KeyNotFoundException($"Template for model '{modelName}' not found.");
        }

        public Dictionary<string, string> GetAllTemplates()
        {
            return new Dictionary<string, string>(_templates);
        }

        public string RenderTemplate(string modelName, string namespaceName)
        {
            string template = GetModelTemplate(modelName);
            return template.Replace("{{Namespace}}", namespaceName);
        }

        public async Task GenerateAuthenticationModelsAsync(string appPath)
        {
            string namespaceName = Path.GetFileName(appPath);
            string modelsPath = Path.Combine(appPath, "Models");

            // Ensure the directory exists
            Directory.CreateDirectory(modelsPath);

            // Generate all models
            foreach (var modelName in _templates.Keys)
            {
                string content = RenderTemplate(modelName, namespaceName);
                await File.WriteAllTextAsync(Path.Combine(modelsPath, $"{modelName}.cs"), content);
            }
        }

        // Generate specific model
        public async Task GenerateModelAsync(string appPath, string modelName)
        {
            string namespaceName = Path.GetFileName(appPath);
            string modelsPath = Path.Combine(appPath, "Models");

            // Ensure the directory exists
            Directory.CreateDirectory(modelsPath);

            // Generate the requested model
            string content = RenderTemplate(modelName, namespaceName);
            await File.WriteAllTextAsync(Path.Combine(modelsPath, $"{modelName}.cs"), content);
        }
    }
}
