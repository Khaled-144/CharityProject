using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;

namespace CharityProject.Controllers
{
    public class salaries_historyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public salaries_historyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: salaries_history
        public ActionResult Index()
        {
            var salaries_history = _context.SalaryHistories
                .GroupBy(sr => new { sr.date.Year, sr.date.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Total = g.Sum(r => r.base_salary) +
                            g.Sum(r => r.housing_allowances ?? 0) +
                            g.Sum(r => r.transportaion_allowances ?? 0) +
                            g.Sum(r => r.other_allowances ?? 0) +
                            g.Sum(r => r.overtime ?? 0) +
                            g.Sum(r => r.bonus ?? 0) -
                            g.Sum(r => r.delay_discount ?? 0) -
                            g.Sum(r => r.absence_discount ?? 0) -
                            g.Sum(r => r.other_discount ?? 0) -
                            g.Sum(r => r.debt ?? 0) -
                            g.Sum(r => r.shared_portion ?? 0) 
                })
                .AsEnumerable()
                .Select(g => new {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                    Count = g.Count,
                    Total = g.Total
                })
                .ToList<dynamic>();  // Cast to List<dynamic>

            return View(salaries_history);
        }

        [HttpGet]
        public ActionResult SearchByYear(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return RedirectToAction("Index");
            }

            int parsedYear;
            if (!int.TryParse(year, out parsedYear))
            {
                return RedirectToAction("Index");
            }

            var filteredSalaries = _context.SalaryHistories
                .Where(sr => sr.date.Year == parsedYear)
                .GroupBy(sr => new { sr.date.Year, sr.date.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Total = g.Sum(r => r.base_salary) +
                            g.Sum(r => r.housing_allowances ?? 0) +
                            g.Sum(r => r.transportaion_allowances ?? 0) +
                            g.Sum(r => r.other_allowances ?? 0) +
                            g.Sum(r => r.overtime ?? 0) +
                            g.Sum(r => r.bonus ?? 0) -
                            g.Sum(r => r.delay_discount ?? 0) -
                            g.Sum(r => r.absence_discount ?? 0) -
                            g.Sum(r => r.other_discount ?? 0) -
                            g.Sum(r => r.debt ?? 0) -
                            g.Sum(r => r.shared_portion ?? 0)
                })
                .AsEnumerable()
                .Select(g => new {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                    Count = g.Count,
                    Total = g.Total
                })
                .ToList<dynamic>();

