using CharityProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Extensions.Logging;
namespace CharityProject.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

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


        [HttpGet]
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

        public async Task<IActionResult> Archive() { return View(); }

        //-----------------------------------------------------------------------------{ Transactions Actions }-----------------------------------------------
        public async Task<IActionResult> Transactions()
        {
            // Retrieve the current user's ID from the session or context
            int currentUserId = GetEmployeeIdFromSession();

            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            // Filter transactions based on the current user's ID
            var transactions = await _context.Transactions
                .Where(t => t.to_emp_id == currentUserId)
                .ToListAsync();

            int internalCount = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.status != "منهاة" &&
                    (t.to_emp_id == currentUserId ||
                     t.Referrals.Any(r => r.to_employee_id == currentUserId && r.from_employee_id == currentUserId))
                )
                .CountAsync();

            int holidaysCount = await _context.HolidayHistories
                .Where(h => h.emp_id == currentUserId)
                .CountAsync();
            int externalCount = await _context.ExternalTransactions
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
            ViewBag.ExternalCount = externalCount;
            ViewBag.AssetsCount = assetsCount;

            return View(transactions);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Transaction(IFormFile files, [Bind("create_date,close_date,title,description,to_emp_id,department_id,Confidentiality,Urgency,Importance")] Transaction transaction)
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

        [HttpGet]
        [Route("Employees/GetNextReceivingNumber")]

        public JsonResult GetNextReceivingNumber()
        {
            var lastTransaction = _context.ExternalTransactions.OrderByDescending(t => t.external_transactions_id).FirstOrDefault();
            int nextReceivingNumber = (lastTransaction != null) ? lastTransaction.external_transactions_id + 1 : 1;

            return Json(nextReceivingNumber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExternalTransaction([Bind("name,identity_number,status,communication,case_status,sending_party,receiving_date,sending_number,sending_date,receiving_number")] ExternalTransaction transaction)
        {

            // Add the transaction to the context
            _context.Add(transaction);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or another action
            return RedirectToAction(nameof(Transactions)); // Change "Index" to the appropriate action or page
        }

        [HttpGet]
        // Action method to retrieve all external transactions
        public async Task<IActionResult> GetAllExternalTransactions()
        {
            // Retrieve all external transactions from the database
            var transactions = await _context.ExternalTransactions.ToListAsync();
            if (transactions.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }
            // Return the view with the list of transactions
            return PartialView("_getAllExternalTransactios", transactions);
        }



        [HttpPost]
        public IActionResult UpdateExternalTransaction(ExternalTransaction model)
        {
            var transaction = _context.ExternalTransactions
                .FirstOrDefault(t => t.external_transactions_id == model.external_transactions_id);

            if (transaction != null)
            {
                transaction.name = model.name;
                transaction.identity_number = model.identity_number;
                transaction.status = model.status;
                transaction.communication = model.communication;
                transaction.case_status = model.case_status;
                transaction.sending_party = model.sending_party;
                transaction.receiving_date = model.receiving_date;
                transaction.sending_date = model.sending_date;
                transaction.receiving_number = model.receiving_number;

                // Save changes to the database
                _context.SaveChanges();

                return RedirectToAction(nameof(Transactions));
            }
            else
            {
                return RedirectToAction(nameof(Transactions));
            }
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

        public async Task<IActionResult> GetAllTransactions()
        {
            var employeeId = GetEmployeeIdFromSession();

            // Fetch transactions sent directly to the employee or referred to the employee
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.status != "منهاة" && (
                    (t.status == "مرسلة" && t.to_emp_id == employeeId) || t.Referrals.Any() && // Ensure there are referrals
                    t.Referrals
                        .OrderByDescending(r => r.referral_date)
                        .First().to_employee_id == employeeId &&
                    t.Referrals
                        .OrderByDescending(r => r.referral_date)
                        .First().from_employee_id != employeeId // Exclude transactions referred back to the same employee
                ))
                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();

            // Check if there are no transactions
            if (transactions.Count == 0)
            {
                // Render the _NothingNew partial view if no transactions
                return PartialView("_NothingNew");
            }

            // Fetch employee names
            var employeeIds = transactions.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");

            // Render the _getAllTransactions partial view if there are transactions
            return PartialView("_getAllTransactions", transactions);

        }

        public async Task<IActionResult> GetArchivedTransactions()
        {
            var employeeId = GetEmployeeIdFromSession();

            // Fetch transactions sent directly to the employee or referred to the employee
            // getting the transactions sent to me or referred to me 
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.to_emp_id == employeeId || t.from_emp_id == employeeId || t.Referrals.Any(r => r.to_employee_id == employeeId))
                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();

            // Fetch employee names
            var employeeIds = transactions.SelectMany(t => new[] { t.from_emp_id, t.to_emp_id }).Distinct().ToList();
            var employees = await _context.employee
                .Where(e => employeeIds.Contains(e.employee_id))
                .ToDictionaryAsync(e => e.employee_id, e => e.name);

            ViewBag.EmployeeNames = employees;

            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");

            return PartialView("_getAllArchivedTransactions", transactions);
        }


        public IActionResult ReferTransaction()
        {
            return RedirectToAction("Transactions", "Employees");
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
            return RedirectToAction("Transactions", "Employees");
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionsStatus(int transaction_id)
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

        //-----------------------------------------------------------------------------{ END Transactions Actions }-----------------------------------------------

        //--

        //-----------------------------------------------------------------------------{  Holidays Actions }------------------------------------------------------




        public async Task<IActionResult> GetArchivedHolidays()
        {
            var employeeId = GetEmployeeIdFromSession();

            // Fetch holidays created by the logged-in employee
            var holidays = await _context.HolidayHistories
                .Where(h => h.emp_id == employeeId)
                .OrderByDescending(h => h.holidays_history_id)
                .ToListAsync();

            return PartialView("_getAllArchivedHolidays", holidays);
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





        [HttpGet]
        [Route("Employees/GetRemainingHolidayBalance")]
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

        //-----------------------------------------------------------------------------{ END Holidays Actions }-----------------------------------------------

        //--

        //-----------------------------------------------------------------------------{  Letters Actions }------------------------------------------------------

        public async Task<IActionResult> GetAllLetters()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var letters = await _context.letters
                .Where(l => l.to_emp_id == employeeDetails.employee_id || l.departement_id == employeeDetails.departement_id)
                .OrderByDescending(l => l.letters_id)
                .ToListAsync();

            // Check if there are no letters
            if (letters.Count == 0)
            {
                // Render the _NothingNew partial view if no letters
                return PartialView("_NothingNew");
            }

            // Render the _getAllLetters partial view if there are letters
            return PartialView("_getAllLetters", letters);
        }



        public async Task<IActionResult> GetArchivedLetters()
        {
            var employeeDetails = await GetEmployeeDetailsFromSessionAsync();
            var letters = await _context.letters
                .Where(l => l.from_emp_id == employeeDetails.employee_details_id)
                .OrderByDescending(l => l.letters_id)
                .ToListAsync();

            return PartialView("_getAllArchivedLetters", letters);
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



        [HttpGet]
        public async Task<IActionResult> SearchAssets(string searchTerm = "", string sortOrder = "")
        {
            _logger.LogInformation($"SearchAssets called with searchTerm: {searchTerm}, sortOrder: {sortOrder}");

            var employeeId = GetEmployeeIdFromSession();
            _logger.LogInformation($"Employee ID from session: {employeeId}");

            var query = _context.charter
                .Where(a => a.to_emp_id == employeeId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    a.charter_id.ToString().Contains(searchTerm) ||
                    a.charter_info.Contains(searchTerm)
                );
            }

            switch (sortOrder)
            {
                case "oldest":
                    query = query.OrderBy(a => a.receive_date);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(a => a.receive_date);
                    break;
            }

            var assets = await query.ToListAsync();
            _logger.LogInformation($"Found {assets.Count} assets");

            return PartialView("_getAllAssets", assets);
        }

        [HttpGet]
        public async Task<IActionResult> SearchTransactions(string searchTerm = "", string sortOrder = "")
        {
            _logger.LogInformation($"SearchTransactions called with searchTerm: {searchTerm}, sortOrder: {sortOrder}");

            var employeeId = GetEmployeeIdFromSession();
            _logger.LogInformation($"Employee ID from session: {employeeId}");

            var query = _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.to_emp_id == employeeId || t.Referrals.Any(r => r.to_employee_id == employeeId));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t =>
                    t.transaction_id.ToString().Contains(searchTerm) ||
                    t.title.Contains(searchTerm)
                );
            }

            switch (sortOrder)
            {
                case "oldest":
                    query = query.OrderBy(t => t.create_date);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(t => t.create_date);
                    break;
            }

            var transactions = await query.ToListAsync();
            _logger.LogInformation($"Found {transactions.Count} transactions");

            // Fetch employee names
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

        [HttpGet]
        public async Task<IActionResult> SearchHolidays(string searchTerm = "", string sortOrder = "")
        {
            _logger.LogInformation($"SearchHolidays called with searchTerm: {searchTerm}, sortOrder: {sortOrder}");

            var employeeId = GetEmployeeIdFromSession();
            _logger.LogInformation($"Employee ID from session: {employeeId}");

            var query = _context.HolidayHistories
                .Where(h => h.emp_id == employeeId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h =>
                    h.holidays_history_id.ToString().Contains(searchTerm) ||
                    h.title.Contains(searchTerm)
                );
            }

            switch (sortOrder)
            {
                case "oldest":
                    query = query.OrderBy(h => h.start_date);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(h => h.start_date);
                    break;
            }

            var holidays = await query.ToListAsync();
            _logger.LogInformation($"Found {holidays.Count} holidays");

            return PartialView("_getAllHolidays", holidays);
        }
        [HttpGet]
        public async Task<IActionResult> SearchLetters(string searchTerm = "", string sortOrder = "")
        {
            _logger.LogInformation($"SearchLetters called with searchTerm: {searchTerm}, sortOrder: {sortOrder}");

            var employeeId = GetEmployeeIdFromSession();
            _logger.LogInformation($"Employee ID from session: {employeeId}");

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
            _logger.LogInformation($"Found {letters.Count} letters");

            return PartialView("_getAllLetters", letters);
        }
        [HttpGet]
        public IActionResult SearchExternalTransactions(string searchTerm)
        {
            var transactions = _context.ExternalTransactions.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                transactions = transactions.Where(t => t.identity_number.ToString().Contains(searchTerm) || t.sending_number.ToString().Contains(searchTerm));
            }

            var resultList = transactions.ToList();

            if (!resultList.Any())
            {
                return PartialView("_NoResults"); // Return the _NoResults partial view if no results are found
            }

            return PartialView("_getAllExternalTransactios", resultList); // Return the transaction list partial view if results are found
        }



        //-----------------------------------------------------------------------------{  Charter Actions }------------------------------------------------------


        //public async Task<IActionResult> GetAllCharters()
        //{
        //    var employe_details = await GetEmployeeDetailsFromSessionAsync();

        //    var charter = await _context.charter
        //        .Include(c => c.employee)
        //        .Where(c => c.status == "غير مسلمة" && c.to_emp_id == employe_details.employee_id)
        //        .OrderByDescending(t => t.charter_id) // Order by transaction_id in descending order
        //        .ToListAsync();
        //    if (charter.Count == 0)
        //    {
        //        // Render the _NothingNew partial view if no transactions
        //        return PartialView("_NothingNew");
        //    }
        //    return PartialView("_GetAllCharters", charter);
        //}


    }
}
