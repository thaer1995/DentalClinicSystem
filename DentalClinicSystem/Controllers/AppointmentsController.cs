using DentalClinicSystem.Data;
using DentalClinicSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly DentalClinicContext _context;

        public AppointmentsController(DentalClinicContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();
            return View(appointments);
        }

        public IActionResult Create()
        {
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "Name");
            ViewBag.Doctors = new SelectList(_context.Doctors, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DoctorId,AppointmentDate,Treatment,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Status = AppointmentStatus.Scheduled;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appointment);
        }
    }
}
