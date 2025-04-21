using DynamicAppGenerator.Models;
using DynamicAppGenerator.Repositories;

namespace DynamicAppGenerator.Services
{
    public class GeneratorService
    {
        private readonly RoleRepository _roleRepository;
        private readonly TemplateService _templateService;

        public GeneratorService(RoleRepository roleRepository, TemplateService templateService)
        {
            _roleRepository = roleRepository;
            _templateService = templateService;
        }

        public async Task<string> GenerateApplicationAsync(GeneratorConfig config)
        {
            // Create base directory if it doesn't exist
            var appPath = Path.Combine(config.OutputPath, config.ApplicationName);
            if (!Directory.Exists(config.OutputPath))
            {
                Directory.CreateDirectory(config.OutputPath);
            }

            if (Directory.Exists(appPath))
            {
                return $"Error: Application directory already exists at {appPath}";
            }

            try
            {
                // Create application directory structure
                Directory.CreateDirectory(appPath);

                // Create subdirectories
                var directories = new[]
                {
                        "Controllers",
                        "Models",
                        "Views",
                        "Views\\Account",
                        "Views\\Home",
                        "Views\\Shared",
                        "wwwroot",
                        "wwwroot\\css",
                        "wwwroot\\js",
                        "Services",
                        "Scripts"
                    };

                foreach (var dir in directories)
                {
                    Directory.CreateDirectory(Path.Combine(appPath, dir));
                }

                // Get all roles from the database
                var roles = await _roleRepository.GetAllRolesAsync();

                // Generate database script
                var dbScript = _templateService.GenerateDatabaseScript(roles);
                await File.WriteAllTextAsync(Path.Combine(appPath, "Scripts", "DatabaseSetup.sql"), dbScript);

                // Generate authentication models
                await GenerateAuthenticationModelsAsync(appPath);

                // Generate controllers
                await GenerateControllersAsync(appPath);

                // Generate views
                await GenerateViewsAsync(appPath, roles);

                // Generate services
                await GenerateServicesAsync(appPath);

                // Generate project file and startup classes
                await GenerateProjectFilesAsync(appPath);

                return $"Application successfully generated at {appPath}";
            }
            catch (Exception ex)
            {
                // In case of error, try to clean up
                if (Directory.Exists(appPath))
                {
                    try { Directory.Delete(appPath, true); } catch { }
                }

                return $"Error generating application: {ex.Message}";
            }
        }

        private async Task GenerateAuthenticationModelsAsync(string appPath)
        {
            var userModel = @"using System;
using System.ComponentModel.DataAnnotations;

namespace " + Path.GetFileName(appPath) + @".Models
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
}";

            var roleModel = @"using System.ComponentModel.DataAnnotations;

namespace " + Path.GetFileName(appPath) + @".Models
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
}";

            var registerViewModel = @"using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace " + Path.GetFileName(appPath) + @".Models
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
}";

            var loginViewModel = @"using System.ComponentModel.DataAnnotations;

namespace " + Path.GetFileName(appPath) + @".Models
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
}";

            await File.WriteAllTextAsync(Path.Combine(appPath, "Models", "User.cs"), userModel);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Models", "Role.cs"), roleModel);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Models", "RegisterViewModel.cs"), registerViewModel);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Models", "LoginViewModel.cs"), loginViewModel);
        }

        private async Task GenerateControllersAsync(string appPath)
        {
            var homeController = @"using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using " + Path.GetFileName(appPath) + @".Models;

namespace " + Path.GetFileName(appPath) + @".Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [Authorize(Roles = ""Admin"")]
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}";

            var accountController = @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using " + Path.GetFileName(appPath) + @".Models;
using " + Path.GetFileName(appPath) + @".Services;

