namespace DynamicAppGenerator.Services
{
    public class ProjectTemplateService
    {
        private readonly Dictionary<string, string> _templates;

        public ProjectTemplateService()
        {
            _templates = new Dictionary<string, string>
            {
                ["Program"] = @"using Microsoft.AspNetCore.Authentication.Cookies;
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
builder.Services.AddScoped<{{Namespace}}.Services.UserService>();
builder.Services.AddScoped<{{Namespace}}.Services.RoleService>();
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
    pattern: ""{controller=Account}/{action=Register}/{id?}"");
app.Run();",

                ["Appsettings"] = @"{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Server=DESKTOP-GQM2C7S\\LOCALHOST;Database={{Namespace}};trusted_connection=true;TrustServerCertificate=True""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft"": ""Warning"",
      ""Microsoft.Hosting.Lifetime"": ""Information""
    }
  },
  ""AllowedHosts"": ""*""
}",

                ["AppsettingsDev"] = @"{
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Debug"",
      ""Microsoft"": ""Information"",
      ""Microsoft.Hosting.Lifetime"": ""Information""
    }
  }
}",

                ["Csproj"] = @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Dapper"" Version=""2.1.66"" />
    <PackageReference Include=""System.Data.SqlClient"" Version=""4.9.0"" />
  </ItemGroup>
</Project>"
            };
        }

        public string GetProjectTemplate(string templateName)
        {
            if (_templates.TryGetValue(templateName, out string template))
            {
                return template;
            }

            throw new KeyNotFoundException($"Template '{templateName}' not found.");
        }

        public Dictionary<string, string> GetAllTemplates()
        {
            return new Dictionary<string, string>(_templates);
        }

        public string RenderTemplate(string templateName, string namespaceName)
        {
            string template = GetProjectTemplate(templateName);
            return template.Replace("{{Namespace}}", namespaceName);
        }

        public async Task GenerateProjectFilesAsync(string appPath)
        {
            string namespaceName = Path.GetFileName(appPath);

            // Generate Program.cs
            string programContent = RenderTemplate("Program", namespaceName);
            await File.WriteAllTextAsync(Path.Combine(appPath, "Program.cs"), programContent);

            // Generate appsettings.json
            string appsettingsContent = RenderTemplate("Appsettings", namespaceName);
            await File.WriteAllTextAsync(Path.Combine(appPath, "appsettings.json"), appsettingsContent);

            // Generate appsettings.Development.json
            string appsettingsDevContent = RenderTemplate("AppsettingsDev", namespaceName);
            await File.WriteAllTextAsync(Path.Combine(appPath, "appsettings.Development.json"), appsettingsDevContent);

            // Generate projectName.csproj
            string csprojContent = RenderTemplate("Csproj", namespaceName);
            await File.WriteAllTextAsync(Path.Combine(appPath, $"{namespaceName}.csproj"), csprojContent);
        }
    }
}
