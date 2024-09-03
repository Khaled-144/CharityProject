using CharityProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;

using System;

namespace CharityProject.Controllers
{
    public class CustomerServiceManagerController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public CustomerServiceManagerController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Start of khaled work -----------------------------------------------------
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
                    .Include(ed => ed.employee) // To include related employee data
                    .FirstOrDefaultAsync(ed => ed.employee_id == employeeId);

                return employeeDetails;
            }

            return null;
        }

        public IActionResult ReferTransaction()
        {
            return RedirectToAction("Transactions", "CustomerServiceManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReferTransaction(int transaction_id, int to_employee_id, string comments)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
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
            };

            _context.Referrals.Add(referral);
            transaction.to_emp_id = to_employee_id;
            await _context.SaveChangesAsync();

			// Redirect to the Transactions page after successful referral
			return RedirectToAction("Transactions", "CustomerServiceManager");
		}

        // New method to view referral history
        public async Task<IActionResult> ReferralHistory(int id)
        {
            var referrals = await _context.Referrals
                .Where(r => r.transaction_id == id)
                .OrderByDescending(r => r.referral_date)
                .ToListAsync();

            return View(referrals);
        }

        // End of khaled work -----------------------------------------------------





        public async Task<IActionResult> Index()
        {
            int currentUserId = GetEmployeeIdFromSession();

            // Count transactions based on their status, ensuring no duplicates
            var newTransactions = await _context.Transactions
                .Where(t => t.status == "مرسلة" && (t.to_emp_id == currentUserId || t.Referrals.Any(r => r.to_employee_id == currentUserId)))
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            var ongoingTransactions = await _context.Transactions
                .Where(t => t.status != "منهاة" && (t.to_emp_id == currentUserId || t.Referrals.Any(r => r.to_employee_id == currentUserId)))
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            var completedTransactions = await _context.Transactions
                .Where(t => t.status == "منهاة" && (t.to_emp_id == currentUserId || t.Referrals.Any(r => r.to_employee_id == currentUserId)))
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.NewTransactionsCount = newTransactions;
            ViewBag.OngoingTransactionsCount = ongoingTransactions;
            ViewBag.CompletedTransactionsCount = completedTransactions;

            return View();
        }

        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

        public async Task<IActionResult> Transactions()
        {
            // Retrieve the current user's ID from the session or context
            int currentUserId = GetEmployeeIdFromSession();

            // Populate the Departments dropdown list
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            // Filter transactions based on the current user's ID and include the related Department
            var transactions = await _context.Transactions
                .Where(t => t.to_emp_id == currentUserId)
                .Include(t => t.Department) // Include the Department
                .ToListAsync();

            // Get the counts for various entities
            int internalCount = await _context.Transactions
                .Where(t => t.to_emp_id == currentUserId)
                .CountAsync();

            int holidaysCount = await _context.HolidayHistories
                .Where(h => h.emp_id == currentUserId)
                .CountAsync();

            int lettersCount = await _context.letters
                .Where(l => l.to_emp_id == currentUserId || l.to_emp_id == currentUserId)
                .CountAsync();

            int assetsCount = await _context.charter
                .Where(c => c.to_emp_id == currentUserId)
                .CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.InternalCount = internalCount;
            ViewBag.HolidaysCount = holidaysCount;
            ViewBag.LettersCount = lettersCount;
            ViewBag.AssetsCount = assetsCount;

            return View(transactions);
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var employeeId = GetEmployeeIdFromSession();

            // Fetch transactions based on the conditions provided
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.status == "مرسلة" && (t.from_emp_id == employeeId || // Transactions sent by the employee
                    t.to_emp_id == employeeId ||
                     t.department_id == 5 || // Transactions sent to the employee

                    t.Referrals.Any(r => r.to_employee_id == employeeId))
                 // Transactions for the manager's department or from department 5
                 )  // Transactions referred to the employee
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

            return PartialView("_getAllTransactions", transactions);
        }
        public async Task<IActionResult> GetAllHolidays()
        {
            var employeeId = GetEmployeeIdFromSession();

            // Fetch holidays with the status "مرسلة" where the employee's department ID is 5
            var holidays = await _context.HolidayHistories
                .Where(h => h.status == "مرسلة" && h.Employee_detail.departement_id == 5)
                .OrderByDescending(h => h.holidays_history_id)
                .ToListAsync();
            var employeeIds = holidays.SelectMany(t => new[] { t.emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLetters()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            if (employeeDetails == null)
            {
                return NotFound(); // Handle the case where employee details are not found
            }

            var letters = await _context.letters
                .Where(l => l.to_emp_id == employeeDetails.employee_id || l.departement_id == employeeDetails.departement_id)
                .OrderByDescending(l => l.letters_id)
                .ToListAsync();

            return PartialView("_getAllLetters", letters);
        }

        // Create Actions  --------------------------------------------------------


        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Transaction(IFormFile files, [Bind("create_date,close_date,title,description,to_emp_id,department_id")] Transaction transaction)
        {
            // Retrieve the employee ID from session
            var employeeId = GetEmployeeIdFromSession();

            transaction.from_emp_id = employeeId;

            if (files != null && files.Length > 0)
            {
                // Validate the file type
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                var extension = Path.GetExtension(files.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                    return View(transaction); // Return the view with validation error
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
                transaction.files = filename;
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
            holidayHistory.status = "مرسلة"; // Set default status
            _context.Add(holidayHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Letter(IFormFile files, [Bind("title,description,type,to_emp_id")] letter letter)
        {
            var employeeId = GetEmployeeIdFromSession();
            letter.from_emp_id = employeeId; // Assign the employee ID to the letter

            if (files != null && files.Length > 0)
            {
                // Validate the file type
                var allowedExtensions = new[] { ".pdf", ".xls", ".xlsx", ".doc", ".docx" };
                var extension = Path.GetExtension(files.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("files", "Only PDF, Excel, and Word files are allowed.");
                    // Return the view with validation error
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


            letter.date = DateTime.Now; // Set the current date

            // Optionally set department_id dynamically based on the user's department
            // letter.departement_id = /* retrieve from employee details */;

            _context.Add(letter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions)); // Or any other relevant action


            // Return the same view with validation errors
        }
        public IActionResult GetRemainingHolidayBalance(int employeeId, int holidayId)
        {
            // Get the allowed duration for the specified holiday type
            var holidayType = _context.Holidays
                .Where(h => h.holiday_id == holidayId)
                .Select(h => h.allowedDuration)
                .FirstOrDefault();

            // Calculate the total duration taken for the specified holiday type
            var totalTakenDuration = _context.HolidayHistories
                .Where(hh => hh.emp_id == employeeId
                             && hh.holiday_id == holidayId
                             && hh.start_date.Year == DateTime.Now.Year)
                .Sum(hh => hh.duration);

            // Calculate the remaining balance
            var remainingBalance = holidayType - totalTakenDuration;

            return Json(remainingBalance);
        }



        // Update Actions --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionStatus(int transaction_id)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            transaction.status = "منهاة";
            transaction.close_date = DateTime.Now;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHolidayStatus(int holidays_history_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holidays_history_id);
            if (holiday == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            holiday.status = "رفضت من المدير المباشر";


            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
        }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveTransaction(int transaction_id)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }


            // Update the status to "Closed"
            transaction.status = "موافقة المدير المباشر";
            transaction.close_date = DateTime.Now;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartmentName(int departmentId)
        {
            var department = await _context.Department.FindAsync(departmentId);
            if (department != null)
            {
                return Content(department.departement_name);
            }
            return Ok();
        }

        
        public async Task<IActionResult> ApproveHoliday(int holidays_history_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holidays_history_id);
            if (holiday == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            holiday.status = "موافقة المدير المباشر";
        

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Transactions));
        }





        // Delete Actions --------------------------------------------------------



    }
}
