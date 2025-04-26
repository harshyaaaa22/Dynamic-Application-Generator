using DynamicAppGenerator.Models;
using DynamicAppGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DynamicAppGenerator.Controllers
{
    public class GeneratorController : Controller
    {
        private readonly GeneratorService _generatorService;
        private readonly LayoutsService _layoutsService;

        public GeneratorController(GeneratorService generatorService, LayoutsService layoutsService)
        {
            _generatorService = generatorService;
            _layoutsService = layoutsService;
        }

        public IActionResult Index()
        {
            var config = new GeneratorConfig
            {
                ApplicationName = "DynamicAuthApp",
                SelectedLayout = "default" // Default selection
            };

            // Pass the layouts data to the view
            ViewBag.AvailableLayouts = _layoutsService.GetAvailableLayouts();

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GeneratorConfig config)
        {
            if (ModelState.IsValid)
            {
                var result = await _generatorService.GenerateApplicationAsync(config);
                TempData["Message"] = result;
                return RedirectToAction(nameof(Index));
            }

            // If there's a validation error, we need to pass the layouts again
            ViewBag.AvailableLayouts = _layoutsService.GetAvailableLayouts();
            return View("Index", config);
        }
    }
}