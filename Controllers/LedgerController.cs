using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAjax.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestAjax.Controllers
{
    public class LedgerController : Controller
    {
        private readonly UharContext _context;

        public LedgerController(UharContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Ledger page for a contact
        [HttpGet]
        public async Task<IActionResult> Index(long id)
        {

            if (id <= 0)
            {
                return BadRequest("Invalid Contact ID");
            }

            var contact = await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (contact == null)
                return NotFound();

            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.ContactId == id)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var balance = await _context.Transactions
                .Where(t => t.ContactId == id)
                .SumAsync(t => t.Type == "Received" ? t.Amount : -t.Amount);

            ViewData["Balance"] = balance;
            ViewData["Transactions"] = transactions;
            ViewData["WhatsAppNumber"] = contact.PhoneNumber;


            return View(contact);
        }

        // GET: Add transaction form
        [HttpGet]
        public async Task<IActionResult> Add(long id, string type = "Received")
        {
            var contact = await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (contact == null)
                return NotFound();

            type = type?.Trim() == "Given" ? "Given" : "Received";

            ViewBag.TransactionType = type;
            ViewBag.CurrentBalance = await _context.Transactions
                .Where(t => t.ContactId == id)
                .SumAsync(t => t.Type == "Received" ? t.Amount : -t.Amount);

            return View("TransactionEntry", contact);
        }

        // POST: Add transaction (AJAX support)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(
     long contactId,
     decimal amount,
     string type,
     string? details = null,
     DateTime? transactionDate = null)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Amount must be greater than zero.";
                return RedirectToAction("Add", new { id = contactId, type });
            }

            type = type?.Trim();
            if (type != "Received" && type != "Given")
            {
                TempData["ErrorMessage"] = "Invalid transaction type.";
                return RedirectToAction("Add", new { id = contactId, type });
            }

            var contactExists = await _context.Contacts
                .AnyAsync(c => c.Id == contactId && c.IsActive);

            if (!contactExists)
            {
                TempData["ErrorMessage"] = "Contact not found or inactive.";
                return RedirectToAction("Index", "Home");
            }

            var transaction = new Transaction
            {
                ContactId = contactId,
                Amount = amount,
                Type = type,
                Details = details?.Trim(),
                TransactionDate = transactionDate?.Date ?? DateTime.Today
            };

            _context.Transactions.Add(transaction);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction added successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Error while saving transaction. Please try again.";
            }

            // Seedha ledger page pe redirect (wahi screenshot wala page)
            return RedirectToAction("Index", new { id = contactId });
        }
    }
}