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
            var referrals = await _context.Referrals
                .Where(r => r.transaction_id == id)
                .OrderByDescending(r => r.referral_date)
                .ToListAsync();

            return View(referrals);
        }

        // End of khaled work -----------------------------------------------------







        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

        public async Task<IActionResult> Transactions()
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


            return View(await _context.Transactions.ToListAsync());
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Referrals)
                .OrderByDescending(t => t.transaction_id) // Order by transaction_id in descending order
                .ToListAsync();
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
        public async Task<IActionResult> Create_Transaction([Bind("create_date,close_date,title,description,files,from_emp_id,to_emp_id,department_id,Confidentiality,Urgency,Importance")] Transaction transaction)
        {

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

            return RedirectToAction(nameof(Index));
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






    }







}
