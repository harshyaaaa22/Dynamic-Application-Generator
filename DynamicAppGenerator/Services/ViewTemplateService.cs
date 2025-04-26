namespace DynamicAppGenerator.Services
{
    public class ViewTemplateService
    {
        private readonly Dictionary<string, string> _templates;

        private readonly LayoutsService _layoutsService;

        public ViewTemplateService()
        {
            _templates = new Dictionary<string, string>

            {
                ["_Layout"] = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@ViewData[""Title""] - {{Namespace}}</title>
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"" />
    <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css2?family=Nunito:wght@400;600;700&display=swap"" />
    <link rel=""stylesheet"" href=""~/css/site.css"" />
  
<link rel=""stylesheet"" href=""~/css/LayoutStyles.css"" asp-append-version=""true"" />
</head>
<body class=""@(ViewContext.RouteData.Values[""controller""].ToString() == ""Account"" ? ""auth-page"" : """")"">
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""navbar-brand"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">{{Namespace}}</a>
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
   
            @RenderBody()
      

   

    <script src=""https://code.jquery.com/jquery-3.5.1.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.2/jquery.validate.min.js""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.11/jquery.validate.unobtrusive.min.js""></script>
    <script src=""https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.bundle.min.js""></script>
    <script src=""~/js/site.js"" asp-append-version=""true""></script>
    @await RenderSectionAsync(""Scripts"", required: false)
</body>
</html>",
                ["Login"] = @"@model {{Namespace}}.Models.LoginViewModel
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

</body>",
                ["Register"] = @"@model {{Namespace}}.Models.RegisterViewModel
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

</body>",
                ["_ValidationScriptsPartial"] = @"<script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.2/jquery.validate.min.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/3.2.11/jquery.validate.unobtrusive.min.js""></script>",
                ["_LoginPartial"] = @"@using Microsoft.AspNetCore.Authentication
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
}",
                ["_ViewImports"] = @"@using {{Namespace}}
@using {{Namespace}}.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers",
                ["_ViewStart"] = @"@{
    Layout = ""_Layout"";
}"
            };
            
            // Add CSS templates
            _templates["site.css"] = "/* Your site.css content here */";
            _templates["auth-styles.css"] = "/* Your auth-styles.css content here */";
            _templates["LayoutStyles.css"] = "/* Your LayoutStyles.css content here */";
            _templates["site.js"] = "// Your site.js content here";
            
            // Add Home views
            _templates["HomeIndex"] = "<!-- Your Home/Index.cshtml content here -->";
            _templates["HomePrivacy"] = "<!-- Your Home/Privacy.cshtml content here -->";
        }

        public LayoutTemplate CurrentLayout { get; private set; }

        // Add this method to set the current layout
        public void SetLayout(LayoutTemplate layout)
        {
            CurrentLayout = layout;

            // Set the _Layout template to use the selected layout content
            if (layout != null)
            {
                _templates["_Layout"] = layout.LayoutContent;
                _templates["LayoutStyles.css"] = layout.CssContent;
            }
        }

        // Existing methods...

        // Get a view template by name
        public string GetViewTemplate(string templateName)
        {
            if (_templates.TryGetValue(templateName, out string template))
            {
                return template;
            }

            throw new KeyNotFoundException($"Template '{templateName}' not found.");
        }

        // Get all templates
        public Dictionary<string, string> GetAllTemplates()
        {
            return new Dictionary<string, string>(_templates);
        }

        // Render a template with the namespace replaced
        public string RenderTemplate(string templateName, string namespaceName)
        {
            string template = GetViewTemplate(templateName);
            return template.Replace("{{Namespace}}", namespaceName);
        }

        // Add or update a template
        public void AddTemplate(string templateName, string content)
        {
            _templates[templateName] = content;
        }

        // Load template content from file
        public async Task LoadTemplateFromFileAsync(string templateName, string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = await File.ReadAllTextAsync(filePath);
                _templates[templateName] = content;
            }
            else
            {
                throw new FileNotFoundException($"Template file not found: {filePath}");
            }
        }

        // Generate all views
        public async Task GenerateViewsAsync(string appPath, IEnumerable<Models.Role> roles = null)
        {
            string namespaceName = Path.GetFileName(appPath);

            // Create directory structure
            Directory.CreateDirectory(Path.Combine(appPath, "Views", "Shared"));
            Directory.CreateDirectory(Path.Combine(appPath, "Views", "Home"));
            Directory.CreateDirectory(Path.Combine(appPath, "Views", "Account"));
            Directory.CreateDirectory(Path.Combine(appPath, "wwwroot", "css"));
            Directory.CreateDirectory(Path.Combine(appPath, "wwwroot", "js"));

            // Generate Shared views
            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Shared", "_Layout.cshtml"),
                RenderTemplate("_Layout", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Shared", "_LoginPartial.cshtml"),
                RenderTemplate("_LoginPartial", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Shared", "_ValidationScriptsPartial.cshtml"),
                RenderTemplate("_ValidationScriptsPartial", namespaceName));

            // Generate Home views
            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Home", "Index.cshtml"),
                RenderTemplate("HomeIndex", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Home", "Privacy.cshtml"),
                RenderTemplate("HomePrivacy", namespaceName));

            // Generate Account views
            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Account", "Login.cshtml"),
                RenderTemplate("Login", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "Account", "Register.cshtml"),
                RenderTemplate("Register", namespaceName));

            // Generate root views
            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "_ViewImports.cshtml"),
                RenderTemplate("_ViewImports", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "Views", "_ViewStart.cshtml"),
                RenderTemplate("_ViewStart", namespaceName));

            // Generate CSS and JS files
            await File.WriteAllTextAsync(
                Path.Combine(appPath, "wwwroot", "css", "site.css"),
                RenderTemplate("site.css", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "wwwroot", "css", "auth-styles.css"),
                RenderTemplate("auth-styles.css", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "wwwroot", "css", "LayoutStyles.css"),
                RenderTemplate("LayoutStyles.css", namespaceName));

            await File.WriteAllTextAsync(
                Path.Combine(appPath, "wwwroot", "js", "site.js"),
                RenderTemplate("site.js", namespaceName));
        }
        
        // Generate specific view
        public async Task GenerateViewAsync(string appPath, string viewName, string viewFolder = null)
        {
            string namespaceName = Path.GetFileName(appPath);
            string viewsPath = Path.Combine(appPath, "Views");
            
            if (viewFolder != null)
            {
                Directory.CreateDirectory(Path.Combine(viewsPath, viewFolder));
                await File.WriteAllTextAsync(
                    Path.Combine(viewsPath, viewFolder, $"{viewName}.cshtml"), 
                    RenderTemplate(viewName, namespaceName));
            }
            else
            {
                Directory.CreateDirectory(viewsPath);
                await File.WriteAllTextAsync(
                    Path.Combine(viewsPath, $"{viewName}.cshtml"), 
                    RenderTemplate(viewName, namespaceName));
            }
        }
    }
}
