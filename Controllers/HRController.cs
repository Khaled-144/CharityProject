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
namespace CharityProject.Controllers
{
	public class HRController : Controller



	{
		private readonly ILogger<EmployeesController> _logger;
		private readonly ApplicationDbContext _context;

		public HRController(ApplicationDbContext context, ILogger<EmployeesController> logger)
		{
			_context = context;
			_logger = logger;
		}

        public IActionResult Index()
        {
            return View();
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
                    .Include(ed => ed.employee) // To include related employee data
                    .FirstOrDefaultAsync(ed => ed.employee_id == employeeId);

                return employeeDetails;
            }

            return null;
        }





        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

        public async Task<IActionResult> Transactions()
        {
			int currentUserId = GetEmployeeIdFromSession();
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
			// Filter transactions based on the current user's ID
			var transactions = await _context.Transactions
				.Where(t => t.to_emp_id == currentUserId)
				.ToListAsync();

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

            // Fetch transactions sent directly to the employee or referred to the employee
            // getting the transactions sent to me or referred to me 
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.from_employee)
                .Include(t => t.Referrals)
                    .ThenInclude(r => r.to_employee)
                .Where(t => t.to_emp_id == employeeId || t.Referrals.Any(r => r.to_employee_id == employeeId) || t.department_id == 1)
                .OrderByDescending(t => t.transaction_id)
                .ToListAsync();

