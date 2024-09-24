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
using Microsoft.CodeAnalysis.CSharp;

namespace CharityProject.Controllers
{
    public class CEO : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public CEO(ApplicationDbContext context)
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
                    .Include(ed => ed.employee)
                   .Include(ed => ed.Department)
                    .FirstOrDefaultAsync(ed => ed.employee_id == employeeId);

                return employeeDetails;
            }

            return null;
        }

        public IActionResult ReferTransaction()
        {
            return RedirectToAction("Transactions", "CEO");
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
            var referrals = await _context.Referrals
                .Where(r => r.transaction_id == id)
                .OrderByDescending(r => r.referral_date)
                .ToListAsync();

            return View(referrals);
        }

        // End of khaled work -----------------------------------------------------





        public async Task<IActionResult> Index()
        {
            var employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var hrManager = _context.employee_details
        .FirstOrDefault(e => e.position == "المدير التنفيذي");

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
                .Where(t => t.status == "منهاة" &&t.to_emp_id == employeeId)
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
        public async Task<IActionResult> Archive()
        {
            return View();
        }
            public async Task<IActionResult> Transactions()
        {
            // Retrieve the current user's ID from the session or context
            int employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var Manager = _context.employee_details
       .FirstOrDefault(e => e.position == "مدير التنمية المالية والاستدامة");
            // Populate the Departments dropdown list
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            // Get the counts for various entities
            int internalCount =
                         await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
     (t.status == "مرسلة" && t.Employee_detail.departement_id == employeeDetails.departement_id && t.Employee_detail.employee_id != employeeDetails.employee_id && t.Employee_detail.permission_position == "موظف") ||
   
    (t.Referrals.Any() && // Ensure there are referrals
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == Manager.employee_id
        ) &&
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == Manager.employee_id
        )
    )
).CountAsync();

            int holidaysCount = await _context.HolidayHistories
                .Where(h => h.status  == "مرسلة من مدير" || (h.status== "مرسلة" && h.Employee_detail.departement_id == employeeDetails.departement_id))
                .CountAsync();

            int lettersCount = await _context.letters
                   .Where(l => l.to_emp_id == employeeDetails.employee_id || (l.to_departement_name == employeeDetails.Department.departement_name && l.to_emp_id == 0)||l.type=="تظلم")
                 .CountAsync();

            int assetsCount = await _context.charter
               .Where(c => c.status == "غير مسلمة" && c.to_emp_id == employeeDetails.employee_id)
                .CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.InternalCount = internalCount;
            ViewBag.HolidaysCount = holidaysCount;
            ViewBag.LettersCount = lettersCount;
            ViewBag.AssetsCount = assetsCount;

            return View();
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var employeeId = GetEmployeeIdFromSession();
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var Manager = _context.employee_details
        .FirstOrDefault(e => e.position == "المدير التنفيذي");

            // Fetch transactions based on the conditions provided
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
     (t.status == "مرسلة" && t.Employee_detail.departement_id == employeeDetails.departement_id && t.Employee_detail.employee_id != employeeDetails.employee_id && t.Employee_detail.permission_position == "موظف") ||
    (t.status == "مرسلة" && (t.to_emp_id == employeeId || t.to_emp_id == Manager.employee_id) && t.Employee_detail.permission_position != "موظف")
    || (t.status == "مرسلة" && t.department_id == employeeDetails.departement_id && t.Employee_detail.permission_position != "موظف" && t.Employee_detail.employee_id != employeeDetails.employee_id) ||// Transactions sent to the employee
    (t.Referrals.Any() && // Ensure there are referrals
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == Manager.employee_id
        ) &&
        (
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == employeeId ||
            t.Referrals.OrderByDescending(r => r.referral_date).First().to_employee_id == Manager.employee_id
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
        public async Task<IActionResult> GetAllTransactionsArchived()
        {
          
           

            // Fetch transactions based on the conditions provided
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t =>
                (t.status == "منهاة"))
   
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
        public async Task<IActionResult> GetAllHolidays()
        {
            var employee = await GetEmployeeDetailsFromSessionAsync();

            // Fetch holidays with the status "مرسلة" where the employee's department ID is 5
            var holidays = await _context.HolidayHistories
                .Include(h=>h.holiday)
                .Where(h=> h.status == "مرسلة من مدير" )
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
        public async Task<IActionResult> GetAllHolidaysArchived()
        {
            var employee = await GetEmployeeDetailsFromSessionAsync();

            // Fetch holidays with the status "مرسلة" where the employee's department ID is 5
            var holidays = await _context.HolidayHistories
                 .Include(h => h.holiday)
                .Where(h => h.status == "موافقة مدير الموارد البشرية" || h.status == "رفضت من مدير الموارد البشرية"|| h.status == "موافقة المدير التنفيذي"|| h.status == "رفضت من المدير التنفيذي")
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
               
                  .Where(l => l.to_emp_id == employeeDetails.employee_id || (l.to_departement_name == employeeDetails.Department.departement_name && l.to_emp_id == 0)||l.type == "تظلم")
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
        public async Task<IActionResult> GetAllLettersArchived()
        {
          
            var letters = await _context.letters
                .OrderByDescending(l => l.letters_id) // Order by letters_id in descending order
                .ToListAsync();

            if (letters.Count == 0)
            {
                // Render the _NothingNew partial view if no letters
                return PartialView("_NothingNew");
            }

        

            return PartialView("_getAllLetters", letters);
        }
        public async Task<IActionResult> GetAllCharters()
        {
            var employe_details = await GetEmployeeDetailsFromSessionAsync();

            var charter = await _context.charter
                .Include(c => c.employee)
                .Where(c => c.status == "مستلمة" && c.to_emp_id == employe_details.employee_id)
                .OrderByDescending(t => t.charter_id) // Order by transaction_id in descending order
                .ToListAsync();
            if (charter.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            return PartialView("_GetAllCharters", charter);
        }

        public async Task<IActionResult> GetAllChartersArchived()
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
        /* [HttpGet]
         public async Task<IActionResult> SearchLetters(string searchTerm = "", string sortOrder = "")
         {

             var employeeId = GetEmployeeIdFromSession();

             var query = _context.letters
                 .Where(l => l.to_emp_id == employeeId);

             if (!string.IsNullOrEmpty(searchTerm))
             {
                 query = query.Where(l =>
                     l.letters_id.ToString().Contains(searchTerm) ||
                     l.title.Contains(searchTerm)
                 );
             }

             switch (sortOrder)
             {
                 case "oldest":
                     query = query.OrderBy(l => l.date);
                     break;
                 case "newest":
                 default:
                     query = query.OrderByDescending(l => l.date);
                     break;
             }

             var letters = await query.ToListAsync();

             return PartialView("_getAllLetters", letters);
         }*/


        [HttpGet]
        public async Task<IActionResult> GetEmployeesByDepartmentName([FromQuery] int[] departmentNames)
        {
            _logger.LogInformation($"Fetching employees for department names: {string.Join(", ", departmentNames)}");

            // Find department IDs by names
            var departmentIds = await _context.Department
                .Where(d => departmentNames.Contains(d.departement_id))
                .Select(d => d.departement_id)
                .ToListAsync();

            if (!departmentIds.Any())
            {
                _logger.LogWarning($"No departments found with names: {string.Join(", ", departmentNames)}");
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
                _logger.LogWarning($"No employees found for department names: {string.Join(", ", departmentNames)}");
                return NotFound("No employees found for the given departments.");
            }

            _logger.LogInformation($"Found {employees.Count} employees for department names: {string.Join(", ", departmentNames)}");
            return Ok(employees);
        }

        // Create Actions  --------------------------------------------------------


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
            holidayHistory.status = "مرسلة"; // Set default status
            _context.Add(holidayHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }


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
            if (to_emp_id != null && to_emp_id.Any())
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
            if ((to_emp_id == null || !to_emp_id.Any()) && to_departement_name != null && to_departement_name.Any())
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
            if (letter.type == "تظلم" && (to_departement_name == null || !to_departement_name.Any()) && (to_emp_id == null || !to_emp_id.Any()))
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
                    to_emp_id = 0,
                    departement_id = letter.departement_id
                };

                _context.Add(newLetter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }

        [HttpGet]
        [Route("CEO/GetRemainingHolidayBalance")]
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
                             && hh.start_date.Year == DateTime.Now.Year)
                .Sum(hh => hh.duration);

            var remainingBalance = holidayType - totalTakenDuration;

            return Json(remainingBalance);
        }



        // Update Actions --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionStatus(int transaction_id,string TerminationCause)
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

        public async Task<IActionResult> GetEmployeesByDepartment(int departmentId)
        {
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
                return Ok(new { message = "لا يوجد موظفين في هذا القسم" });
            }

            return Ok(employees);
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


        public async Task<IActionResult> ApproveHoliday(int holiday_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holiday_id);
            if (holiday == null)
            {
                return Json(new { success = false, message = "طلب الإجازة غير موجود" });
            }

            // Update the status to "Closed"
            holiday.status = "موافقة المدير التنفيذي";


            // Save changes to the database
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تمت الموافقة على الإجازة" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyHoliday(int holiday_id)
        {
            var holiday = await _context.HolidayHistories.FindAsync(holiday_id);
            if (holiday == null)
            {
                return Json(new { success = false, message = "طلب الإجازة غير موجود" });
            }

            // Update the status to "Closed"
            holiday.status = "رفضت من المدير التنفيذي";


            // Save changes to the database
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "تم رفض الإجازة" });
        }


        // mange permisions --------------------------------------------------------

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

    }
}
