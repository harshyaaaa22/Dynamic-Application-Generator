using DynamicAppGenerator.Data;

namespace DynamicAppGenerator.Services
{
    public class LayoutsService
    {
        private readonly List<LayoutTemplate> _availableLayouts;
        private readonly string _layoutsBasePath;
        private readonly AppDbContext _dbContext;

        public LayoutsService(IHostEnvironment hostEnvironment, AppDbContext dbContext)
        {
            // Set the base path for layout files
            _layoutsBasePath = Path.Combine(hostEnvironment.ContentRootPath, "Layouts", "Templates");

            _dbContext = dbContext;

            // Initialize layouts collection
            _availableLayouts = InitializeLayouts();
        }

        private List<LayoutTemplate> InitializeLayouts()
        {
            // Start with the built-in layouts
            var layouts = new List<LayoutTemplate>
        {
            CreateLayoutTemplate(
                "default",
                "Default Layout",
                "The standard layout with a top navigation bar and responsive design.",
                "Default/Default.cshtml",
                "Default/Default.css"
            ),
            CreateLayoutTemplate(
                "modern",
                "Modern Dashboard",
                "A modern dashboard layout with sidebar navigation and dark theme.",
                "Modern/Modern.cshtml",
                "Modern/Modern.css"
            ),
            CreateLayoutTemplate(
                "minimal",
                "Minimal Clean",
                "A minimalist layout with clean design and light color scheme.",
                "Minimal/Minimal.cshtml",
                "Minimal/Minimal.css"
            ),
            CreateLayoutTemplate(
                "Material",
                "Material Cool",
                "A Material layout with clean design and light color scheme.",
                "Material/Material.cshtml",
                "Material/Material.css"
            ),
            CreateLayoutTemplate(
                "Dark",
                "Dark Theme",
                "A Material layout with clean design and light color scheme.",
                "Dark/Dark.cshtml",
                "Dark/Dark.css"
            )
        };

            // Add custom layouts from the database
            try
            {
                var customLayouts = _dbContext.CustomLayouts.ToList();
                foreach (var customLayout in customLayouts)
                {
                    layouts.Add(new LayoutTemplate
                    {
                        Id = customLayout.Id,
                        Name = customLayout.Name,
                        Description = customLayout.Description,
                        LayoutContent = customLayout.HtmlContent,
                        CssContent = customLayout.CssContent,
                        IsCustom = true
                    });
                }
            }
            catch
            {
                // If database is not available, continue with the built-in layouts
            }

            return layouts;
        }

        private LayoutTemplate CreateLayoutTemplate(string id, string name, string description, string layoutPath, string cssPath)
        {
            string fullLayoutPath = Path.Combine(_layoutsBasePath, layoutPath);
            string fullCssPath = Path.Combine(_layoutsBasePath, cssPath);

            return new LayoutTemplate
            {
                Id = id,
                Name = name,
                Description = description,
                LayoutFilePath = layoutPath,
                CssFilePath = cssPath,
                LayoutContent = File.Exists(fullLayoutPath) ? File.ReadAllText(fullLayoutPath) : string.Empty,
                CssContent = File.Exists(fullCssPath) ? File.ReadAllText(fullCssPath) : string.Empty,
                IsCustom = false
            };
        }

        public List<LayoutTemplate> GetAvailableLayouts()
        {
            // Refresh custom layouts from database
            RefreshCustomLayouts();
            return _availableLayouts;
        }

        public LayoutTemplate GetLayoutById(string id)
        {
            // Refresh custom layouts to ensure we have the latest
            RefreshCustomLayouts();
            return _availableLayouts.FirstOrDefault(l => l.Id == id) ??
                   _availableLayouts.First(); // Return default layout if not found
        }

        // Method to get layout content directly from file (for dynamic updates)
        public string GetLayoutContent(string layoutPath)
        {
            string fullPath = Path.Combine(_layoutsBasePath, layoutPath);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
        }

        // Method to get CSS content directly from file (for dynamic updates)
        public string GetCssContent(string cssPath)
        {
            string fullPath = Path.Combine(_layoutsBasePath, cssPath);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
        }

        // Refresh custom layouts from the database
        private void RefreshCustomLayouts()
        {
            try
            {
                // Remove existing custom layouts
                _availableLayouts.RemoveAll(l => l.IsCustom);

                // Add updated custom layouts from the database
                var customLayouts = _dbContext.CustomLayouts.ToList();
                foreach (var customLayout in customLayouts)
                {
                    _availableLayouts.Add(new LayoutTemplate
                    {
                        Id = customLayout.Id,
                        Name = customLayout.Name,
                        Description = customLayout.Description,
                        LayoutContent = customLayout.HtmlContent,
                        CssContent = customLayout.CssContent,
                        IsCustom = true
                    });
                }
            }
            catch
            {
                // If database is not available, continue with the existing layouts
            }
        }
    }
    public class LayoutTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LayoutContent { get; set; }
        public string CssContent { get; set; }

        // Added properties for file paths
        public string LayoutFilePath { get; set; }
        public string CssFilePath { get; set; }

        // Flag to identify custom layouts from the database
        public bool IsCustom { get; set; }
    }
}
