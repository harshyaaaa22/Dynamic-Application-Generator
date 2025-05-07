using DynamicAppGenerator.Data;
using DynamicAppGenerator.Models;
using DynamicAppGenerator.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace DynamicAppGenerator.Services
{
    public class LayoutEditorService
    {
        private readonly AppDbContext _dbContext;
        private readonly string _layoutsBasePath;

        public LayoutEditorService(AppDbContext dbContext, IHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _layoutsBasePath = Path.Combine(hostEnvironment.ContentRootPath, "Layouts", "Templates");
        }

        public string GetJsContent(string layoutId)
        {
            string jsPath = Path.Combine(_layoutsBasePath, layoutId, $"{layoutId}.js");
            return File.Exists(jsPath) ? File.ReadAllText(jsPath) : string.Empty;
        }

        public string GetDefaultLayoutHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@ViewData[""Title""] - My Application</title>
    <link rel=""stylesheet"" href=""~/lib/bootstrap/dist/css/bootstrap.min.css"" />
    <link rel=""stylesheet"" href=""~/css/site.css"" />
    <link rel=""stylesheet"" href=""~/css/custom-layout.css"" />
    @RenderSection(""Styles"", required: false)
</head>
<body>
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container"">
                <a class=""navbar-brand"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">My Application</a>
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

    <footer class=""border-top footer text-muted"">
        <div class=""container"">
            &copy; @DateTime.Now.Year - My Application - <a asp-area="""" asp-controller=""Home"" asp-action=""Privacy"">Privacy</a>
        </div>
    </footer>
    <script src=""~/lib/jquery/dist/jquery.min.js""></script>
    <script src=""~/lib/bootstrap/dist/js/bootstrap.bundle.min.js""></script>
    <script src=""~/js/site.js"" asp-append-version=""true""></script>
    <script src=""~/js/custom-layout.js"" asp-append-version=""true""></script>
    @RenderSection(""Scripts"", required: false)
</body>
</html>";
        }

        public string GetDefaultLayoutCss()
        {
            return @"/* Custom Layout CSS */
body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.navbar {
    background-color: #f8f9fa;
}

.footer {
    position: absolute;
    bottom: 0;
    width: 100%;
    white-space: nowrap;
    line-height: 60px;
}";
        }

        public string GetDefaultLayoutJs()
        {
            return @"// Custom Layout JavaScript
$(document).ready(function() {
    console.log('Custom layout script loaded');
    
    // Add your custom JavaScript here
});";
        }

        public async Task<string> SaveEditedLayoutAsync(LayoutEditorViewModel model)
        {
           
                // Check if this is a new layout or an existing one
                var existingLayout = await _dbContext.CustomLayouts
                    .FirstOrDefaultAsync(l => l.Id == model.Id);

                if (existingLayout == null)
                {
                    // Create new layout
                    var newId = !string.IsNullOrEmpty(model.Id) ? model.Id : GenerateLayoutId(model.Name);

                    var newLayout = new CustomLayout
                    {
                        Id = newId,
                        Name = model.Name,
                        Description = model.Description,
                        HtmlContent = model.HtmlContent,
                        CssContent = model.CssContent,
                        JsContent = model.JsContent,
                        CreatedDate = DateTime.UtcNow
                    };

                    await _dbContext.CustomLayouts.AddAsync(newLayout);
                    await _dbContext.SaveChangesAsync();

                    return $"Layout '{model.Name}' created successfully.";
                }
                else
                {
                    // Update existing layout
                    existingLayout.Name = model.Name;
                    existingLayout.Description = model.Description;
                    existingLayout.HtmlContent = model.HtmlContent;
                    existingLayout.CssContent = model.CssContent;
                    existingLayout.JsContent = model.JsContent;
                    existingLayout.ModifiedDate = DateTime.UtcNow;

                    _dbContext.CustomLayouts.Update(existingLayout);
                    await _dbContext.SaveChangesAsync();

                    return $"Layout '{model.Name}' updated successfully.";
                }
            }
            
        

        private string GenerateLayoutId(string name)
        {
            // Generate a URL-friendly ID based on the layout name
            var id = name.ToLower()
                .Replace(" ", "-")
                .Replace("_", "-");

            // Remove any non-alphanumeric characters except dashes
            id = new string(id.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            return id;
        }
    }
}