namespace " + Path.GetFileName(appPath) + @".Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        
        public AccountController(UserService userService, RoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var viewModel = new RegisterViewModel
            {
                AvailableRoles = (List<Role>)await _roleService.GetAllRolesAsync()
            };
            
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userService.GetByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("""", ""Email already in use"");
                    model.AvailableRoles = (List<Role>)await _roleService.GetAllRolesAsync();
                    return View(model);
                }
                
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    RoleId = model.RoleId
                };
                
                await _userService.CreateUserAsync(user, model.Password);
                
                return RedirectToAction(""Login"");
            }
            
            model.AvailableRoles = (List<Role>)await _roleService.GetAllRolesAsync();
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateAsync(model.Email, model.Password);
                
                if (user == null)
                {
                    ModelState.AddModelError("""", ""Invalid login attempt."");
                    return View(model);
                }
                
                var role = await _roleService.GetRoleByIdAsync(user.RoleId);
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $""{user.FirstName} {user.LastName}""),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role.Name)
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };
                
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                
                return RedirectToAction(""Index"", ""Home"");
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(""Register"", ""Account"");
        }
    }
}";


            await File.WriteAllTextAsync(Path.Combine(appPath, "Controllers", "HomeController.cs"), homeController);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Controllers", "AccountController.cs"), accountController);
        }


        private async Task GenerateViewsAsync(string appPath, IEnumerable<Role> roles)
        {
            // Generate layout with auth-styles.css reference
            var layout = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@ViewData[""Title""] - " + Path.GetFileName(appPath) + @"</title>
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" />
    <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css2?family=Nunito:wght@400;600;700&display=swap"" />
    <link rel=""stylesheet"" href=""~/css/site.css"" />
  
<link rel=""stylesheet"" href=""~/css/LayoutStyles.css"" asp-append-version=""true"" />
</head>
<body class=""@(ViewContext.RouteData.Values[""controller""].ToString() == ""Account"" ? ""auth-page"" : """")"">
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""navbar-brand"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">" + Path.GetFileName(appPath) + @"</a>
                <button class=""navbar-toggler"" type=""button"" data-toggle=""collapse"" data-target="".navbar-collapse"">
                    <span class=""navbar-toggler-icon""></span>
                </button>
                <div class=""navbar-collapse collapse d-sm-inline-flex justify-content-between"">
                    <ul class=""navbar-nav flex-grow-1"">
                        <li class=""nav-item"">
                            <a class=""nav-link text-dark"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">Home</a>
                        </li>
                        <li class=""nav-item"">
                            <a class=""nav-link text-dark"" asp-area="""" asp-controller=""Home"" asp-action=""Privacy"">Privacy</a>
                        </li>
                    </ul>
                    <partial name=""_LoginPartial"" />
                </div>
            </div>
        </nav>
    </header>
    <div class=""container"">
        <main role=""main"" class=""pb-3"">
            @RenderBody()
        </main>
    </div>

   

    <script src=""https://code.jquery.com/jquery-3.5.1.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.2/jquery.validate.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.11/jquery.validate.unobtrusive.min.js""></script>
    <script src=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.bundle.min.js""></script>
    <script src=""~/js/site.js"" asp-append-version=""true""></script>
    @await RenderSectionAsync(""Scripts"", required: false)
</body>
</html>";

            // Generate Login.cshtml with auth-styles classes
            var login = @"@model " + Path.GetFileName(appPath) + @".Models.LoginViewModel
@{
    ViewData[""Title""] = ""Login"";
    Layout = null;
}
<link rel=""stylesheet"" href=""~/css/auth-styles.css"" asp-append-version=""true"" />
<body class=""auth-page"">
    <div class=""auth-container"">
        <!-- Left side branding panel -->
        <!-- Right side form panel -->
        <div class=""auth-form-container"">
            <div class=""auth-form-header"">
                <h2 class=""auth-form-title"">LogIn</h2>

            </div>

            <form asp-controller=""Account"" asp-action=""Login"" method=""post"">
                <div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
                
                <div class=""form-group"">
                    <label asp-for=""Email"" class=""control-label""></label>
                    <input asp-for=""Email"" class=""form-control"" placeholder=""Enter your email"" />
                    <span asp-validation-for=""Email"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""Password"" class=""control-label""></label>
                    <input asp-for=""Password"" class=""form-control"" placeholder=""Enter your password"" />
                    <span asp-validation-for=""Password"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <div class=""checkbox"">
                        <input asp-for=""RememberMe"" />
                        <label asp-for=""RememberMe""></label>
                    </div>
                </div>
                
                <div class=""form-group"">
                    <button type=""submit"" class=""btn btn-primary"">Login</button>
                </div>
            </form>
         <div class=""auth-footer"">
                Already have an account? <a asp-action=""Register"">Register here</a>
            </div>

        </div>
    </div>


@section Scripts {
    @{await Html.RenderPartialAsync(""_ValidationScriptsPartial"");}
}

</body>";

            // Generate Register.cshtml with auth-styles classes
            var register = @"@model " + Path.GetFileName(appPath) + @".Models.RegisterViewModel
@{
    ViewData[""Title""] = ""Register"";
    Layout = null;
}
<link rel=""stylesheet"" href=""~/css/auth-styles.css"" asp-append-version=""true"" />
<body class=""auth-page"">
    <div class=""auth-container"">
        <!-- Left side branding panel -->
      

        <!-- Right side form panel -->
        <div class=""auth-form-container"">
            <div class=""auth-form-header"">
                <h2 class=""auth-form-title"">Register</h2>
              
            </div>

            <form asp-controller=""Account"" asp-action=""Register"" method=""post"">
                <div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
                
                <div class=""form-group"">
                    <label asp-for=""FirstName"" class=""control-label""></label>
                    <input asp-for=""FirstName"" class=""form-control"" placeholder=""Enter your first name"" />
                    <span asp-validation-for=""FirstName"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""LastName"" class=""control-label""></label>
                    <input asp-for=""LastName"" class=""form-control"" placeholder=""Enter your last name"" />
                    <span asp-validation-for=""LastName"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""Email"" class=""control-label""></label>
                    <input asp-for=""Email"" class=""form-control"" placeholder=""Enter your email"" />
                    <span asp-validation-for=""Email"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""RoleId"" class=""control-label""></label>
                    <select asp-for=""RoleId"" class=""form-control"">
                        <option value="""">-- Select Role --</option>
                        @foreach (var role in Model.AvailableRoles)
                        {
                            <option value=""@role.Id"">@role.Name</option>
                        }
                    </select>
                    <span asp-validation-for=""RoleId"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""Password"" class=""control-label""></label>
                    <input asp-for=""Password"" class=""form-control password-input"" placeholder=""Enter your password"" />
                    <div class=""password-strength""></div>
                    <span asp-validation-for=""Password"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <label asp-for=""ConfirmPassword"" class=""control-label""></label>
                    <input asp-for=""ConfirmPassword"" class=""form-control"" placeholder=""Confirm your password"" />
                    <span asp-validation-for=""ConfirmPassword"" class=""text-danger""></span>
                </div>
                
                <div class=""form-group"">
                    <button type=""submit"" class=""btn btn-primary"">Register</button>
                </div>
            </form>
      
            <div class=""auth-footer"">
                Already have an account? <a asp-action=""Login"">Sign in</a>
            </div>
        </div>
    </div>

@section Scripts {
    @{await Html.RenderPartialAsync(""_ValidationScriptsPartial"");}
    <script>
        // Simple password strength indicator
        $(document).ready(function() {
            $('.password-input').on('input', function() {
                var password = $(this).val();
                var strength = 0;
                
                if (password.length >= 6) strength += 1;
                if (password.match(/[A-Z]/)) strength += 1;
                if (password.match(/[0-9]/)) strength += 1;
                if (password.match(/[^a-zA-Z0-9]/)) strength += 1;
                
                var strengthBar = $(this).siblings('.password-strength');
                strengthBar.removeClass('weak medium strong very-strong');
                
                if (password.length === 0) {
                    strengthBar.css('width', '0%');
                } else if (strength <= 1) {
                    strengthBar.addClass('weak');
                } else if (strength === 2) {
                    strengthBar.addClass('medium');
                } else if (strength === 3) {
                    strengthBar.addClass('strong');
                } else {
                    strengthBar.addClass('very-strong');
                }
            });
        });
    </script>
}

</body>";

            // Generate _ValidationScriptsPartial.cshtml
            var validationScriptsPartial = @"<script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.2/jquery.validate.min.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.11/jquery.validate.unobtrusive.min.js""></script>";

            // Generate Home/Index.cshtml
            var homeIndex = await File.ReadAllTextAsync("Templates/Views/Home/Index.cshtml");


            // Generate Home/Privacy.cshtml

            var homePrivacy = await File.ReadAllTextAsync("Views/Home/Privacy.cshtml");
           

            // Generate _LoginPartial
            var loginPartial = @"@using Microsoft.AspNetCore.Authentication
@using System.Security.Claims

@if (User.Identity.IsAuthenticated)
{
    <ul class=""navbar-nav"">
        <li class=""nav-item"">
            <span class=""nav-link text-dark"">Hello @User.Identity.Name!</span>
        </li>
        <li class=""nav-item"">
            <form class=""form-inline"" asp-controller=""Account"" asp-action=""Logout"" method=""post"">
                <button type=""submit"" class=""nav-link btn btn-link text-dark"">Logout</button>
            </form>
        </li>
    </ul>
}
else
{
    <ul class=""navbar-nav"">
        <li class=""nav-item"">
            <a class=""nav-link text-dark"" asp-controller=""Account"" asp-action=""Register"">Register</a>
        </li>
        <li class=""nav-item"">
            <a class=""nav-link text-dark"" asp-controller=""Account"" asp-action=""Login"">Login</a>
        </li>
    </ul>
}";
            var siteCss = await File.ReadAllTextAsync("wwwroot/css/site.css");
            // Create site.css
            

            // Create site.js
            var siteJs = await File.ReadAllTextAsync("wwwroot/js/site.js");

            // Create auth-styles.css
            var authStyles = await File.ReadAllTextAsync("wwwroot/css/auth-styles.css"); // Use the content from the artifact
            var LayoutStyles = await File.ReadAllTextAsync("wwwroot/css/LayoutStyles.css");
            // Write all view files
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Shared", "_Layout.cshtml"), layout);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Shared", "_LoginPartial.cshtml"), loginPartial);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Shared", "_ValidationScriptsPartial.cshtml"), validationScriptsPartial);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Home", "Index.cshtml"), homeIndex);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Home", "Privacy.cshtml"), homePrivacy);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Account", "Login.cshtml"), login);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "Account", "Register.cshtml"), register);
            await File.WriteAllTextAsync(Path.Combine(appPath, "wwwroot", "css", "site.css"), siteCss);
            await File.WriteAllTextAsync(Path.Combine(appPath, "wwwroot", "css", "auth-styles.css"), authStyles);
            await File.WriteAllTextAsync(Path.Combine(appPath, "wwwroot", "css", "LayoutStyles.css"), LayoutStyles);
            await File.WriteAllTextAsync(Path.Combine(appPath, "wwwroot", "js", "site.js"), siteJs);



            // Create _ViewImports.cshtml
            var viewImports = $@"@using {Path.GetFileName(appPath)}
@using {Path.GetFileName(appPath)}.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers";
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "_ViewImports.cshtml"), viewImports);

            // Create _ViewStart.cshtml
            var viewStart = @"@{
    Layout = ""_Layout"";
}";
            await File.WriteAllTextAsync(Path.Combine(appPath, "Views", "_ViewStart.cshtml"), viewStart);
        }

        private async Task GenerateServicesAsync(string appPath)
        {
            var userService = @"using Dapper;
using " + Path.GetFileName(appPath) + @".Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace " + Path.GetFileName(appPath) + @".Services
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
}";

            var roleService = @"using Dapper;
using " + Path.GetFileName(appPath) + @".Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace " + Path.GetFileName(appPath) + @".Services
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
}";

            await File.WriteAllTextAsync(Path.Combine(appPath, "Services", "UserService.cs"), userService);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Services", "RoleService.cs"), roleService);
        }

        private async Task GenerateProjectFilesAsync(string appPath)
        {
            var projectName = Path.GetFileName(appPath);

            // Modern Program.cs (Minimal Hosting Model)
            var program = @$"using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {{
        options.LoginPath = ""/Account/Login"";
        options.LogoutPath = ""/Account/Logout"";
    }});

// Register application services
builder.Services.AddScoped<{projectName}.Services.UserService>();
builder.Services.AddScoped<{projectName}.Services.RoleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{{
    app.UseExceptionHandler(""/Account/Register"");
    app.UseHsts();
}}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: ""default"",
    pattern: ""{{controller=Account}}/{{action=Register}}/{{id?}}"");

app.Run();
";

            // appsettings.json
            var appsettings = @$"{{
  ""ConnectionStrings"": {{
    ""DefaultConnection"": ""Server=DESKTOP-GQM2C7S\\LOCALHOST;Database={projectName};trusted_connection=true;TrustServerCertificate=True""
  }},
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft"": ""Warning"",
      ""Microsoft.Hosting.Lifetime"": ""Information""
    }}
  }},
  ""AllowedHosts"": ""*""
}}";

            // appsettings.Development.json
            var appsettingsDev = @"{
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Debug"",
      ""Microsoft"": ""Information"",
      ""Microsoft.Hosting.Lifetime"": ""Information""
    }
  }
}";

            // Updated .csproj for .NET 8
            var csproj = @$"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Dapper"" Version=""2.1.66"" />
    <PackageReference Include=""System.Data.SqlClient"" Version=""4.9.0"" />
  </ItemGroup>

</Project>";

            // Write files
            await File.WriteAllTextAsync(Path.Combine(appPath, "Program.cs"), program);
            await File.WriteAllTextAsync(Path.Combine(appPath, "appsettings.json"), appsettings);
            await File.WriteAllTextAsync(Path.Combine(appPath, "appsettings.Development.json"), appsettingsDev);
            await File.WriteAllTextAsync(Path.Combine(appPath, $"{projectName}.csproj"), csproj);
        }

    }
}