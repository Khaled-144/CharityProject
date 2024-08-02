using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityProject.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ViewDevice()
        {
            var devices = await _context.Devices.ToListAsync();
            return View(devices);
        }

        public IActionResult ManageSalaries()
        {
            var salarySummary = _context.SalaryHistories
                .GroupBy(s => s.emp_id)
                .Select(g => new
                {
                    EmpId = g.Key,
                    BaseSalary = g.Sum(s => s.base_salary),
                    TotalAllowances = g.Sum(s => (s.housing_allowances ?? 0) + (s.transportaion_allowances ?? 0) + (s.other_allowances ?? 0)),
                    TotalDiscounts = g.Sum(s => (s.delay_discount ?? 0) + (s.absence_discount ?? 0) + (s.other_discount ?? 0) + (s.debt ?? 0)),
                    TotalSalary = g.Sum(s => s.base_salary + (s.housing_allowances ?? 0) + (s.transportaion_allowances ?? 0) + (s.other_allowances ?? 0) + (s.overtime ?? 0) + (s.bonus ?? 0) - (s.delay_discount ?? 0) - (s.absence_discount ?? 0) - (s.other_discount ?? 0) - (s.debt ?? 0) + (s.shared_portion ?? 0) + (s.facility_portion ?? 0)),
                    Details = g.ToList()
                }).ToList();

            return View(salarySummary);
        }


        // Update Salary Action
        [HttpPost]
        public async Task<IActionResult> UpdateSalary(salaries_history salaryHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Update(salaryHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSalaries), new { month = salaryHistory.date.ToString("MMMM") });
            }
            return View(salaryHistory);
        }
        public async Task<IActionResult> EditDevice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDevice(int id, [Bind("devices_id,name,quantity")] Devices device)
        {
            if (id != device.devices_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.devices_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewDevice)); // Assuming you have an Index action
            }
            return View(device);
        }
        public async Task<IActionResult> DeleteDevice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(m => m.devices_id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }
        [HttpPost, ActionName("DeleteDevice")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewDevice)); 
                                                        
        }
        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.devices_id == id);
        }
    }
}
