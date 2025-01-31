using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using DentalClinicSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicSystem.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly DentalClinicContext _context;

        public InvoicesController(DentalClinicContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Items)
                .ThenInclude(item => item.Treatment)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
            return View(invoices);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Items)
                .ThenInclude(item => item.Treatment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.Patient = patient;
            ViewBag.Treatments = await _context.Treatments.ToListAsync();

            var viewModel = new InvoiceViewModel
            {
                PatientId = patientId,
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel() // Add one empty item by default
                }
            };

            return View(viewModel);
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var invoice = new Invoice
                {
                    PatientId = model.PatientId,
                    InvoiceDate = DateTime.Now,
                    TotalAmount = model.Items.Sum(i => i.Price * i.Quantity),
                    PaidAmount = model.PaidAmount,
                    PaymentStatus = model.PaidAmount >= model.Items.Sum(i => i.Price * i.Quantity)
                        ? PaymentStatus.Paid
                        : model.PaidAmount > 0 ? PaymentStatus.PartiallyPaid : PaymentStatus.Pending,
                    Items = model.Items.Select(i => new InvoiceItem
                    {
                        TreatmentId = i.TreatmentId,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList()
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = invoice.Id });
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Patient = await _context.Patients.FindAsync(model.PatientId);
            ViewBag.Treatments = await _context.Treatments.ToListAsync();
            return View(model);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Items)
                .ThenInclude(item => item.Treatment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new InvoiceViewModel
            {
                InvoiceId = invoice.Id,
                PatientId = invoice.PatientId,
                PaidAmount = invoice.PaidAmount,
                Items = invoice.Items.Select(i => new InvoiceItemViewModel
                {
                    TreatmentId = i.TreatmentId,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            ViewBag.Patient = invoice.Patient;
            ViewBag.Treatments = await _context.Treatments.ToListAsync();

            return View(viewModel);
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceViewModel model)
        {
            if (id != model.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var invoice = await _context.Invoices
                        .Include(i => i.Items)
                        .FirstOrDefaultAsync(i => i.Id == id);

                    if (invoice == null)
                    {
                        return NotFound();
                    }

                    // Update invoice properties
                    invoice.TotalAmount = model.Items.Sum(i => i.Price * i.Quantity);
                    invoice.PaidAmount = model.PaidAmount;
                    invoice.PaymentStatus = model.PaidAmount >= invoice.TotalAmount
                        ? PaymentStatus.Paid
                        : model.PaidAmount > 0 ? PaymentStatus.PartiallyPaid : PaymentStatus.Pending;

                    // Remove existing items
                    _context.InvoiceItems.RemoveRange(invoice.Items);

                    // Add new items
                    invoice.Items = model.Items.Select(i => new InvoiceItem
                    {
                        TreatmentId = i.TreatmentId,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList();

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = invoice.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(model.InvoiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Patient = await _context.Patients.FindAsync(model.PatientId);
            ViewBag.Treatments = await _context.Treatments.ToListAsync();
            return View(model);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                _context.InvoiceItems.RemoveRange(invoice.Items);
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Invoices/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Items)
                .ThenInclude(item => item.Treatment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }

        // GET: Invoices/UpdatePayment/5
        public async Task<IActionResult> UpdatePayment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new UpdatePaymentViewModel
            {
                InvoiceId = invoice.Id,
                CurrentPaidAmount = invoice.PaidAmount,
                TotalAmount = invoice.TotalAmount,
                RemainingAmount = invoice.RemainingAmount
            };

            return View(viewModel);
        }

        // POST: Invoices/UpdatePayment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayment(int id, UpdatePaymentViewModel model)
        {
            if (id != model.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.PaidAmount = model.CurrentPaidAmount + model.AdditionalPayment;
                invoice.PaymentStatus = invoice.PaidAmount >= invoice.TotalAmount
                    ? PaymentStatus.Paid
                    : PaymentStatus.PartiallyPaid;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = invoice.Id });
            }

            return View(model);
        }
    }
}
