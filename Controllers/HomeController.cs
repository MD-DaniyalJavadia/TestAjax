using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TestAjax.Models;

namespace TestAjax.Controllers
{
    public class HomeController : Controller
    {
         private readonly UharContext _context;
         private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UharContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

            public IActionResult Index()
            {
                ViewData["Title"] = "Dashboard";
                return View();
            }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult MonthlyTransactionSummary()
        {
            var data = _context.MonthlyTransactionSummaries.ToList();
            return Json(data);
        }

        public IActionResult TransactionSummary()
        {
            var data = _context.TransactionSummaries.ToList();
            return Json(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