            return View("Index", filteredSalaries);
        }

        private string GetArabicMonth(string monthYear)
        {
            var parts = monthYear.Split(' ');
            if (parts.Length == 2)
            {
                string englishMonth = parts[0];
                string year = parts[1];

                string arabicMonth = englishMonth switch
                {
                    "January" => "يناير",
                    "February" => "فبراير",
                    "March" => "مارس",
                    "April" => "أبريل",
                    "May" => "مايو",
                    "June" => "يونيو",
                    "July" => "يوليو",
                    "August" => "أغسطس",
                    "September" => "سبتمبر",
                    "October" => "أكتوبر",
                    "November" => "نوفمبر",
                    "December" => "ديسمبر",
                    _ => englishMonth
                };

                return $"{arabicMonth} {year}";
            }

            return monthYear;
        }

        public IActionResult MonthDetails(string month)
        {
            var arabicMonth = GetArabicMonth(month);

            ViewBag.Month = arabicMonth;

            var salaries_history = _context.SalaryHistories
                .Include(sr => sr.employee)
                .AsEnumerable()  // Switch to client-side evaluation
                .Where(sr => sr.date.ToString("MMMM yyyy") == month)
                .ToList();

            // Calculate the total for the month
            var total = salaries_history
                .Sum(r => r.base_salary) +
                salaries_history.Sum(r => r.housing_allowances ?? 0) +
                salaries_history.Sum(r => r.transportaion_allowances ?? 0) +
                salaries_history.Sum(r => r.other_allowances ?? 0) +
                salaries_history.Sum(r => r.overtime ?? 0) +
                salaries_history.Sum(r => r.bonus ?? 0) -
                salaries_history.Sum(r => r.delay_discount ?? 0) -
                salaries_history.Sum(r => r.absence_discount ?? 0) -
                salaries_history.Sum(r => r.other_discount ?? 0) -
                salaries_history.Sum(r => r.debt ?? 0) -
                salaries_history.Sum(r => r.shared_portion ?? 0);

            ViewBag.Total = total;

            return View(salaries_history);
        }

        // GET: salaries_history/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaries_history = await _context.SalaryHistories
                .FirstOrDefaultAsync(m => m.salaries_history_id == id);
            if (salaries_history == null)
            {
                return NotFound();
            }

            return View(salaries_history);
        }

        // GET: salaries_history/Create
        public IActionResult CreateRecord()
        {
            ViewBag.Employees = new SelectList(_context.employee, "employee_id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int emp_id,
            DateTime date,
            double base_salary,
            double? housing_allowances,
            double? transportaion_allowances,
            double? other_allowances,
            double? overtime,
            double? bonus,
            double? delay_discount,
            double? absence_discount,
            double? other_discount,
            double? debt,
            double? shared_portion,
            double? facility_portion,
            double? Social_insurance, // إضافة Social_insurance هنا
            int work_days,
            string exchange_statement,
            string? notes)
        {
            if (ModelState.IsValid)
            {
                var salaryRecord = new salaries_history
                {
                    emp_id = emp_id,
                    date = date,
                    base_salary = base_salary,
                    housing_allowances = housing_allowances,
                    transportaion_allowances = transportaion_allowances,
                    other_allowances = other_allowances,
                    overtime = overtime,
                    bonus = bonus,
                    delay_discount = delay_discount,
                    absence_discount = absence_discount,
                    other_discount = other_discount,
                    debt = debt,
                    shared_portion = shared_portion,
                    facility_portion = facility_portion,
                   /* Social_insurance = Social_insurance,*/ // إضافة Social_insurance هنا
                    work_days = work_days,
                    exchange_statement = exchange_statement,
                    notes = notes
                };

                _context.SalaryHistories.Add(salaryRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Adjust the redirect as needed
            }

            ViewBag.Employees = new SelectList(_context.employee, "employee_id", "name", emp_id);
            return View();
        }

        // GET: salaries_history/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryRecord = await _context.SalaryHistories.FindAsync(id);
            if (salaryRecord == null)
            {
                return NotFound();
            }

            // Retrieve employee name based on emp_id
            var employeeName = await _context.employee
                .Where(e => e.employee_id == salaryRecord.emp_id)
                .Select(e => e.name)
                .FirstOrDefaultAsync();

            ViewBag.EmployeeName = employeeName; // Pass the name to the view

            return View(salaryRecord);
        }

        // POST: salaries_history/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            DateTime date,
            double base_salary,
            double? housing_allowances,
            double? transportaion_allowances,
            double? other_allowances,
            double? overtime,
            double? bonus,
            double? delay_discount,
            double? absence_discount,
            double? other_discount,
            double? debt,
            double? shared_portion,
            double? facility_portion,
            double? Social_insurance, // إضافة Social_insurance هنا
            int work_days,
            string exchange_statement,
            string? notes,
            int id) // ID is included but not modified
        {
            var salaryRecord = await _context.SalaryHistories.FindAsync(id);
            if (salaryRecord == null)
            {
                return NotFound();
            }

            // Update the properties without modifying the ID
            salaryRecord.date = date;
            salaryRecord.base_salary = base_salary;
            salaryRecord.housing_allowances = housing_allowances;
            salaryRecord.transportaion_allowances = transportaion_allowances;
            salaryRecord.other_allowances = other_allowances;
            salaryRecord.overtime = overtime;
            salaryRecord.bonus = bonus;
            salaryRecord.delay_discount = delay_discount;
            salaryRecord.absence_discount = absence_discount;
            salaryRecord.other_discount = other_discount;
            salaryRecord.debt = debt;
            salaryRecord.shared_portion = shared_portion;
            salaryRecord.facility_portion = facility_portion;
          /*  salaryRecord.Social_insurance = Social_insurance;*/
            salaryRecord.work_days = work_days;
            salaryRecord.exchange_statement = exchange_statement;
            salaryRecord.notes = notes;

            try
            {
                _context.Update(salaryRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SalaryHistories.Any(e => e.salaries_history_id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: salaries_history/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryRecord = await _context.SalaryHistories
                .FirstOrDefaultAsync(m => m.salaries_history_id == id);
            if (salaryRecord == null)
            {
                return NotFound();
            }

            return View(salaryRecord);
        }

        // POST: salaries_history/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryRecord = await _context.SalaryHistories.FindAsync(id);
            _context.SalaryHistories.Remove(salaryRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
