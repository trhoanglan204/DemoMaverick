using Maverick.Commander.Services;
using Maverick.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

#pragma warning disable IDE0290

namespace Maverick.Commander.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommandAndControl _commandAndControl;

        public HomeController(ILogger<HomeController> logger, ICommandAndControl commandAndControl)
        {
            _logger = logger;
            _commandAndControl = commandAndControl;
        }

        public IActionResult Index()
        {
            var listClient = _commandAndControl.GetAllClients();
            var listHistory = new List<HistoryModel>();
            return View(listClient);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
