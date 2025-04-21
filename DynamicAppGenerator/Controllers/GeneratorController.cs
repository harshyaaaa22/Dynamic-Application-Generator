using DynamicAppGenerator.Models;
using DynamicAppGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppGenerator.Controllers
{
    public class GeneratorController : Controller
    {
        private readonly GeneratorService _generatorService;

        public GeneratorController(GeneratorService generatorService)
        {
            _generatorService = generatorService;
        }

        public IActionResult Index()
        {
            var config = new GeneratorConfig
            {
                ApplicationName = "DynamicAuthApp"
            };

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

            return View("Index", config);
        }
    }
}
