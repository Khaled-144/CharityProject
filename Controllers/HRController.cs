using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Diagnostics.Metrics;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CharityProject.Controllers
{
    [PermissionFilter("مدير الموارد البشرية والمالية")]


    public class HRController : Controller



    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult ExportToExcel(DateTime startDate, DateTime endDate)
        {
            var salaries = _context.SalaryHistories
           .Include(s => s.employee) // Include employee data
           .Where(s => s.date >= startDate && s.date <= endDate)
           .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Salaries");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "Employee Name"; // New header for employee name
                worksheet.Cells[1, 3].Value = "Base Salary";
                worksheet.Cells[1, 4].Value = "Housing Allowances";
                worksheet.Cells[1, 5].Value = "Transportation Allowances";
                worksheet.Cells[1, 6].Value = "Other Allowances";
                worksheet.Cells[1, 7].Value = "Overtime";
                worksheet.Cells[1, 8].Value = "Bonus";
                worksheet.Cells[1, 9].Value = "Delay Discount";
                worksheet.Cells[1, 10].Value = "Absence Discount";
                worksheet.Cells[1, 11].Value = "Other Discount";
                worksheet.Cells[1, 12].Value = "Debt";
                worksheet.Cells[1, 13].Value = "Shared Portion";
                worksheet.Cells[1, 14].Value = "Facility Portion";
                worksheet.Cells[1, 15].Value = "Social Insurance";
                worksheet.Cells[1, 16].Value = "Work Days";
                worksheet.Cells[1, 17].Value = "Date";
                worksheet.Cells[1, 18].Value = "Exchange Statement";
                worksheet.Cells[1, 19].Value = "Notes";

                // Add data
                for (int i = 0; i < salaries.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = salaries[i].emp_id;
                    worksheet.Cells[i + 2, 2].Value = salaries[i].employee?.name; // Get employee name
                    worksheet.Cells[i + 2, 3].Value = salaries[i].base_salary;
                    worksheet.Cells[i + 2, 4].Value = salaries[i].housing_allowances;
                    worksheet.Cells[i + 2, 5].Value = salaries[i].transportaion_allowances;
                    worksheet.Cells[i + 2, 6].Value = salaries[i].other_allowances;
                    worksheet.Cells[i + 2, 7].Value = salaries[i].overtime;
                    worksheet.Cells[i + 2, 8].Value = salaries[i].bonus;
                    worksheet.Cells[i + 2, 9].Value = salaries[i].delay_discount;
                    worksheet.Cells[i + 2, 10].Value = salaries[i].absence_discount;
                    worksheet.Cells[i + 2, 11].Value = salaries[i].other_discount;
                    worksheet.Cells[i + 2, 12].Value = salaries[i].debt;
                    worksheet.Cells[i + 2, 13].Value = salaries[i].shared_portion;
                    worksheet.Cells[i + 2, 14].Value = salaries[i].facility_portion;
                    worksheet.Cells[i + 2, 15].Value = salaries[i].Social_insurance;
                    worksheet.Cells[i + 2, 16].Value = salaries[i].work_days;
                    worksheet.Cells[i + 2, 17].Value = salaries[i].date.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 18].Value = salaries[i].exchange_statement;
                    worksheet.Cells[i + 2, 19].Value = salaries[i].notes;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = $"Salaries_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var salaryRecord = new salaries_history
                        {
                            emp_id = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                            base_salary = Convert.ToDouble(worksheet.Cells[row, 2].Value),
                            housing_allowances = Convert.ToDouble(worksheet.Cells[row, 3].Value),
                            transportaion_allowances = Convert.ToDouble(worksheet.Cells[row, 4].Value),
                            other_allowances = Convert.ToDouble(worksheet.Cells[row, 5].Value),
                            overtime = Convert.ToDouble(worksheet.Cells[row, 6].Value),
                            bonus = Convert.ToDouble(worksheet.Cells[row, 7].Value),
                            delay_discount = Convert.ToDouble(worksheet.Cells[row, 8].Value),
                            absence_discount = Convert.ToDouble(worksheet.Cells[row, 9].Value),
                            other_discount = Convert.ToDouble(worksheet.Cells[row, 10].Value),
                            debt = Convert.ToDouble(worksheet.Cells[row, 11].Value),
                            shared_portion = Convert.ToDouble(worksheet.Cells[row, 12].Value),
                            facility_portion = Convert.ToDouble(worksheet.Cells[row, 13].Value),
                            Social_insurance = Convert.ToDouble(worksheet.Cells[row, 14].Value),
                            work_days = Convert.ToInt32(worksheet.Cells[row, 15].Value),
                            date = Convert.ToDateTime(worksheet.Cells[row, 16].Value),
                            exchange_statement = worksheet.Cells[row, 17].Value.ToString(),
                            notes = worksheet.Cells[row, 18].Value?.ToString()
                        };

                        _context.SalaryHistories.Add(salaryRecord);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var hrManager = _context.employee_details
        .FirstOrDefault(e => e.position == "مدير الموارد البشرية والمالية");

            // Count transactions based on their status, ensuring no duplicates
            var newTransactions = await _context.Transactions

                .Where(t =>
     (t.status == "مرسلة" && t.Employee_detail.departement_id == employeeDetails.departement_id && t.Employee_detail.employee_id != employeeDetails.employee_id && t.Employee_detail.permission_position == "موظف") ||
    (t.status == "مرسلة" && (t.to_emp_id == employeeId || t.to_emp_id == hrManager.employee_id) && t.Employee_detail.permission_position != "موظف")
    || (t.status == "مرسلة" && t.department_id == employeeDetails.departement_id && t.Employee_detail.permission_position != "موظف" && t.Employee_detail.employee_id != employeeDetails.employee_id) ||// Transactions sent to the employee
    (t.Referrals.Any() && // Ensure there are referrals
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        ) &&
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        )
    )
)
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            var ongoingTransactions = await _context.Transactions
                .Where(t => t.to_emp_id == employeeId && t.status == "تحت الإجراء")
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            var completedTransactions = await _context.Transactions
                .Where(t => t.status == "منهاة" && t.to_emp_id == employeeId)
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.NewTransactionsCount = newTransactions;
            ViewBag.OngoingTransactionsCount = ongoingTransactions;
            ViewBag.CompletedTransactionsCount = completedTransactions;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ApproveHoliday(int holiday_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holiday_id);
            if (holiday == null)
            {
                return Json(new { success = false, message = "طلب الإجازة غير موجود" });
            }

            // Update the status to "Closed"
            holiday.status = "موافقة مدير الموارد البشرية";


            // Save changes to the database
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت الموافقة على الإجازة" });
        }

        [HttpPost]
        public async Task<IActionResult> DenyHoliday(int holiday_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holiday_id);
            if (holiday == null)
            {
                return Json(new { success = false, message = "طلب الإجازة غير موجود" });
            }

            // Update the status to "Closed"
            holiday.status = "رفضت من مدير الموارد البشرية";


            // Save changes to the database
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم رفض الإجازة" });
        }


        public IActionResult ManageSalaries(int selectedMonth, int selectedYear)
        {
            // If no month/year is selected, default to the current month and year
            if (selectedMonth == 0) selectedMonth = DateTime.Now.Month;
            if (selectedYear == 0) selectedYear = DateTime.Now.Year;

            // Get all employees
            var employees = _context.employee.ToList();

            // Get salaries for the selected month/year
            var salaries = _context.SalaryHistories
                .Where(s => s.date.Month == selectedMonth && s.date.Year == selectedYear)
                .ToList();

            // Create the ViewModel
            var viewModel = new SalariesViewModel
            {
                Employees = employees,
                Salaries = salaries,
                SelectedMonth = selectedMonth,
                SelectedYear = selectedYear
            };

            return View(viewModel);
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



        // Start of khaled work -----------------------------------------------------

        public IActionResult ReferTransaction()
        {
            return RedirectToAction("Transactions", "HR");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReferTransaction(List<IFormFile> files, int transaction_id, int to_employee_id, string comments)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }
            List<string> fileNames = new List<string>();
            if (files != null && files.Count > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };


                foreach (var file in files)
                {
                    // Validate the file type
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                        return View(transaction); // Return the view with validation error
                    }

                    // Save the file
                    string filename = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filePath = Path.Combine(path, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Add the filename to the list
                    fileNames.Add(filename);
                }

                // Concatenate the file names and store them in the transaction

            }
            // Get the current employee's ID from the session
            int fromEmployeeId;
            if (!int.TryParse(HttpContext.Session.GetString("Id"), out fromEmployeeId))
            {
                // Handle the case where the ID is not in the session or is invalid
                return RedirectToAction("LoginPage", "Home"); // Redirect to login or handle appropriately
            }

            var referral = new Referral
            {
                transaction_id = transaction_id,
                from_employee_id = fromEmployeeId, // Use the ID from the session
                to_employee_id = to_employee_id,
                referral_date = DateTime.Now,
                comments = comments,
                files = string.Join(",", fileNames)
            };

            _context.Referrals.Add(referral);
            transaction.to_emp_id = to_employee_id;
            transaction.status = "تحت الإجراء";
            await _context.SaveChangesAsync();

            // Redirect to the Transactions page after successful referral
            return RedirectToAction("Transactions");
        }

        // New method to view referral history
        public async Task<IActionResult> ReferralHistory(int id)
        {
            _logger.LogInformation($"Fetching referral history for transaction {id}");

            var referrals = await _context.Referrals
                .Where(r => r.transaction_id == id)
                .Include(r => r.from_employee)
                .Include(r => r.to_employee)
                .OrderByDescending(r => r.referral_date)
                .ToListAsync();

            _logger.LogInformation($"Found {referrals.Count} referrals");

            foreach (var referral in referrals)
            {
                _logger.LogInformation($"Referral {referral.referral_id}: From {referral.from_employee_id} ({referral.from_employee?.name ?? "N/A"}) To {referral.to_employee_id} ({referral.to_employee?.name ?? "N/A"})");
            }

            return View(referrals);
        }

        // End of khaled work -----------------------------------------------------

        private int GetEmployeeIdFromSession()
        {
            var employeeIdString = HttpContext.Session.GetString("Id");
            if (employeeIdString != null)
            {
                return int.Parse(employeeIdString);

            }

            return 0;
        }
        private async Task<employee_details> GetEmployeeDetailsFromSessionAsync()
        {
            var employeeIdString = HttpContext.Session.GetString("Id");

            if (employeeIdString != null && int.TryParse(employeeIdString, out int employeeId))
            {
                var employeeDetails = await _context.employee_details
                    .Include(ed => ed.employee)
                   .Include(ed => ed.Department)
                    .FirstOrDefaultAsync(ed => ed.employee_id == employeeId);

                return employeeDetails;
            }

            return null;
        }





        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

        public async Task<IActionResult> Transactions()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            int currentUserId = GetEmployeeIdFromSession();
            var hrManager = _context.employee_details
      .FirstOrDefault(e => e.position == "مدير الموارد البشرية والمالية");

            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(), // Use the department ID as the value
                Text = d.departement_name            // Use the department name as the text
            }).ToList();
            var holidayTypes = await _context.Holidays.ToListAsync();
            ViewData["HolidayTypes"] = holidayTypes ?? new List<Holiday>();

            ViewData["Employees"] = _context.employee.Select(e => new SelectListItem
            {
                Value = e.employee_id.ToString(),
                Text = e.name
            }).ToList();
            // Filter transactions based on the current user's ID


            int internalCount = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
     (t.status == "مرسلة" && t.Employee_detail.departement_id == employeeDetails.departement_id && t.Employee_detail.employee_id != employeeDetails.employee_id && t.Employee_detail.permission_position == "موظف") ||
    (t.status == "مرسلة" && (t.to_emp_id == currentUserId || t.to_emp_id == hrManager.employee_id) && t.Employee_detail.permission_position != "موظف")
    || (t.status == "مرسلة" && t.department_id == employeeDetails.departement_id && t.Employee_detail.permission_position != "موظف" && t.Employee_detail.employee_id != employeeDetails.employee_id) ||// Transactions sent to the employee
    (t.Referrals.Any() && // Ensure there are referrals
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == currentUserId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        ) &&
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == currentUserId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        )
    )
).CountAsync();
            int holidaysCount = await _context.HolidayHistories
                 .Where(h => h.status == "موافقة المدير المباشر" ||
                (h.status == "مرسلة" &&
                 (h.Employee_detail.departement_id == employeeDetails.departement_id ||
                  h.Employee_detail.Department.departement_name == "الادارة التنفيذية")))
                .CountAsync();
            int lettersCount = await _context.letters
                  .Where(l => l.to_emp_id == employeeDetails.employee_id || (l.to_departement_name == employeeDetails.Department.departement_name && l.to_emp_id == 0))
                .CountAsync();
            int assetsCount = await _context.charter
             .Where(c => c.status != "مستلمة")
                .CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.InternalCount = internalCount;
            ViewBag.HolidaysCount = holidaysCount;
            ViewBag.LettersCount = lettersCount;
            ViewBag.AssetsCount = assetsCount;

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartmentName(int departmentId)
        {
            var department = await _context.Department.FindAsync(departmentId);
            if (department != null)
            {
                return Content(department.departement_name);
            }
            return NotFound();
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var hrManager = _context.employee_details
        .FirstOrDefault(e => e.position == "مدير الموارد البشرية والمالية");

            // Fetch transactions based on the conditions provided
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
     (t.status == "مرسلة" && t.Employee_detail.departement_id == employeeDetails.departement_id && t.Employee_detail.employee_id != employeeDetails.employee_id && t.Employee_detail.permission_position == "موظف") ||
    (t.status == "مرسلة" && (t.to_emp_id == employeeId || t.to_emp_id == hrManager.employee_id) && t.Employee_detail.permission_position != "موظف")
    || (t.status == "مرسلة" && t.department_id == employeeDetails.departement_id && t.Employee_detail.permission_position != "موظف" && t.Employee_detail.employee_id != employeeDetails.employee_id) ||// Transactions sent to the employee
    (t.Referrals.Any() && // Ensure there are referrals
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        ) &&
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == hrManager.employee_id
        )
    )
)

                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();
            var employeeIds = transactions.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");
            if (transactions.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_getAllTransactions", transactions);
        }

        public async Task<IActionResult> GetAllCharters()
        {
            var charter = await _context.charter
                .Include(c => c.employee)
                .Where(c => c.status != "مستلمة")
                .OrderByDescending(t => t.charter_id) // Order by transaction_id in descending order
                .ToListAsync();
            if (charter.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_GetAllCharters", charter);
        }


        public async Task<IActionResult> GetAllHolidays()
        {
            var emplyee_Details = await GetEmployeeDetailsFromSessionAsync();
            var holidays = await _context.HolidayHistories
                .Include(h => h.holiday)  // Eager load the Holiday entity
                .Where(h => h.status == "موافقة المدير المباشر" ||
                (h.status == "مرسلة" &&
                 (h.Employee_detail.departement_id == emplyee_Details.departement_id ||
                  h.Employee_detail.Department.departement_name == "الادارة التنفيذية")))
    .OrderByDescending(h => h.holidays_history_id)
    .ToListAsync();
            var employeeIds = holidays.SelectMany(t => new[] { t.emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;
            if (holidays.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_getAllHolidays", holidays);
        }


        public async Task<IActionResult> GetAllLetters()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var letters = await _context.letters
                  .Where(l => l.to_emp_id == employeeDetails.employee_id || (l.to_departement_name == employeeDetails.Department.departement_name && l.to_emp_id == 0))
                .OrderByDescending(l => l.letters_id) // Order by letters_id in descending order
                .ToListAsync();
            if (letters.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }

            var employeeIds = letters.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            return PartialView("_getAllLetters", letters);
        }
        ////////////////////////////////////////////////////////////////////     Archive //////////////////////////////
        ///
        public async Task<IActionResult> Archive()
        {
            return View();
        }
        public async Task<IActionResult> GetAllTransactionsArchive()
        {
            var employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();


            // Fetch transactions based on the conditions provided
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
     (t.status == "منهاة" && t.department_id == employeeDetails.departement_id))

                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();
            var employeeIds = transactions.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");
            if (transactions.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_getAllTransactions", transactions);
        }


        public async Task<IActionResult> GetAllHolidaysArchive()
        {
            var employee = await GetEmployeeDetailsFromSessionAsync();

            // Fetch holidays with the status "مرسلة" where the employee's department ID is 5
            var holidays = await _context.HolidayHistories
                .Include(h => h.holiday)
                .Include(h => h.holiday)
               .Where(h =>
        (h.status.Contains("موافقة") || h.status.Contains("رفضت"))
        && h.Employee_detail.departement_id == employee.departement_id)
                .OrderByDescending(h => h.holidays_history_id)
                .ToListAsync();
            var employeeIds = holidays.SelectMany(t => new[] { t.emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;
            if (holidays.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }

            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLettersArchive()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var letters = await _context.letters
                  .Where(l => l.to_emp_id == employeeDetails.employee_id || (l.to_departement_name == employeeDetails.Department.departement_name && l.to_emp_id == 0))
                .OrderByDescending(l => l.letters_id) // Order by letters_id in descending order
                .ToListAsync();
            if (letters.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }

            var employeeIds = letters.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            return PartialView("_getAllLetters", letters);
        }
        public async Task<IActionResult> GetAllChartersArchive()
        {
            var employe_details = await GetEmployeeDetailsFromSessionAsync();

            var charter = await _context.charter
                .Include(c => c.employee)
                .Where(c => c.status == "مستلمة")
                .OrderByDescending(t => t.charter_id) // Order by transaction_id in descending order
                .ToListAsync();
            if (charter.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_GetAllCharters", charter);
        }

        // Search Actions  --------------------------------------------------------



        // Create Actions  --------------------------------------------------------



        public void Create_Charter()
        {
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            // Initial load, employees list will be empty until a department is selected.
            ViewData["Employees"] = new List<SelectListItem>();
        }

        // New action to get employees by department
        [HttpGet]
        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
        {
            _logger.LogInformation($"Fetching employees for department ID: {departmentId}");

            var employees = await _context.employee_details
                .Where(ed => ed.departement_id == departmentId)
                .Select(ed => new
                {
                    employee_id = ed.employee_id,
                    name = ed.employee.name,
                    position = ed.position
                })
                .GroupBy(e => e.employee_id)
                .Select(g => g.First())
                .ToListAsync();

            if (!employees.Any())
            {
                _logger.LogWarning($"No employees found for department ID: {departmentId}");
                return NotFound("No employees found for the given department.");
            }

            _logger.LogInformation($"Found {employees.Count} employees for department ID: {departmentId}");
            return Ok(employees);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesByDepartmentName([FromQuery] int[] departmentNames)
        {

            // Find department IDs by names
            var departmentIds = await _context.Department
                .Where(d => departmentNames.Contains(d.departement_id))
                .Select(d => d.departement_id)
                .ToListAsync();

            if (!departmentIds.Any())
            {
                return NotFound("No departments found with the given names.");
            }

            // Fetch employees based on the department IDs
            var employees = await _context.employee_details
                .Where(ed => departmentIds.Contains(ed.departement_id))
                .Select(ed => new
                {
                    employee_id = ed.employee_id,
                    name = ed.employee.name,
                    position = ed.position
                })
                .GroupBy(e => e.employee_id)
                .Select(g => g.First())
                .ToListAsync();

            if (!employees.Any())
            {
                return NotFound("No employees found for the given departments.");
            }

            return Ok(employees);
        }




        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Charter([Bind("charter_id,charter_info,serial_number,creation_date,notes,to_departement_name,to_emp_id,receive_date,end_date")] charter charter)
        {


            charter.status = "غير مسلمة";
            charter.from_departement_name = "الموارد البشرية والمالية";
            _context.Add(charter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));


        }
        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Transaction(List<IFormFile> files, [Bind("create_date,close_date,title,description,to_emp_id,department_id,Confidentiality,Urgency,Importance")] Transaction transaction)
        {
            // Retrieve the employee ID from session
            var employeeId = GetEmployeeIdFromSession();
            transaction.from_emp_id = employeeId;

            // Check if files were uploaded
            if (files != null && files.Count > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                List<string> fileNames = new List<string>();

                foreach (var file in files)
                {
                    // Validate the file type
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                        return View(transaction); // Return the view with validation error
                    }

                    // Save the file
                    string filename = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filePath = Path.Combine(path, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Add the filename to the list
                    fileNames.Add(filename);
                }

                // Concatenate the file names and store them in the transaction
                transaction.files = string.Join(",", fileNames);
            }

            if (transaction.create_date == null)
            {
                transaction.create_date = DateTime.Now;
            }

            transaction.status = "مرسلة";
            _context.Add(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Holiday(IFormFile files, [Bind("title,description,duration,start_date,end_date,holiday_id")] HolidayHistory holidayHistory)
        {
            // Get the logged-in employee's ID from the session
            var employeeId = GetEmployeeIdFromSession();
            holidayHistory.emp_id = employeeId; // Assign the employee ID to the holiday

            if (files != null && files.Length > 0)
            {
                // Validate the file type
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                var extension = Path.GetExtension(files.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                    return View(holidayHistory); // Return the view with validation error
                }

                string filename = Path.GetFileName(files.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(path, filename);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    await files.CopyToAsync(filestream);
                }
                holidayHistory.files = filename;
            }

            holidayHistory.creation_date = DateOnly.FromDateTime(DateTime.Now); // Set to current date
            holidayHistory.status = "مرسلة من مدير"; // Set default status
            _context.Add(holidayHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }


    

        /// <summary>
        /// /thisssssssssssssssssss is Newwwwwwwwwwwwwww Action
        /// </summary>
        /// <returns></returns>

        /*  public IActionResult Create_Letter()
          {
              // Any initialization code if needed
              return View(); // This will look for a view named "Create_Letter" by default
          }


          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Create_Letter(IFormFile files, string[] to_departement_name, string[] to_emp_id, [Bind("title,description,type,from_emp_id,Confidentiality,Urgency,Importance")] letter letter)
          {
              // Handle file upload
              if (files != null && files.Length > 0)
              {
                  var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                  var extension = Path.GetExtension(files.FileName).ToLower();

                  if (!allowedExtensions.Contains(extension))
                  {
                      ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                      return View(letter);
                  }

                  string filename = Path.GetFileName(files.FileName);
                  string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                  if (!Directory.Exists(path))
                  {
                      Directory.CreateDirectory(path);
                  }
                  string filePath = Path.Combine(path, filename);
                  using (var filestream = new FileStream(filePath, FileMode.Create))
                  {
                      await files.CopyToAsync(filestream);
                  }
                  letter.files = filename;
              }

              // Convert selected departments and employees to comma-separated strings
              if (to_departement_name != null)
              {
                  letter.to_departement_name = string.Join(",", to_departement_name);
              }

             if (to_emp_id != null)
              {
                  letter.to_emp_id = string.Join(",", to_emp_id);
              }

              if (ModelState.IsValid)
              {
                  letter.date = DateTime.Now;
                  letter.departement_id = 3;
                  _context.Add(letter);
                  await _context.SaveChangesAsync();
                  return RedirectToAction(nameof(Index));
              }


              return View(letter);
          }*/


        ///////   Old Code
        public void Create_Letter()
        {
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_name,
                Text = d.departement_name
            }).ToList();

            ViewData["Employees"] = _context.employee.Select(e => new SelectListItem
            {
                Value = e.employee_id.ToString(),
                Text = e.name
            }).ToList();


        }



        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Letter(
    IFormFile files,
    string[]? to_departement_name,
    string[]? to_emp_id,
    [Bind("title,description,type,from_emp_id,files,Confidentiality,Urgency,Importance")] letter letter)
        {
            if (files != null && files.Length > 0)
            {
                // Validate the file type
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                var extension = Path.GetExtension(files.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                    return View(letter); // Return the view with validation error
                }

                string filename = Path.GetFileName(files.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(path, filename);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    await files.CopyToAsync(filestream);
                }
                letter.files = filename;
            }


                letter.date = DateTime.Now; // Set the current date
         letter.from_emp_id = 23;
         letter.departement_id = 1;

                // If departments are selected, send the letter to the departments
                if (to_departement_name != null && to_departement_name.Any())
                {
                    foreach (var dept in to_departement_name)
                    {
                        var newLetter = new letter
                        {
                            title = letter.title,
                            description = letter.description,
                            type = letter.type,
                            from_emp_id = letter.from_emp_id,
                            files = letter.files,
                            Confidentiality = letter.Confidentiality,
                            Urgency = letter.Urgency,
                            Importance = letter.Importance,
                            date = letter.date,
                            departement_id = letter.departement_id,
                            to_departement_name = dept,
                            to_emp_id = letter.to_emp_id // No employee selected
                        };

                        _context.Add(newLetter);
                    }
                }

                // If employees are selected, send the letter to the employees
                if (to_emp_id != null && to_emp_id.Any())
                {
                    foreach (var emp in to_emp_id)
                    {
                        var newLetter = new letter
                        {
                            title = letter.title,
                            description = letter.description,
                            type = letter.type,
                            from_emp_id = letter.from_emp_id,
                            files = letter.files,
                            Confidentiality = letter.Confidentiality,
                            Urgency = letter.Urgency,
                            Importance = letter.Importance,
                            date = letter.date,
                            departement_id = letter.departement_id,
                            to_departement_name = letter.to_departement_name, // No department selected
                            to_emp_id = int.Parse(emp)
                        };

                        _context.Add(newLetter);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Transactions));


            return View(letter);
        }
*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Letter(
     List<IFormFile> files,
     int[]? to_departement_name,
     string[]? to_emp_id,
     [Bind("title,description,type,from_emp_id,date,files,Confidentiality,Urgency,Importance")] letter letter)
        {
            // Retrieve employee details from the session
            var employee_details = await GetEmployeeDetailsFromSessionAsync();
            letter.from_emp_id = employee_details.employee_id;
            letter.departement_id = employee_details.departement_id;

            // Check if files were uploaded
            if (files != null && files.Count > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                List<string> fileNames = new List<string>();

                foreach (var file in files)
                {
                    // Validate the file type
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                        return View(letter); // Return the view with validation error
                    }

                    // Save the file
                    string filename = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string filePath = Path.Combine(path, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Add the filename to the list
                    fileNames.Add(filename);
                }

                // Concatenate the file names and store them in the letter
                letter.files = string.Join(",", fileNames);
            }

            // Set the date to now if not provided
            letter.date = DateTime.Now;

            bool letterCreated = false;
            HashSet<int> processedEmployees = new HashSet<int>();

            // If specific employees are chosen, prioritize them
            if (to_emp_id != null && to_emp_id.Any() && letter.type != "تظلم")
            {
                foreach (var empId in to_emp_id.Select(int.Parse))
                {
                    // Find the employee and their department
                    var employee = await _context.employee_details
                                                 .Include(ed => ed.Department)
                                                 .FirstOrDefaultAsync(ed => ed.employee_id == empId);

                    if (employee != null)
                    {
                        var newLetter = new letter
                        {
                            title = letter.title,
                            description = letter.description,
                            type = letter.type,
                            from_emp_id = letter.from_emp_id,
                            files = letter.files,
                            Confidentiality = letter.Confidentiality,
                            Urgency = letter.Urgency,
                            Importance = letter.Importance,
                            date = letter.date,
                            to_emp_id = empId,
                            to_departement_name = employee.Department.departement_name,
                            departement_id = letter.departement_id
                        };

                        _context.Add(newLetter);
                        processedEmployees.Add(empId);
                        letterCreated = true;
                    }
                }
            }

            // If only departments are selected, create letters with to_emp_id set to 0
            if ((to_emp_id == null || !to_emp_id.Any()) && to_departement_name != null && to_departement_name.Any() && letter.type != "تظلم")
            {
                foreach (var deptId in to_departement_name)
                {
                    // Find the department by ID
                    var department = await _context.Department.FirstOrDefaultAsync(d => d.departement_id == deptId);

                    if (department != null)
                    {
                        // Get all active employees in the selected department
                        var departmentEmployees = await _context.employee_details
                            .Where(ed => ed.departement_id == deptId && ed.active)
                            .Select(ed => ed.employee_id)
                            .ToListAsync();

                        // Create a single letter for the department with to_emp_id set to 0
                        if (departmentEmployees.Any())
                        {
                            var newLetter = new letter
                            {
                                title = letter.title,
                                description = letter.description,
                                type = letter.type,
                                from_emp_id = letter.from_emp_id,
                                files = letter.files,
                                Confidentiality = letter.Confidentiality,
                                Urgency = letter.Urgency,
                                Importance = letter.Importance,
                                date = letter.date,
                                to_emp_id = 0, // Set to_emp_id to 0 because this is department-wide
                                to_departement_name = department.departement_name,
                                departement_id = letter.departement_id
                            };

                            _context.Add(newLetter);
                            letterCreated = true;
                        }
                    }
                }
            }

            // Additional condition: If type is "تظلم" and no departments or employees are chosen
            if (letter.type == "تظلم")
            {
                // Create a letter with to_emp_id set to 0 and to_departement_name set to null
                var newLetter = new letter
                {
                    title = letter.title,
                    description = letter.description,
                    type = letter.type,
                    from_emp_id = letter.from_emp_id,
                    files = letter.files,
                    Confidentiality = letter.Confidentiality,
                    Urgency = letter.Urgency,
                    Importance = letter.Importance,
                    date = letter.date,
                    to_emp_id = 0,
                    to_departement_name = null, // Set to_departement_name to null
                    departement_id = letter.departement_id
                };

                _context.Add(newLetter);
                letterCreated = true;
            }

            // If no departments or employees are chosen, create a letter for the default department
            if (!letterCreated)
            {
                var newLetter = new letter
                {
                    title = letter.title,
                    description = letter.description,
                    type = letter.type,
                    from_emp_id = letter.from_emp_id,
                    files = letter.files,
                    Confidentiality = letter.Confidentiality,
                    Urgency = letter.Urgency,
                    Importance = letter.Importance,
                    date = letter.date,
                    to_emp_id = letter.from_emp_id,
                    departement_id = letter.departement_id
                };

                _context.Add(newLetter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }






        /*       [HttpPost]
                   [ValidateAntiForgeryToken]
                   public async Task<IActionResult> Create_Letter(IFormFile files, [Bind("title,description,type,from_emp_id,to_emp_id,files,Confidentiality,Urgency,Importance,to_departement_name")] letter letter)
                   {


                       if (files != null && files.Length > 0)
                       {
                           // Validate the file type
                           var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                           var extension = Path.GetExtension(files.FileName).ToLower();

                           if (!allowedExtensions.Contains(extension))
                           {
                               ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                               return View(letter); // Return the view with validation error
                           }

                           string filename = Path.GetFileName(files.FileName);
                           string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                           if (!Directory.Exists(path))
                           {
                               Directory.CreateDirectory(path);
                           }
                           string filePath = Path.Combine(path, filename);
                           using (var filestream = new FileStream(filePath, FileMode.Create))
                           {
                               await files.CopyToAsync(filestream);
                           }
                           letter.files = filename;
                       }


                           letter.date = DateTime.Now; // Set the current date
                           letter.departement_id = 3;
                           _context.Add(letter);
                           await _context.SaveChangesAsync();
                           return RedirectToAction(nameof(Transactions));


                    }*/


        /*        [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Create_Letter(string title,
            string description,
            string type,
            int from_emp_id,
            int to_emp_id,
            string Confidentiality,
            string Urgency,
            string Importance,
            string to_departement_name, IFormFile files)
                {
                    var letter = new letter
                    {
                        title = title,
                        description = description,
                        type = type,
                        from_emp_id = from_emp_id,
                        to_emp_id = to_emp_id,
                        Confidentiality = Confidentiality,
                        Urgency = Urgency,
                        Importance = Importance,
                        to_departement_name = to_departement_name
                    };

                    if (files != null && files.Length > 0)
                    {
                        // Validate the file type
                        var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                        var extension = Path.GetExtension(files.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                            return View(letter); // Return the view with validation error
                        }

                        string filename = Path.GetFileName(files.FileName);
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string filePath = Path.Combine(path, filename);
                        using (var filestream = new FileStream(filePath, FileMode.Create))
                        {
                            await files.CopyToAsync(filestream);
                        }
                        letter.files = filename;
                    }

                    if (ModelState.IsValid)
                    {
                        letter.date = DateTime.Now; // Set the current date
                        letter.departement_id = 3;
                        _context.Add(letter);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(letter);
                }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDevice([Bind("name,quantity")] Devices device)
        {
            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewDevice));
            }
            return View(device);
        }





        // Update Actions --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionStatus(int transaction_id, string TerminationCause)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            transaction.status = "منهاة";
            transaction.close_date = DateTime.Now;
            transaction.TerminationCause = TerminationCause;
            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
        }



        // Delete Actions --------------------------------------------------------


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
        public async Task<IActionResult> ViewDevice()
        {
            var devices = await _context.Devices.ToListAsync();
            return View(devices);
        }




        ///////////////////////////////////////// employee ////////////////////////////////
        ///
        public IActionResult CreateEmployee()
        {
            var departments = _context.Department.ToList();
            ViewData["Departments"] = departments;
            return View();
        }
        public IActionResult EditEmployee()
        {
            return View();
        }
        public IActionResult EmployeeProfile()
        {

            return View();
        }
        public IActionResult EmployeeView()
        {
            var employeeList = _context.employee
                .Include(e => e.EmployeeDetails) // Include employee details
                .ThenInclude(ed => ed.Department) // Include department details from employee details
                .Select(e => new
                {
                    e.employee_id,
                    e.name,
                    e.username,
                    Position = e.EmployeeDetails != null ? e.EmployeeDetails.position : "No Position",
                    PermissionPosition = e.EmployeeDetails != null ? e.EmployeeDetails.permission_position : "No Permission",
                    DepartmentName = e.EmployeeDetails != null && e.EmployeeDetails.Department != null
                        ? e.EmployeeDetails.Department.departement_name
                        : "No Department",
                    Files = e.EmployeeDetails != null ? e.EmployeeDetails.files : null,
                    contract_type = e.EmployeeDetails.contract_type // Include files information
                })
                .ToList();
            var departments = _context.Department.ToList();
            ViewData["Departments"] = departments;
            ViewData["EmployeeList"] = employeeList;
            return View();
        }

        public async Task<IActionResult> ManageEmpPer()
        {
            var currentEmployee = await GetEmployeeDetailsFromSessionAsync();


            var employeeList = await _context.employee
                .Include(e => e.EmployeeDetails)
                .ThenInclude(ed => ed.Department)
                .Where(e => e.EmployeeDetails.Department.departement_id == currentEmployee.departement_id && e.EmployeeDetails.employee_id != currentEmployee.employee_id)
                .Select(e => new
                {
                    e.employee_id,
                    e.name,
                    e.username,
                    Position = e.EmployeeDetails != null ? e.EmployeeDetails.position : "No Position",
                    PermissionPosition = e.EmployeeDetails != null ? e.EmployeeDetails.permission_position : "No Permission",
                    DepartmentName = e.EmployeeDetails != null && e.EmployeeDetails.Department != null
                        ? e.EmployeeDetails.Department.departement_name
                        : "No Department"
                })
                .ToListAsync();

            var departments = await _context.Department.ToListAsync();
            ViewData["Departments"] = departments;
            ViewData["EmployeeList"] = employeeList;
            return View();
        }
        public async Task<IActionResult> GetRejectedHolidays()
        {

            var holidays = await _context.HolidayHistories
                .Include(h => h.holiday)  // Eager load the Holiday entity
                .Where(h => h.status == "رفضت من المدير المباشر" || h.status == "رفضت من مدير الموارد البشرية") // Filter for rejected holidays
                .OrderByDescending(h => h.holidays_history_id)
                .ToListAsync();
            var employeeIds = holidays.SelectMany(t => new[] { t.emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            if (holidays.Count == 0)
            {
                // Render the _NothingNew partial view if no holidays are found
                return PartialView("_NothingNew");
            }
            return PartialView("_getRejectedHolidays", holidays); // Separate partial view for rejected holidays
        }
        public async Task<IActionResult> GetMyHolidays()
        {
            var emp = GetEmployeeIdFromSession();

            // Fetch the employee name for the current employee
            // Store the single employee name in ViewBag

            var holidays = await _context.HolidayHistories
                .Include(h => h.holiday) // Eager load the Holiday entity
                .Where(h => h.emp_id == emp)
                .OrderByDescending(h => h.holidays_history_id)
                .ToListAsync();

            var employeeIds = holidays.SelectMany(t => new[] { t.emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            if (!holidays.Any())
            {
                // Render the _NothingNew partial view if no holidays are found
                return PartialView("_NothingNew");
            }

            return PartialView("_getAllHolidays", holidays); // Separate partial view for rejected holidays
        }
        [HttpPost]
        public IActionResult UpdatePermission(int id, string permissionPosition)
        {
            try
            {
                // Find the employee by ID
                var employee = _context.employee_details.FirstOrDefault(e => e.employee_details_id == id);

                if (employee == null)
                {
                    return Json(new { success = false, message = "الموظف غير موجود." });
                }

                // Update the employee's permission position
                employee.permission_position = permissionPosition;

                // Save the changes to the database
                _context.SaveChanges();

                // Return a success response
                return Json(new { success = true, message = "تم تحديث صلاحيات الموظف بنجاح." });
            }
            catch (Exception ex)
            {
                // Handle the error and return a failure response
                return Json(new { success = false, message = "حدث خطأ أثناء تحديث صلاحيات الموظف." });
            }
        }

        public IActionResult DownloadFile(int employeeId)
        {
            var employeeDetails = _context.employee_details
                .FirstOrDefault(ed => ed.employee_id == employeeId);

            if (employeeDetails == null || string.IsNullOrEmpty(employeeDetails.files))
            {
                return NotFound("File not found");
            }

            // Combine the path with wwwroot to get the full file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", employeeDetails.files);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                var mimeType = "application/octet-stream";
                return File(fileBytes, mimeType, fileName);
            }
            else
            {
                return NotFound("File not found on the server");
            }
        }

        public IActionResult DownloadFile_HY(int holidayId)
        {
            var holiday = _context.HolidayHistories
                .FirstOrDefault(h => h.holidays_history_id == holidayId);

            if (holiday == null || string.IsNullOrEmpty(holiday.files))
            {
                return NotFound("File not found");
            }

            // Combine the path with wwwroot to get the full file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", holiday.files);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);
                var mimeType = "application/octet-stream";
                return File(fileBytes, mimeType, fileName);
            }
            else
            {
                return NotFound("File not found on the server");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertEmployee(
     IFormFile employee_files,
     string employee_name,
     string employee_username,
     string employee_password,
     string? employee_search_role,
     string employee_identity_number,
     string employee_departement_id,
     string employee_position,
     string employee_permission_position,
     string employee_contract_type,
     string employee_national_address,
     string employee_education_level,
     DateTime employee_hire_date,
     DateTime employee_leave_date,
     string employee_email,
     string employee_phone_number,
     string employee_gender,
     bool employee_active)
        {
            var employee = new employee
            {
                name = employee_name,
                username = employee_username,
                password = employee_password,
                search_role = employee_search_role
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.employee.Add(employee);
                await _context.SaveChangesAsync();

                var employeeDetails = new employee_details
                {
                    employee_details_id = employee.employee_id,
                    employee_id = employee.employee_id, // Ensuring both IDs are the same
                    identity_number = int.Parse(employee_identity_number),
                    departement_id = int.Parse(employee_departement_id),
                    position = employee_position,
                    permission_position = employee_permission_position,
                    contract_type = employee_contract_type,
                    national_address = employee_national_address,
                    education_level = employee_education_level,
                    hire_date = employee_hire_date,
                    leave_date = employee_leave_date,
                    email = employee_email,
                    phone_number = employee_phone_number,
                    gender = employee_gender,
                    active = employee_active,

                };

                if (employee_files != null && employee_files.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                    var extension = Path.GetExtension(employee_files.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                        return Json(new { success = false, message = "الملف الذي تم تحميله غير صالح. يرجى تحميل ملف PDF أو Excel أو Word." });
                    }

                    string filename = Path.GetFileName(employee_files.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string filePath = Path.Combine(path, filename);
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await employee_files.CopyToAsync(filestream);
                    }
                    employeeDetails.files = filename;
                }

                _context.employee_details.Add(employeeDetails);
                await _context.SaveChangesAsync();
                var department = _context.Department.FirstOrDefault(d => d.departement_id == employeeDetails.departement_id);
                if (employee_position == "السكرتير" && department.departement_name != "الادارة التنفيذية")
                {
                    return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
                }
                // Check if the position contains 'مدير'
                if (employee_position.Contains("مدير"))
                {

                    if (department != null)
                    {
                        var oldSupervisorId = department.supervisor_id;

                        // Extract the part of the position after 'مدير'
                        var positionSuffix = employee_position.Replace("مدير", "").Trim();

                        // Check if the department name matches the position suffix
                        if (department.departement_name != positionSuffix)
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
                        }
                        //---------------------------

                        // Check if the old supervisor ID is not the same as the current employee ID
                        if (oldSupervisorId != employee.employee_id)
                        {
                            // Update the old supervisor's position and permission_position
                            var oldSupervisor = _context.employee.Include(e => e.EmployeeDetails).FirstOrDefault(e => e.employee_id == oldSupervisorId);
                            if (oldSupervisor != null)
                            {
                                var oldSupervisorDetails = oldSupervisor.EmployeeDetails;
                                if (oldSupervisorDetails != null)
                                {
                                    oldSupervisorDetails.position = "موظف";
                                    oldSupervisorDetails.permission_position = "موظف";
                                }
                            }
                            if (employee_permission_position.Contains("مدير"))
                            {

                                // Set the new supervisor
                                department.supervisor_id = employee.employee_id;
                            }
                        }
                    }
                }

                if (employee_permission_position.Trim() != "المدير التنفيذي" && employee_permission_position.Contains("مدير"))
                {
                    if (department != null)
                    {
                        var permissionPositionSuffix = employee_permission_position.Replace("مدير", "").Trim();
                        if (department.departement_name != permissionPositionSuffix)
                        {
                            return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات الادارة الخاصة بهذا القسم لموظف خارج القسم" });
                        }
                    }
                }


                if (employee_permission_position.Trim() == "المدير التنفيذي")
                {
                    if (department != null)
                    {

                        if (department.departement_name != "الادارة التنفيذية")
                        {
                            return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات الادارة الخاصة بهذا القسم لموظف خارج القسم" });
                        }
                    }
                }
                if (employee_permission_position == "السكرتير")
                {
                    if (department != null)
                    {

                        if (department.departement_name != "الادارة التنفيذية")
                        {
                            return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات السكرتير لموظف خارج قسم الادارة التنفيذية" });
                        }
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "تم إنشاء موظف جديد بنجاح!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "لم يتم إنشاء الموظف." });
            }
        }





        public IActionResult UpdateEmployee(int id)
        {
            // Fetch the employee details, including related EmployeeDetails
            var employee = _context.employee
                .Include(e => e.EmployeeDetails)
                .FirstOrDefault(e => e.employee_id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Fetch the employee's salary details from the employee_salary table
            var salary = _context.EmployeeSalary
                .FirstOrDefault(s => s.emp_id == id);

            // Fetch the list of departments
            var departments = _context.Department.ToList();

            // Prepare employee data including salary details
            var employeeData = new
            {
                employee.employee_id,
                employee.name,
                employee.username,
                employee.password,
                employee.search_role,
                employee.EmployeeDetails.identity_number,
                employee.EmployeeDetails.departement_id,
                employee.EmployeeDetails.position,
                employee.EmployeeDetails.permission_position,
                employee.EmployeeDetails.contract_type,
                employee.EmployeeDetails.national_address,
                employee.EmployeeDetails.education_level,
                employee.EmployeeDetails.hire_date,
                employee.EmployeeDetails.leave_date,
                employee.EmployeeDetails.email,
                employee.EmployeeDetails.phone_number,
                employee.EmployeeDetails.gender,
                employee.EmployeeDetails.active,
                SalaryDetails = salary == null ? new
                {
                    base_salary = 0m, // Using decimal
                    housing_allowances = (decimal?)0,
                    transportation_allowances = (decimal?)0,
                    other_allowances = (decimal?)0,
                    shared_portion = (decimal?)0,
                    facility_portion = (decimal?)0,
                    Social_insurance_rate = (decimal?)0,
                    max_overtime_rate = (decimal?)0,
                    salary_notes = string.Empty // assuming you want an empty string if null
                } : new
                {
                    salary.base_salary,
                    salary.housing_allowances,
                    salary.transportation_allowances,
                    salary.other_allowances,
                    salary.shared_portion,
                    salary.facility_portion,
                    salary.Social_insurance_rate,
                    salary.max_overtime_rate,
                    salary.salary_notes
                }
            };

            // Pass the data to the view
            ViewData["EmployeeData"] = employeeData;
            ViewData["Departments"] = departments;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeSalary(int empId, decimal baseSalary, decimal? housingAllowances, decimal? transportationAllowances, decimal? otherAllowances, decimal? sharedPortion, decimal? facilityPortion, decimal? socialInsuranceRate, decimal? maxOvertimeRate, string salaryNotes)
        {
            // Find the employee salary by empId
            var salary = await _context.EmployeeSalary
                .FirstOrDefaultAsync(s => s.emp_id == empId);

            if (salary == null)
            {
                // Create a new salary record if it doesn't exist
                salary = new Salary
                {
                    emp_id = empId,
                    base_salary = baseSalary,
                    housing_allowances = housingAllowances,
                    transportation_allowances = transportationAllowances,
                    other_allowances = otherAllowances,
                    shared_portion = sharedPortion,
                    facility_portion = facilityPortion,
                    Social_insurance_rate = socialInsuranceRate,
                    max_overtime_rate = maxOvertimeRate,
                    salary_notes = salaryNotes
                };

                // Add the new salary record to the database
                await _context.EmployeeSalary.AddAsync(salary);
            }
            else
            {
                // Update the existing salary record
                salary.base_salary = baseSalary;
                salary.housing_allowances = housingAllowances;
                salary.transportation_allowances = transportationAllowances;
                salary.other_allowances = otherAllowances;
                salary.shared_portion = sharedPortion;
                salary.facility_portion = facilityPortion;
                salary.Social_insurance_rate = socialInsuranceRate;
                salary.max_overtime_rate = maxOvertimeRate;
                salary.salary_notes = salaryNotes;
            }

            // Save the changes (either update or create)
            await _context.SaveChangesAsync();

            // Optionally, redirect to another view (e.g., employee details page)
            return RedirectToAction("UpdateEmployee", new { id = empId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(
        IFormFile employee_files,
        int employee_id,
        string name,
        string username,
        string password,
        bool isAdmin,
        int identity_number,
        int departement_id,
        string position,
        string permission_position,
        string contract_type,
        string national_address,
        string education_level,
        DateTime hire_date,
        DateTime leave_date,
        string email,
        string phone_number,
        string gender,
        bool active)
        {
            // Find the existing employee
            var employee = _context.employee
                .Include(e => e.EmployeeDetails)
                .ThenInclude(d => d.Department) // Include Department to access supervisor_id
                .FirstOrDefault(e => e.employee_id == employee_id);

            if (employee == null)
            {
                return Json(new { success = false, message = "موظف غير موجود." });
            }

            // Store the previous position to check if the employee was a manager
            var previousPosition = employee.EmployeeDetails.position;

            // Update employee properties
            employee.name = name;
            employee.username = username;
            employee.password = password;
            employee.search_role = isAdmin ? "admin" : null;


            // Update employee details
            var details = employee.EmployeeDetails;
            details.identity_number = identity_number;
            details.departement_id = departement_id;
            details.position = position;
            details.permission_position = permission_position;
            details.contract_type = contract_type;
            details.national_address = national_address;
            details.education_level = education_level;
            details.hire_date = hire_date;
            details.leave_date = leave_date;
            details.email = email;
            details.phone_number = phone_number;
            details.gender = gender;
            details.active = active;

            // Handle file upload
            if (employee_files != null && employee_files.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                var extension = Path.GetExtension(employee_files.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "الملف الذي تم تحميله غير صالح. يرجى تحميل ملف PDF أو Excel أو Word." });
                }

                string filename = Path.GetFileName(employee_files.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = Path.Combine(path, filename);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    await employee_files.CopyToAsync(filestream);
                }

                details.files = filename;
            }

            // Initialize the success message
            string successMessage = "تم تعديل موظف بنجاح!";
            var department = _context.Department.FirstOrDefault(d => d.departement_id == departement_id);
            if (position == "السكرتير" && department.departement_name != "الادارة التنفيذية")
            {
                return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
            }
            // Check if the position contains 'مدير'
            if (position.Contains("مدير"))
            {
                if (department != null)
                {
                    var oldSupervisorId = department.supervisor_id;

                    // Check if the position is "المدير التنفيذي" and the department name is not "الإدارة التنفيذية"



                    // Validate department name and position suffix
                    var positionSuffix = position.Replace("مدير", "").Trim();
                    if (department.departement_name != positionSuffix && department.departement_name != "الادارة التنفيذية")
                    {
                        return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
                    }

                    // Update the old supervisor if needed
                    if (oldSupervisorId != employee_id)
                    {
                        var oldSupervisor = _context.employee
                            .Include(e => e.EmployeeDetails)
                            .FirstOrDefault(e => e.employee_id == oldSupervisorId);

                        if (oldSupervisor != null)
                        {
                            oldSupervisor.EmployeeDetails.position = "موظف";
                            oldSupervisor.EmployeeDetails.permission_position = "موظف";
                        }

                        // Set the new supervisor
                        department.supervisor_id = employee_id;
                    }
                }
            }

            // Validate permission_position
            if (permission_position.Trim() != "المدير التنفيذي" && permission_position.Contains("مدير"))
            {
                if (department != null)
                {
                    var permissionPositionSuffix = permission_position.Replace("مدير", "").Trim();
                    if (department.departement_name != permissionPositionSuffix)
                    {
                        return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات الادارة الخاصة بهذا القسم لموظف خارج القسم" });
                    }
                }
            }


            if (permission_position.Trim() == "المدير التنفيذي")
            {
                if (department != null)
                {

                    if (department.departement_name != "الادارة التنفيذية")
                    {
                        return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات الادارة الخاصة بهذا القسم لموظف خارج القسم" });
                    }
                }
            }
            if (permission_position == "السكرتير")
            {
                if (department != null)
                {

                    if (department.departement_name != "الادارة التنفيذية")
                    {
                        return Json(new { success = false, message = "لا يمكن اعطاء صلاحيات السكرتير لموظف خارج قسم الادارة التنفيذية" });
                    }
                }
            }

            // Handle if the employee was a manager and is now set to 'مدير' or 'موظف'
            if (previousPosition.Contains("مدير") && previousPosition != position)
            {
                var oldDepartment = _context.Department.FirstOrDefault(d => d.supervisor_id == employee_id);
                if (oldDepartment != null)
                {
                    oldDepartment.supervisor_id = null;
                    successMessage += " القسم السابق الذي كان يديره هذا المدير الآن شاغر بلا مدير.";
                }
            }

            // Save changes to the database and return the JSON response with the final success message
            try
            {
                _context.SaveChanges();
                return Json(new { success = true, message = successMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "لم يتم تعديل الموظف.", error = ex.Message });
            }
        }






        [HttpGet]
        [Route("HR/GetRemainingHolidayBalance")]
        public IActionResult GetRemainingHolidayBalance(int holidayId)
        {
            var employeeId = GetEmployeeIdFromSession(); // Ensure this is returning the correct employee ID

            // Check if holidayId exists
            var holidayType = _context.Holidays
                .Where(h => h.holiday_id == holidayId)
                .Select(h => h.allowedDuration)
                .FirstOrDefault();

            if (holidayType == 0)
            {
                return Json("Holiday type not found");
            }

            // Check if any records exist
            var totalTakenDuration = _context.HolidayHistories
                 .Where(hh => hh.emp_id == employeeId
                              && hh.holiday_id == holidayId
                              && ((hh.start_date.Year == DateTime.Now.Year && hh.holiday.type != "استئذان")
                                  || (hh.start_date.Month == DateTime.Now.Month && hh.holiday.type == "استئذان"))
                              && hh.status == "موافقة المدير التنفيذي")
                 .Sum(hh => hh.duration);

            var remainingBalance = holidayType - totalTakenDuration;

            return Json(remainingBalance);
        }

        [HttpGet]
        [Route("HR/GetRemainingHolidayBalanceForEmployee")]
        public IActionResult GetRemainingHolidayBalanceForEmployee(int employeeNumber, int holidayId)
        {
            try
            {
                // Fetch the allowed duration for the specified holiday type
                var holidayType = _context.Holidays
                    .Where(h => h.holiday_id == holidayId)
                    .Select(h => h.allowedDuration)
                    .FirstOrDefault();

                // If the holiday type is not found, return an error
                if (holidayType == 0)
                {
                    return Json("Holiday type not found");
                }

                // Calculate the total taken duration based on the holiday type and employee
                var totalTakenDuration = _context.HolidayHistories
                    .Where(hh => hh.emp_id == employeeNumber
                                 && hh.holiday_id == holidayId
                                 && ((hh.start_date.Year == DateTime.Now.Year && hh.holiday.type != "استئذان")
                                     || (hh.start_date.Month == DateTime.Now.Month && hh.holiday.type == "استئذان"))
                                 && hh.status == "موافقة مدير الموارد البشرية")
                    .Sum(hh => hh.duration);

                // Calculate the remaining balance
                var remainingBalance = holidayType - totalTakenDuration;

                return Json(remainingBalance);
            }
            catch (Exception)
            {
                // Return a generic error message if an exception occurs
                return Json("Error fetching balance");
            }
        }




        [HttpPost]
        public IActionResult ArchiveHoliday(int id)
        {
            var holiday = _context.HolidayHistories.FirstOrDefault(h => h.holidays_history_id == id);
            if (holiday == null)
            {
                return Json(new { success = false, message = "Holiday not found." });
            }

            // Update the status to "مؤرشفة"
            holiday.status = "مؤرشفة";

            // Save the changes to the database
            _context.SaveChanges();

            return Json(new { success = true });
        }

        /*[HttpGet]
		public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
		{
			_logger.LogInformation($"Fetching employees for department ID: {departmentId}");

			var employees = await _context.employee_details
				.Where(ed => ed.departement_id == departmentId)
				.Select(ed => new
				{
					employee_id = ed.employee_id,
					name = ed.employee.name,
					position = ed.position
				})
				.GroupBy(e => e.employee_id)
				.Select(g => g.First())
				.ToListAsync();

			if (!employees.Any())
			{
				_logger.LogWarning($"No employees found for department ID: {departmentId}");
				return NotFound("No employees found for the given department.");
			}

			_logger.LogInformation($"Found {employees.Count} employees for department ID: {departmentId}");
			return Ok(employees);
		}*/


        //////////////   Salaries ////////////////////////
        ///
        public ActionResult _GetAllSalaries()
        {
            var salaries_history = _context.SalaryHistories
                .GroupBy(sr => new { sr.date.Year, sr.date.Month })
                .Select(g => new
                {
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
                .Select(g => new
                {
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
                return RedirectToAction("_GetAllSalaries");
            }

            int parsedYear;
            if (!int.TryParse(year, out parsedYear))
            {
                return RedirectToAction("_GetAllSalaries");
            }

            var filteredSalaries = _context.SalaryHistories
                .Where(sr => sr.date.Year == parsedYear)
                .GroupBy(sr => new { sr.date.Year, sr.date.Month })
                .Select(g => new
                {
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
                .Select(g => new
                {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                    Count = g.Count,
                    Total = g.Total
                })
                .ToList<dynamic>();

            return View("_GetAllSalaries", filteredSalaries);
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


        // GET: salaries_history/Create
        public IActionResult CreateRecord()
        {
            ViewBag.Employees = new SelectList(_context.employee, "employee_id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecord(
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
                return RedirectToAction(nameof(_GetAllSalaries)); // Adjust the redirect as needed
            }

            ViewBag.Employees = new SelectList(_context.employee, "employee_id", "name", emp_id);
            return View();
        }

        // GET: salaries_history/Edit/5
        public async Task<IActionResult> Edit_Salaries(int? id)
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
        public async Task<IActionResult> Edit_Salaries(
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
                return RedirectToAction(nameof(_GetAllSalaries));
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

        [HttpPost, ActionName("Delete_Salaries")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete_Confirmed_Salaries(int id)
        {
            var salaryRecord = await _context.SalaryHistories.FindAsync(id);
            _context.SalaryHistories.Remove(salaryRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(_GetAllSalaries));
        }

        public async Task<IActionResult> UpdateStatus(int transaction_id, string TerminationCause)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            transaction.status = "منهاة";
            transaction.close_date = DateTime.Now;
            transaction.TerminationCause = TerminationCause;
            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }

    }







}
