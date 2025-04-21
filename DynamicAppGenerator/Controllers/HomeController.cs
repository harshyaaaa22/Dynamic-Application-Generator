using System.Diagnostics;
using DynamicAppGenerator.Models;
using DynamicAppGenerator.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleRepository _roleRepository;

        public HomeController(ILogger<HomeController> logger, RoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> Index()
        {
            await _roleRepository.EnsureRolesTableExistsAsync();
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
}
