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
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }



        // Start of khaled work -----------------------------------------------------

        public IActionResult ReferTransaction()
		{
			return RedirectToAction("Transactions", "Employees");
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
        {
            var referrals = await _context.Referrals
                .Where(r => r.transaction_id == id)
                .OrderByDescending(r => r.referral_date)
                .ToListAsync();

            _logger.LogInformation($"Found {referrals.Count} referrals");

            foreach (var referral in referrals)
            {
                _logger.LogInformation($"Referral {referral.referral_id}: From {referral.from_employee_id} ({referral.from_employee?.name ?? "N/A"}) To {referral.to_employee_id} ({referral.to_employee?.name ?? "N/A"})");
            }

            return View(referrals);
        }
            return View(referrals);
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
        // End of khaled work -----------------------------------------------------





        public IActionResult Index()
        {
            return View();
        }

        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

		public async Task<IActionResult> Transactions()
		{
            
            return View(await _context.Transactions.ToListAsync());
		}
        public async Task<IActionResult> Transactions()
        {
            ViewData["Departments"] = _context.Department.Select(d => new SelectListItem
            {
                Value = d.departement_id.ToString(),
                Text = d.departement_name
            }).ToList();

            var transactions = await _context.Transactions.ToListAsync();

            int internalCount = transactions.Count();
            int holidaysCount = await _context.HolidayHistories.CountAsync();
            int lettersCount = await _context.Letters.CountAsync();
            int assetsCount = await _context.charter.CountAsync();

            // Passing the counts to the view using ViewBag
            ViewBag.InternalCount = internalCount;
            ViewBag.HolidaysCount = holidaysCount;
            ViewBag.LettersCount = lettersCount;
            ViewBag.AssetsCount = assetsCount;

            return View(transactions);
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();
            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");

            return PartialView("_getAllTransactions", transactions);
        }
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                .OrderByDescending(t => t.transaction_id) // Order by transaction_id in descending order
                .ToListAsync();
            return PartialView("_getAllTransactions", transactions);
        }

        public async Task<IActionResult> GetAllHolidays()
		{
			var holidays = await _context.HolidayHistories
				.OrderByDescending(h => h.holidays_history_id) // Replace HolidaysHistoryId with the actual ID column name
				.ToListAsync();
			return PartialView("_getAllHolidays", holidays);
		}
        public async Task<IActionResult> GetAllHolidays()
        {
            var holidays = await _context.HolidayHistories
                .OrderByDescending(h => h.holidays_history_id) // Replace HolidaysHistoryId with the actual ID column name
                .ToListAsync();
            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLetters()
        {
            var letters = await _context.Letters
                .OrderByDescending(l => l.letters_id) // Order by letters_id in descending order
                .ToListAsync();
            return PartialView("_getAllLetters", letters);
        }

        // Create Actions  --------------------------------------------------------


        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create_Transaction(IFormFile files, [Bind("create_date,close_date,title,description,from_emp_id,to_emp_id,department_id")] Transaction transaction)
        {
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
        public async Task<IActionResult> Create_Holiday([Bind("title,description,duration,emp_id,start_date,end_date,files,holiday_id")] HolidayHistory holidayHistory)
        {
            holidayHistory.creation_date = DateOnly.FromDateTime(DateTime.Now); // Set to current date
            holidayHistory.status = "مرسلة"; // Set default status
            _context.Add(holidayHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Letter([Bind("title,description,type,from_emp_id,to_emp_id,files")] Letter letter)
        {
            if (ModelState.IsValid)
            {
                letter.date = DateTime.Now; // Set the current date
                letter.departement_id = 3;
                _context.Add(letter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(letter);
        }


        // Update Actions --------------------------------------------------------
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


        // Delete Actions --------------------------------------------------------



    }
}
