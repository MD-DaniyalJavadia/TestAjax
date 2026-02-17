using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestAjax.Models;

namespace TestAjax.Controllers
{
    public class LedgerController : Controller
    {
        private readonly UharContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LedgerController(UharContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // ================================
        // GET: Ledger Page
        // ================================
        [HttpGet]
        public async Task<IActionResult> Index(long id)
        {
            if (id <= 0)
                return BadRequest("Invalid Contact ID");

            var contact = await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (contact == null)
                return NotFound();

            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.ContactId == id)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.Id)
                .ToListAsync();

            var balance = await GetBalanceAsync(id);

            ViewData["Balance"] = balance;
            ViewData["Transactions"] = transactions;
            ViewData["WhatsAppNumber"] = contact.PhoneNumber;

            return View(contact);
        }

        // ================================
        // GET: Add Transaction Page
        // ================================
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
            ViewBag.CurrentBalance = await GetBalanceAsync(id);

            return View("TransactionEntry", contact);
        }

        // ================================
        // POST: Add Transaction
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(
            long contactId,
            decimal amount,
            string type,
            string? details = null,
            DateTime? transactionDate = null,
            IFormFile? photo = null)
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
                return RedirectToAction("Index", "Contacts");
            }

            string? photoFileName = null;

            // ================================
            // Image Upload
            // ================================
            if (photo != null && photo.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(photo.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] = "Invalid image format.";
                    return RedirectToAction("Add", new { id = contactId, type });
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "receipts");
                Directory.CreateDirectory(uploadsFolder);

                photoFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, photoFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await photo.CopyToAsync(stream);
            }

            var transaction = new Transaction
            {
                ContactId = contactId,
                Amount = amount,
                Type = type,
                Details = details?.Trim(),
                TransactionDate = transactionDate?.Date ?? DateTime.Today,
                PhotoFileName = photoFileName
            };

            _context.Transactions.Add(transaction);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction added successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Error while saving transaction.";
                return RedirectToAction("Add", new { id = contactId, type });
            }

            return RedirectToAction("Index", new { id = contactId });
        }

        // ================================
        // GET: Edit Transaction Partial (AJAX)
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetTransactionEditForm(long id)
        {
            var transaction = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return Content("<div class='alert alert-warning'>Transaction not found.</div>", "text/html");
            }
            ViewBag.CurrentBalance = await GetBalanceAsync(transaction.ContactId);


            return PartialView("_TransactionEntryEdit", transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransaction(TestAjax.Models.Transaction model, IFormFile? photo)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_TransactionEntryEdit", model);
            }

            try
            {
                var transaction = await _context.Transactions
                    .FirstOrDefaultAsync(t => t.Id == model.Id);

                if (transaction == null)
                    return NotFound("Transaction not found.");

                // Update fields
                transaction.Amount = model.Amount;
                transaction.Type = model.Type;
                transaction.Details = model.Details?.Trim();
                transaction.TransactionDate = model.TransactionDate;

                // Handle photo upload
                if (photo != null && photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "receipts");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await photo.CopyToAsync(stream);

                    // Delete old photo
                    if (!string.IsNullOrEmpty(transaction.PhotoFileName))
                    {
                        var oldPath = Path.Combine(uploadsFolder, transaction.PhotoFileName);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    transaction.PhotoFileName = fileName;
                }

                _context.Update(transaction);
                await _context.SaveChangesAsync();

                // Redirect to Ledger Index page for this contact
                return RedirectToAction("Index", "Ledger", new { id = transaction.ContactId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Ledger", new { id = model.ContactId });
            }
        }

        // ================================
        // GET: Transaction Canvas Content (AJAX)
        // ================================
        [HttpGet]
        public async Task<IActionResult> GetTransactionCanvasContent(long id)
        {
            var transaction = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return Content("<div class='alert alert-warning'>Transaction not found.</div>", "text/html");

            return PartialView("_TransactionCanvasPartial", transaction);
        }

        // ================================
        // PRIVATE: Balance Calculation
        // ================================
        private async Task<decimal> GetBalanceAsync(long contactId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.ContactId == contactId)
                .Select(t => t.Type == "Received" ? t.Amount : -t.Amount)
                .ToListAsync();

            return transactions.Sum();
        }
    }
}