            // Fetch departments for the dropdown
            var departments = await _context.Department.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "departement_id", "departement_name");

            return PartialView("_getAllTransactions", transactions);



        }

        public async Task<IActionResult> GetAllCharters()
        {




            var charter = await _context.charter

                .OrderByDescending(t => t.charter_id) // Order by transaction_id in descending order
                .ToListAsync();
            return PartialView("_GetAllCharters", charter);
        }


        public async Task<IActionResult> GetAllHolidays()
        {
            var holidays = await _context.HolidayHistories
                .Where(h=>h.status=="موافقة المدير المباشر")
                .OrderByDescending(h => h.holidays_history_id ) // Replace HolidaysHistoryId with the actual ID column name
                .ToListAsync();
            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLetters()
        {
            var letters = await _context.letters
                .OrderByDescending(l => l.letters_id) // Order by letters_id in descending order
                .ToListAsync();
            return PartialView("_getAllLetters", letters);
        }

        // Create Actions  --------------------------------------------------------


  



        public void Create_Charter()
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

        // POST: Charters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Charter([Bind("charter_id,charter_info,serial_number,creation_date,from_departement_name,status,notes,to_departement_name,to_emp_id,receive_date,end_date")] charter charter)
        {



            _context.Add(charter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions));


        }
		// POST: Transactions/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create_Transaction(IFormFile files, [Bind("create_date,close_date,title,description,from_emp_id,to_emp_id,department_id,Confidentiality,Urgency,Importance")] Transaction transaction)
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
				string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/files");
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

            List<int> employeeIdsToSendTo = new List<int>();

            // If departments are selected and no specific employees are chosen
            if (to_departement_name != null && to_departement_name.Any() && (to_emp_id == null || !to_emp_id.Any()))
            {
                foreach (var deptName in to_departement_name)
                {
                    // Find the department ID by name
                    var department = await _context.Department.FirstOrDefaultAsync(d => d.departement_name == deptName);


                    if (department != null)
                    {
                        // Get all active employees in the selected department
                        var departmentEmployees = _context.employee_details
                            .Where(ed => ed.departement_id == department.departement_id && ed.active)
                            .Select(ed => ed.employee_id)
                            .ToList();

                        foreach (var empId in departmentEmployees)
                        {
                            // Create a new letter record for each employee in the department
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
                                to_departement_name = deptName,
                               departement_id = letter.departement_id // Store the specific department name for this record
                            };

                            _context.Add(newLetter);
                        }
                    }
                }
            }

            // If specific employees are chosen, prioritize them over departments
            if (to_emp_id != null && to_emp_id.Any())
            {
                foreach (var empId in to_emp_id.Select(int.Parse))
                {
                    // Find the department(s) for each selected employee
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
                             departement_id = letter.departement_id// Store the department name for this employee
                        };

                        _context.Add(newLetter);
                    }
                }
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
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            var employeeList = new List<Dictionary<string, object>>();

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                string sql = @"SELECT e.employee_id, e.name, e.username, 
                       ed.position, ed.permission_position, ed.departement_id,
                       d.departement_name
                FROM employee e
                JOIN employee_details ed ON e.employee_id = ed.employee_details_id
                LEFT JOIN Department d ON ed.departement_id = d.departement_id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employeeList.Add(new Dictionary<string, object>
                        {
                            ["employee_id"] = reader["employee_id"],
                            ["name"] = reader["name"],
                            ["username"] = reader["username"],
                            ["position"] = reader["position"],
                            ["permission_position"] = reader["permission_position"],
                            ["departement_name"] = reader["departement_name"]
                        });
                    }
                }
            }

            ViewData["EmployeeList"] = employeeList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertEmployee(
    string employee_name,
    string employee_username,
    string employee_password,
    string employee_search_role,
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
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;
                        cmd.CommandText = @"
                    INSERT INTO employee (name, username, password, search_role)
                    VALUES (@Name, @Username, @Password, @SearchRole);
                    
                    INSERT INTO employee_details (
                        employee_details_id, identity_number, departement_id, position, 
                        permission_position, contract_type, national_address, education_level, 
                        hire_date, leave_date, email, phone_number, gender, active
                    )
                    VALUES (
                        SCOPE_IDENTITY(), @IdentityNumber, @DepartmentId, @Position,
                        @PermissionPosition, @ContractType, @NationalAddress, @EducationLevel,
                        @HireDate, @LeaveDate, @Email, @PhoneNumber, @Gender, @Active
                    );";
                        // Set parameters
                        cmd.Parameters.AddWithValue("@Name", employee_name);
                        cmd.Parameters.AddWithValue("@Username", employee_username);
                        cmd.Parameters.AddWithValue("@Password", employee_password);
                        cmd.Parameters.AddWithValue("@SearchRole", employee_search_role);
                        cmd.Parameters.AddWithValue("@IdentityNumber", employee_identity_number);
                        cmd.Parameters.AddWithValue("@DepartmentId", employee_departement_id);
                        cmd.Parameters.AddWithValue("@Position", employee_position);
                        cmd.Parameters.AddWithValue("@PermissionPosition", employee_permission_position);
                        cmd.Parameters.AddWithValue("@ContractType", employee_contract_type);
                        cmd.Parameters.AddWithValue("@NationalAddress", employee_national_address);
                        cmd.Parameters.AddWithValue("@EducationLevel", employee_education_level);
                        cmd.Parameters.AddWithValue("@HireDate", employee_hire_date);
                        cmd.Parameters.AddWithValue("@LeaveDate", employee_leave_date);
                        cmd.Parameters.AddWithValue("@Email", employee_email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", employee_phone_number);
                        cmd.Parameters.AddWithValue("@Gender", employee_gender);
                        cmd.Parameters.AddWithValue("@Active", employee_active);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            transaction.Commit();

                            return Json(new { success = true, message = "تم إنشاء موظف جديد بنجاح!" });

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "لم يتم إنشاء الموظف." });

                        }

                    }

                }

            }

        }





        public IActionResult UpdateEmployee(int id)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();

                // Fetch employee data
                string sql = @"SELECT e.*, ed.* 
                FROM employee e
                JOIN employee_details ed ON e.employee_id = ed.employee_details_id
                WHERE e.employee_id = @Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                Dictionary<string, object> employeeData = new Dictionary<string, object>();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employeeData = new Dictionary<string, object>
                        {
                            ["employee_id"] = reader["employee_id"],
                            ["employee_name"] = reader["name"],
                            ["employee_username"] = reader["username"],
                            ["employee_password"] = reader["password"],
                            ["employee_search_role"] = reader["search_role"],
                            ["employee_identity_number"] = reader["identity_number"],
                            ["employee_departement_id"] = reader["departement_id"],
                            ["employee_position"] = reader["position"],
                            ["employee_permission_position"] = reader["permission_position"],
                            ["employee_contract_type"] = reader["contract_type"],
                            ["employee_national_address"] = reader["national_address"],
                            ["employee_education_level"] = reader["education_level"],
                            ["employee_hire_date"] = ((DateTime)reader["hire_date"]),
                            ["employee_leave_date"] = ((DateTime)reader["leave_date"]),
                            ["employee_email"] = reader["email"],
                            ["employee_phone_number"] = reader["phone_number"],
                            ["employee_gender"] = reader["gender"],
                            ["employee_active"] = reader["active"]
                        };
                    }
                }

                // Fetch departments
                sql = "SELECT departement_id, departement_name FROM Department";
                cmd = new SqlCommand(sql, conn);
                List<Department> departments = new List<Department>();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            departement_id = reader.GetInt32(0),
                            departement_name = reader.GetString(1)
                        });
                    }
                }

                ViewData["EmployeeData"] = employeeData;
                ViewData["Departments"] = departments;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmployee(int employee_id, string employee_name, string employee_username, string employee_password, string employee_search_role, string employee_identity_number, int employee_departement_id, string employee_position, string employee_permission_position, string employee_contract_type, string employee_national_address, string employee_education_level, DateTime employee_hire_date, DateTime employee_leave_date, string employee_email, string employee_phone_number, string employee_gender, bool employee_active)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;
                        cmd.CommandText = @"
                UPDATE employee 
                SET name = @Name, username = @Username, password = @Password, search_role = @SearchRole 
                WHERE employee_id = @EmployeeId;

                UPDATE employee_details 
                SET identity_number = @IdentityNumber, departement_id = @DepartmentId, 
                    position = @Position, permission_position = @PermissionPosition, 
                    contract_type = @ContractType, national_address = @NationalAddress, 
                    education_level = @EducationLevel, hire_date = @HireDate, leave_date = @LeaveDate, 
                    email = @Email, phone_number = @PhoneNumber, gender = @Gender, active = @Active 
                WHERE employee_details_id = @EmployeeId;";

                        // Set parameters for both updates
                        cmd.Parameters.AddWithValue("@EmployeeId", employee_id);
                        cmd.Parameters.AddWithValue("@Name", employee_name);
                        cmd.Parameters.AddWithValue("@Username", employee_username);
                        cmd.Parameters.AddWithValue("@Password", employee_password);
                        cmd.Parameters.AddWithValue("@SearchRole", employee_search_role);
                        cmd.Parameters.AddWithValue("@IdentityNumber", employee_identity_number);
                        cmd.Parameters.AddWithValue("@DepartmentId", employee_departement_id);
                        cmd.Parameters.AddWithValue("@Position", employee_position);
                        cmd.Parameters.AddWithValue("@PermissionPosition", employee_permission_position);
                        cmd.Parameters.AddWithValue("@ContractType", employee_contract_type);
                        cmd.Parameters.AddWithValue("@NationalAddress", employee_national_address);
                        cmd.Parameters.AddWithValue("@EducationLevel", employee_education_level);
                        cmd.Parameters.AddWithValue("@HireDate", employee_hire_date);
                        cmd.Parameters.AddWithValue("@LeaveDate", employee_leave_date);
                        cmd.Parameters.AddWithValue("@Email", employee_email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", employee_phone_number);
                        cmd.Parameters.AddWithValue("@Gender", employee_gender);
                        cmd.Parameters.AddWithValue("@Active", employee_active);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            transaction.Commit();

                            return Json(new { success = true, message = "تم تعديل موظف جديد بنجاح!" });

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "لم يتم تعديل الموظف." });

                        }

                    }

                }


            }

        }



		[HttpGet]
		[Route("ControllerName/GetRemainingHolidayBalance")]
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




	}







}
