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
        public IActionResult TotalContacts()
        {
            int totalParties = _context.Contacts
                .Count();

            return Json(new { TotalParties = totalParties });
        }

        public IActionResult TotalActiveAccount()
        {
            int totalActiveAccounts = _context.Contacts
                .Where(c => _context.Transactions.Any(t => t.ContactId == c.Id))
                .Count();

            return Json(new { TotalActiveAccounts = totalActiveAccounts });
        }


        // GET: /Dashboard/GetSummary
        [HttpGet]
        public async Task<IActionResult> TotalCustomers()
        {
            var summary = await _context.VwDashboardSummaries.FirstOrDefaultAsync();

            int totalCustomers = summary?.TotalCustomers ?? 0;

            return Json(new { TotalCustomers = totalCustomers });
        }
        [HttpGet]
        public async Task<IActionResult> GetMonthlyTotals()
        {
            var data = await _context.TransactionSummaries
                .ToListAsync();

            return Json(data);
        }




        [HttpGet]
        public async Task<IActionResult> TotalSuppliers()
        {
        var summary = await _context.VwDashboardSummaries.FirstOrDefaultAsync();

        int totalSuppliers = summary?.TotalSuppliers ?? 0;

        return Json(new { TotalSuppliers = totalSuppliers });
    }


        public IActionResult TotalGiven()
        {
            decimal totalGiven = _context.VwTotalReceivables
                .Sum(x => x.TotalReceivable ?? 0);

            return Json(new { TotalGiven = totalGiven });
        }



        public IActionResult GetNewCustomers()
        {
            var customers = _context.Contacts
                .OrderByDescending(x => x.Id)   // Latest contacts
                .Take(10)                      // last 10 
                .Select(x => new
                {
                    x.Name,
                    x.ContactType
                })
                .ToList();

            return Json(customers);
        }
        public IActionResult TransactionHistory2()
        {
            var transactions = _context.Transactions
                .OrderByDescending(x => x.TransactionDate)
                .Take(10)
                .Select(x => new
                {
                    x.TransactionDate,
                    x.Amount,
                })
                .ToList();

            return Json(transactions);
        }
        [HttpGet]
        public IActionResult GetTransactions()
        {
            var last10 = _context.TransactionSummaries
                                 .OrderByDescending(t => t.Year)         // newest year first
                                 .ThenByDescending(t => t.MonthNumber)  // within year, newest month first
                                 .Take(10)
                                 .ToList();

            return Json(last10);
        }

        public IActionResult TotalReceived()
        {
            decimal totalReceived = _context.VwTotalPayables
                .Sum(x => x.TotalPayable ?? 0);

            return Json(new { TotalReceived = totalReceived });
        }


        public IActionResult Balance()
        {
            decimal netBalance = _context.VwOverallBalances
                .Sum(x => x.NetBalance ?? 0);

            return Json(new { NetBalance = netBalance });
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
