using DynamicAppGenerator.Models;
using DynamicAppGenerator.Repositories;

namespace DynamicAppGenerator.Services
{
    public class GeneratorService
    {
        private readonly RoleRepository _roleRepository;
        private readonly TemplateService _templateService;
        private readonly LayoutsService _layoutsService;

        public GeneratorService(RoleRepository roleRepository, TemplateService templateService, LayoutsService layoutsService)
        {
            _roleRepository = roleRepository;
            _templateService = templateService;
            _layoutsService = layoutsService;
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

                var roles = await _roleRepository.GetAllRolesAsync();

                // Generate database script
                var dbScript = _templateService.GenerateDatabaseScript(roles);
                await File.WriteAllTextAsync(Path.Combine(appPath, "Scripts", "DatabaseSetup.sql"), dbScript);

                // Generate authentication models
                await GenerateAuthenticationModelsAsync(appPath);

                // Generate controllers
                await GenerateControllersAsync(appPath);

                // Get the selected layout
                var selectedLayout = _layoutsService.GetLayoutById(config.SelectedLayout);

                // Generate views with the selected layout
                await GenerateViewsAsync(appPath, roles, selectedLayout);

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
            var templateService = new AuthModelTemplateService();
            await templateService.GenerateAuthenticationModelsAsync(appPath);
        }

        private async Task GenerateControllersAsync(string appPath)
        {
            var templateService = new ControllerTemplateService();
            await templateService.GenerateControllersAsync(appPath);
        }


        private async Task GenerateViewsAsync(string appPath, IEnumerable<Role> roles, LayoutTemplate layout)
        {
            var templateService = new ViewTemplateService();

            // Set the selected layout
            templateService.SetLayout(layout);

            // Load templates from files if needed
            try
            {
                await templateService.LoadTemplateFromFileAsync("HomeIndex", "Templates/Views/Home/Index.cshtml");
                await templateService.LoadTemplateFromFileAsync("HomePrivacy", "Views/Home/Privacy.cshtml");
                await templateService.LoadTemplateFromFileAsync("site.css", "wwwroot/css/site.css");
                await templateService.LoadTemplateFromFileAsync("auth-styles.css", "wwwroot/css/auth-styles.css");
                await templateService.LoadTemplateFromFileAsync("site.js", "wwwroot/js/site.js");
            }
            catch (FileNotFoundException)
            {
                // Use default templates if files not found
            }

            // Generate all views
            await templateService.GenerateViewsAsync(appPath, roles);
        }

        private async Task GenerateServicesAsync(string appPath)
        {
            var serviceTemplateService = new ServiceTemplateService();
            await serviceTemplateService.GenerateServicesAsync(appPath);
        }

        private async Task GenerateProjectFilesAsync(string appPath)
        {
            var projectTemplateService = new ProjectTemplateService();
            await projectTemplateService.GenerateProjectFilesAsync(appPath);
        }

    }
}
