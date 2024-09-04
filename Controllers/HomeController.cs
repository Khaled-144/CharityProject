using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CharityProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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



        public async Task<IActionResult> getHolidays()
        {
            return _context.HolidayHistories != null ?
                    View(await _context.HolidayHistories.Include(h => h.holiday).ToListAsync()) :
                    Problem("Entity set 'ApplicationDbContext.HolidayHistories' is null.");
        }

      

        public IActionResult Letters()
        {
            return View();
        }
        public IActionResult addEmp()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult test()
        {
            return View();
        }
        public IActionResult transactions()
        {
            return View();
        }
        public IActionResult transaction()
        {
            return View();
        }
        public IActionResult CreateTransaction()
        {
            return View();
        }
        public IActionResult Charter()
        {

            return View();
        }
        public IActionResult WorkingLoginPage()
        {
            return View();
        }
        public IActionResult EmpHomePage()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Id"))
                return View();
            else
            {
                string id = HttpContext.Request.Cookies["Id"];
                string pass = HttpContext.Request.Cookies["pass"];
                ViewData["UserId"] = id;
                ViewData["Password"] = pass;

                return View();
                
            }
        }

        [HttpPost, ActionName("LoginPage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPage(string userid, string pass, bool rememberMe)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                string sql = @"SELECT e.employee_id,e.password, e.name, ed.permission_position, ed.departement_id, d.departement_name
                           FROM employee e 
                           JOIN employee_details ed ON e.employee_id = ed.employee_details_id 
                           JOIN Department d ON ed.departement_id = d.departement_id
                           WHERE e.employee_id = @UserId AND e.password = @Password";

                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@UserId", userid);
                    comm.Parameters.AddWithValue("@Password", pass);

                    conn.Open();
                    using (SqlDataReader reader = await comm.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string id = reader["employee_id"].ToString();
                            string name = reader["name"].ToString();
                            string position = reader["permission_position"].ToString();
                            string departmentId = reader["departement_id"].ToString();
                            string departmentName = reader["departement_name"].ToString();
                            string password = reader["password"].ToString();
                            
                            HttpContext.Session.SetString("Id", id);
                            HttpContext.Session.SetString("Name", name);
                            HttpContext.Session.SetString("Position", position);
                            HttpContext.Session.SetString("DepartmentId", departmentId);
                            HttpContext.Session.SetString("DepartmentName", departmentName);
                            if (rememberMe)
                            {
                                HttpContext.Response.Cookies.Append("Id", id);
                                HttpContext.Response.Cookies.Append("pass", password);
                            }

                                if (position == "ãæÙÝ")
                            {
                                return RedirectToAction("Index", "Employees");
                            }
                            else if (position != "ãæÙÝ" )
                            {
                                return RedirectToAction("Index", "Employees");
                            }
                            
                            else
                            {
                                ViewData["Message"] = "Unknown position";
                                return View();
                            }
                        }
                        else
                        {
                            ViewData["Message"] = "Wrong username or password";
                            return View();
                        }
                    }
                }
            }
        }

      
        public IActionResult Logout()
        {
           
            HttpContext.Session.Clear();
           
            return RedirectToAction("LoginPage");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
