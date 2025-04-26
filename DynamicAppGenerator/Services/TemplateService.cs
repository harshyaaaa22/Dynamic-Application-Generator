using DynamicAppGenerator.Models;
using System.Text;

namespace DynamicAppGenerator.Services
{
    public class TemplateService
    {
        private readonly string _templateDir;

        public TemplateService(string templateDirectory)
        {
            _templateDir = templateDirectory;
        }

        public string GenerateRegisterPageTemplate(IEnumerable<Role> roles)
        {
            var sb = new StringBuilder();

            sb.AppendLine("@model RegisterViewModel");
            sb.AppendLine("@{");
            sb.AppendLine("    ViewData[\"Title\"] = \"Register\";"); 
            sb.AppendLine("    Layout = null;");
            sb.AppendLine("}");
            sb.AppendLine("<h2>Register</h2>");
            sb.AppendLine("<div class=\"row\">");
            sb.AppendLine("    <div class=\"col-md-6\">");
            sb.AppendLine("        <form asp-action=\"Register\" method=\"post\">");
            sb.AppendLine("            <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"FirstName\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"FirstName\" class=\"form-control\" />");
            sb.AppendLine("                <span asp-validation-for=\"FirstName\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"LastName\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"LastName\" class=\"form-control\" />");
            sb.AppendLine("                <span asp-validation-for=\"LastName\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"Email\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"Email\" class=\"form-control\" />");
            sb.AppendLine("                <span asp-validation-for=\"Email\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"RoleId\" class=\"control-label\">Role</label>");
            sb.AppendLine("                <select asp-for=\"RoleId\" class=\"form-control\">");
            sb.AppendLine("                    <option value=\"\">-- Select Role --</option>");

            foreach (var role in roles)
            {
                sb.AppendLine($"                    <option value=\"{role.Id}\">{role.Name}</option>");
            }

            sb.AppendLine("                </select>");
            sb.AppendLine("                <span asp-validation-for=\"RoleId\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"Password\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"Password\" class=\"form-control\" type=\"password\" />");
            sb.AppendLine("                <span asp-validation-for=\"Password\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"ConfirmPassword\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"ConfirmPassword\" class=\"form-control\" type=\"password\" />");
            sb.AppendLine("                <span asp-validation-for=\"ConfirmPassword\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <input type=\"submit\" value=\"Register\" class=\"btn btn-primary\" />");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </form>");



            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public string GenerateLoginPageTemplate()
        {
            var sb = new StringBuilder();

            sb.AppendLine("@model LoginViewModel");
            sb.AppendLine("@{");
            sb.AppendLine("    ViewData[\"Title\"] = \"Login\";");
            sb.AppendLine("    Layout = null;");
            sb.AppendLine("}");
            sb.AppendLine("<h2>Login</h2>");
            sb.AppendLine("<div class=\"row\">");
            sb.AppendLine("    <div class=\"col-md-6\">");
            sb.AppendLine("        <form asp-action=\"Login\" method=\"post\">");
            sb.AppendLine("            <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"Email\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"Email\" class=\"form-control\" />");
            sb.AppendLine("                <span asp-validation-for=\"Email\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <label asp-for=\"Password\" class=\"control-label\"></label>");
            sb.AppendLine("                <input asp-for=\"Password\" class=\"form-control\" type=\"password\" />");
            sb.AppendLine("                <span asp-validation-for=\"Password\" class=\"text-danger\"></span>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <div class=\"checkbox\">");
            sb.AppendLine("                    <label>");
            sb.AppendLine("                        <input asp-for=\"RememberMe\" /> @Html.DisplayNameFor(m => m.RememberMe)");
            sb.AppendLine("                    </label>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine("                <input type=\"submit\" value=\"Log in\" class=\"btn btn-primary\" />");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </form>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        public string GenerateDatabaseScript(IEnumerable<Role> roles)
        {
            var sb = new StringBuilder();

            sb.AppendLine("-- Database creation script for authentication and authorization");
            sb.AppendLine("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')");
            sb.AppendLine("BEGIN");
            sb.AppendLine("    CREATE TABLE Roles (");
            sb.AppendLine("        Id INT PRIMARY KEY IDENTITY(1,1),");
            sb.AppendLine("        Name NVARCHAR(50) NOT NULL,");
            sb.AppendLine("        Description NVARCHAR(200) NULL");
            sb.AppendLine("    )");
            sb.AppendLine("END");
            sb.AppendLine();

            sb.AppendLine("IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')");
            sb.AppendLine("BEGIN");
            sb.AppendLine("    CREATE TABLE Users (");
            sb.AppendLine("        Id INT PRIMARY KEY IDENTITY(1,1),");
            sb.AppendLine("        FirstName NVARCHAR(50) NOT NULL,");
            sb.AppendLine("        LastName NVARCHAR(50) NOT NULL,");
            sb.AppendLine("        Email NVARCHAR(100) NOT NULL,");
            sb.AppendLine("        PasswordHash NVARCHAR(MAX) NOT NULL,");
            sb.AppendLine("        PasswordSalt NVARCHAR(MAX) NOT NULL,");
            sb.AppendLine("        RoleId INT NOT NULL,");
            sb.AppendLine("        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),");
            sb.AppendLine("        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)");
            sb.AppendLine("    )");
            sb.AppendLine("END");
            sb.AppendLine();

            // Insert roles from the database
            if (roles.Any())
            {
                sb.AppendLine("-- Insert predefined roles");
                foreach (var role in roles)
                {
                    sb.AppendLine($"IF NOT EXISTS (SELECT * FROM Roles WHERE Name = '{role.Name}')");
                    sb.AppendLine("BEGIN");
                    sb.AppendLine($"    INSERT INTO Roles (Name, Description) VALUES ('{role.Name}', '{role.Description ?? ""}')");
                    sb.AppendLine("END");
                }
            }

            return sb.ToString();
        }
    }

}
