using DynamicAppGenerator.Models;
using DynamicAppGenerator.Services;
using DynamicAppGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppGenerator.Controllers
{
    public class LayoutEditorController : Controller
    {
        private readonly LayoutsService _layoutsService;
        private readonly LayoutEditorService _layoutEditorService;

        public LayoutEditorController(LayoutsService layoutsService, LayoutEditorService layoutEditorService)
        {
            _layoutsService = layoutsService;
            _layoutEditorService = layoutEditorService;
        }

        public IActionResult Index()
        {
            var layouts = _layoutsService.GetAvailableLayouts();
            return View(layouts);
        }

        public IActionResult Edit(string id)
        {
            var layout = _layoutsService.GetLayoutById(id);
            if (layout == null)
            {
                return NotFound();
            }

            var model = new LayoutEditorViewModel
            {
                Id = layout.Id,
                Name = layout.Name,
                Description = layout.Description,
                HtmlContent = layout.LayoutContent,
                CssContent = layout.CssContent,
                JsContent = _layoutEditorService.GetJsContent(layout.Id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LayoutEditorViewModel model)
        {
           
                var result = await _layoutEditorService.SaveEditedLayoutAsync(model);
                TempData["Message"] = result;
                return RedirectToAction(nameof(Index));
            
            return View(model);
        }

        public IActionResult Create()
        {
            var model = new LayoutEditorViewModel
            {
                Id = string.Empty,
                Name = "New Layout",
                Description = "Custom layout description",
                HtmlContent = _layoutEditorService.GetDefaultLayoutHtml(),
                CssContent = _layoutEditorService.GetDefaultLayoutCss(),
                JsContent = _layoutEditorService.GetDefaultLayoutJs()
            };

            return View("Edit", model);
        }

        public IActionResult Preview(string id)
        {
            var layout = _layoutsService.GetLayoutById(id);
            if (layout == null)
            {
                return NotFound();
            }

            return View(layout);
        }
    }
}
