namespace DynamicAppGenerator.Services
{
    public class ControllerTemplateService
    {
        private readonly Dictionary<string, string> _templates;

        public ControllerTemplateService()
        {
            _templates = new Dictionary<string, string>
            {
                ["HomeController"] = @"using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using {{Namespace}}.Models;

namespace {{Namespace}}.Controllers
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
}",
                ["AccountController"] = @"using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using {{Namespace}}.Models;
using {{Namespace}}.Services;

namespace {{Namespace}}.Controllers
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
}"
            };
        }

        public string GetControllerTemplate(string controllerName)
        {
            if (_templates.TryGetValue(controllerName, out string template))
            {
                return template;
            }

            throw new KeyNotFoundException($"Template for controller '{controllerName}' not found.");
        }

        public Dictionary<string, string> GetAllTemplates()
        {
            return new Dictionary<string, string>(_templates);
        }

        public string RenderTemplate(string controllerName, string namespaceName)
        {
            string template = GetControllerTemplate(controllerName);
            return template.Replace("{{Namespace}}", namespaceName);
        }

        public async Task GenerateControllersAsync(string appPath)
        {
            string namespaceName = Path.GetFileName(appPath);
            string controllersPath = Path.Combine(appPath, "Controllers");

            // Ensure the directory exists
            Directory.CreateDirectory(controllersPath);

            // Generate all controllers
            foreach (var controllerName in _templates.Keys)
            {
                string content = RenderTemplate(controllerName, namespaceName);
                await File.WriteAllTextAsync(Path.Combine(controllersPath, $"{controllerName}.cs"), content);
            }
        }

        // Generate specific controller
        public async Task GenerateControllerAsync(string appPath, string controllerName)
        {
            string namespaceName = Path.GetFileName(appPath);
            string controllersPath = Path.Combine(appPath, "Controllers");

            // Ensure the directory exists
            Directory.CreateDirectory(controllersPath);

            // Generate the requested controller
            string content = RenderTemplate(controllerName, namespaceName);
            await File.WriteAllTextAsync(Path.Combine(controllersPath, $"{controllerName}.cs"), content);
        }

        // Add a new controller template
        public void AddControllerTemplate(string controllerName, string templateContent)
        {
            _templates[controllerName] = templateContent;
        }
    }
}
