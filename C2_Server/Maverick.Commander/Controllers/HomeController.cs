using Maverick.Commander.Services;
using Maverick.Models;
using Maverick.Models.History;
using Maverick.Models.ViewModels;
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

        public IActionResult Index(string? clientId)
        {
            var clientChatVM = new ClientChatVM
            {
                Clients = _commandAndControl.GetAllClients(),
                SelectedClientId = clientId
            };
            if (!string.IsNullOrEmpty(clientId))
            {
                clientChatVM.Histories = _commandAndControl.GetAllHistories(int.Parse(clientId));
            }
            return View(clientChatVM);
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
