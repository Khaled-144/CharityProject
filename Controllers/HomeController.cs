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

        public IActionResult Index()
        {
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

        public IActionResult Privacy()
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
        public IActionResult EmpHomePage()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string id = HttpContext.Request.Cookies["Id"].ToString();
                string name = HttpContext.Request.Cookies["Name"].ToString();
                string position = HttpContext.Request.Cookies["Position"].ToString();
                string departementId = HttpContext.Request.Cookies["DepartementId"].ToString();
                HttpContext.Session.SetString("Id", id);
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Position", position);
                HttpContext.Session.SetString("DeparetementId", departementId);
                if (position == "employee")
                {
                    return RedirectToAction("EmpHomePage", "Home");
                }
                else
                {
                    return RedirectToAction("ManagerHomePage", "Home");
                }
            }
        }

        [HttpPost, ActionName("LoginPage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPage(string userid, string pass, bool rememberMe)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("DefaultConnection");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT e.employee_id, e.name, ed.permission_position, ed.departement_id " +
                  "FROM employee e " +
                  "JOIN employee_details ed ON e.employee_id = ed.employee_details_id " +
                  "WHERE e.employee_id ='" + userid + "' AND e.password ='" + pass + "'";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string id = Convert.ToString((int)reader["employee_id"]);
                string name = (string)reader["name"];
                string position = (string)reader["permission_position"];
                string departementId = Convert.ToString((int)reader["departement_id"]);
                reader.Close();
                conn1.Close();

                HttpContext.Session.SetString("Id", id);
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Position", position);
                HttpContext.Session.SetString("DepartementId", departementId);
                if (rememberMe)
                {
                    HttpContext.Response.Cookies.Append("Id", id);
                    HttpContext.Response.Cookies.Append("Name", name);
                    HttpContext.Response.Cookies.Append("Position", position);
                    HttpContext.Response.Cookies.Append("DepartementId", departementId);
                }

                if (position == "employee")
                {
                    return RedirectToAction("EmpHomePage", "Home");
                }
                else if (position == "manager")
                {
                    return RedirectToAction("ManagerHomePage", "Home");
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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
