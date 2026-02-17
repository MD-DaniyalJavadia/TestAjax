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
                var contacts = await _context.VwActiveContactsWithBalances
                  .Where(c => c.ContactType == type) 
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
        public async Task<IActionResult> GetTotals(string type)
        {
            try
            {
                var balances = await _context.Contacts
                    .Where(c => c.IsActive && c.ContactType == type)
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

        // Edit Contact - GET

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            var contact = _context.Contacts.FirstOrDefault(x => x.Id == id);

            if (contact == null)
                return NotFound();

            var model = new Contact
            {
                Id = contact.Id,
                Name = contact.Name,
                PhoneNumber = contact.PhoneNumber,
                ContactType = contact.ContactType,
                Email = contact.Email,
            };

            ViewData["Title"] = "Edit Contact";
            return View(model);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Delete(long id)
        {
            try
            {
                var contact = _context.Contacts.Find(id);
                if (contact == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Contact not found"
                    });
                }

                // Pehle contact ke saare transactions delete karein
                var transactions = _context.Transactions.Where(t => t.ContactId == id).ToList();

                if (transactions.Any())
                {
                    _context.Transactions.RemoveRange(transactions);
                }

                // Ab contact delete karein
                _context.Contacts.Remove(contact);
                _context.SaveChanges();

                var message = transactions.Any()
                    ? $"Contact and {transactions.Count} transaction(s) deleted successfully"
                    : "Contact deleted successfully";

                return Json(new
                {
                    success = true,
                    message = message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting the contact"
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> HasTransactions(long id)
        {
            var hasTransactions = await _context.Transactions
                .AnyAsync(t => t.ContactId == id);

            return Json(hasTransactions);
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
        // Edit Contact - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Contact model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contact = await _context.Contacts
                                        .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (contact == null)
                return NotFound();

            // Update fields
            contact.Name = model.Name;
            contact.PhoneNumber = model.PhoneNumber;
            contact.ContactType = model.ContactType;
            contact.Email = model.Email;
            contact.Address = model.Address;
            contact.Notes = model.Notes;

            // Optional audit fields
            contact.UpdatedAt = DateTime.Now;
            contact.UpdatedBy = User.Identity?.Name ?? "System";

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Ledger", new { id = contact.Id });
        }

    }
}