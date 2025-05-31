using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortenerApp.Models;
using URLShortenerApp.Settings;

namespace URLShortenerApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _aboutFile;

        public HomeController(IConfiguration configuration)
        {
            _aboutFile = configuration["AboutDescriptionFilePath"];
        }

        public IActionResult Index()
        {
            return View();
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

        [AllowAnonymous]
        public IActionResult About()
        {
            var content = System.IO.File.Exists(_aboutFile)
                ? System.IO.File.ReadAllText(_aboutFile)
                : "No about info yet.";
            return View(model: content);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public IActionResult UpdateAbout(string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_aboutFile)!);
            System.IO.File.WriteAllText(_aboutFile, content ?? "");
            return RedirectToAction("About");
        }
    }
}
