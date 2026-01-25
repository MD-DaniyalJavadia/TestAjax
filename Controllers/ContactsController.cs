using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestAjax.Models;
using TestAjax.ViewMdoel;

namespace TestAjax.Controllers
{
    public class ContactsController : Controller
    {
        private readonly UharContext _context;

        public ContactsController(UharContext context)
        {
            _context = context;
        }

        // Accounts main page (tabs)
        public IActionResult Index(string tab = "customers")
        {
            ViewData["ActiveTab"] = tab.ToLower() == "suppliers" ? "suppliers" : "customers";
            ViewData["Title"] = "Accounts";
            return View();
        }

        // AJAX for DataTable - Returns contacts with real balance (using view or calculation)
        [HttpGet]
        public async Task<IActionResult> GetContacts(string type)
        {
            if (string.IsNullOrWhiteSpace(type) || (type != "Customer" && type != "Supplier"))
            {
                return Json(new { data = Array.Empty<object>() });
            }

            try
            {
                var contacts = await _context.Contacts
                    .Where(c => c.IsActive && c.ContactType == type)
                    .Select(c => new
                    {
                        ContactId = c.Id,
                        name = c.Name,
                        phoneNumber = c.PhoneNumber ?? "-",
                        createdDateFormatted = c.CreatedAt.ToString("dd-MM-yyyy"), // yeh client-side safe nahi, isliye problem
                        dueDate = "-",
                        balance = _context.Transactions
                            .Where(t => t.ContactId == c.Id)
                            .Sum(t => t.Type == "Received" ? t.Amount : -t.Amount)
                    })
                    .ToListAsync();

                return Json(new { data = contacts });
            }
            catch (Exception ex)
            {
                // Debugging ke liye – production mein remove kar dena
                Console.WriteLine("GetContacts Error: " + ex.Message);
                return StatusCode(500, new { error = "Server error while loading contacts" });
            }
        }

        // Totals for "I will receive" and "I will give"
        [HttpGet]
        public async Task<IActionResult> GetTotals()
        {
            try
            {
                var balances = await _context.Contacts
                    .Where(c => c.IsActive)
                    .Select(c => _context.Transactions
                        .Where(t => t.ContactId == c.Id)
                        .Sum(t => t.Type == "Received" ? t.Amount : -t.Amount))
                    .ToListAsync();

                decimal receive = balances.Where(b => b > 0).Sum();
                decimal give = balances.Where(b => b < 0).Sum(b => Math.Abs(b));

                return Json(new
                {
                    totalReceive = receive,
                    totalGive = give,
                    formattedReceive = receive.ToString("N0"),
                    formattedGive = give.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTotals error: {ex.Message}");
                return Json(new
                {
                    totalReceive = 0,
                    totalGive = 0,
                    formattedReceive = "0",
                    formattedGive = "0"
                });
            }
        }

        // Create Contact - GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Add Contact";
            return View(new ContactViewModel());
        }

        // Create Contact - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contact = new Contact
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                ContactType = model.ContactType,
                Email = model.Email,
                Address = model.Address,
                Notes = model.Notes,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Ledger", new { id = contact.Id });
        }
    }
}