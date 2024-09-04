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

    

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> LoginPage(string userid, string pass, bool rememberMe)
            {
                var user = await _context.employee
                    .Include(e => e.EmployeeDetails)
                        .ThenInclude(ed => ed.Department)
                    .FirstOrDefaultAsync(e => e.employee_id.ToString() == userid && e.password == pass);

                if (user != null)
                {
                    string id = user.employee_id.ToString();
                    string name = user.name;
                    string permission_position = user.EmployeeDetails.permission_position;
                    string position = user.EmployeeDetails.position;
                    string departmentId = user.EmployeeDetails.departement_id.ToString();
                    string departmentName = user.EmployeeDetails.Department.departement_name;

                    HttpContext.Session.SetString("Id", id);
                    HttpContext.Session.SetString("Name", name);
                    HttpContext.Session.SetString("Position", position);
                    HttpContext.Session.SetString("Permission_position", permission_position);
                    HttpContext.Session.SetString("DepartmentId", departmentId);
                    HttpContext.Session.SetString("DepartmentName", departmentName);

                    if (rememberMe)
                    {
                        Response.Cookies.Append("Id", id);
                        Response.Cookies.Append("pass", user.password);
                    }

                   else if (permission_position == "„ÊŸ›"|| permission_position =="”ﬂ—Ì — «·„œÌ— «· ‰›Ì–Ì")
                    {
                        return RedirectToAction("Index", "Employees");
                    }

               else if (permission_position == "„œÌ— «·„Ê«—œ «·»‘—Ì… Ê«·„«·Ì…")
                {
                    return RedirectToAction("Index", "HR");
                }

                else if (permission_position == "„œÌ— «· ‰„Ì… «·„«·Ì… Ê«·«” œ«„…")
                {
                    return RedirectToAction("Index", "FinancialSustainabilityDevelopmentManager");
                }
                else if (permission_position == "„œÌ— Œœ„… «·„” ›ÌœÌ‰")
                {
                    return RedirectToAction("Index", "customerServiceManager");
                }
                else if (permission_position == "«·„œÌ— «· ‰›Ì–Ì")
                {
                    return RedirectToAction("Index", "CEO");
                }

                else
                    {
                        ViewData["Message"] = "Unknown position";
                        return View();
                    }
                }
                else
                    ViewData["Message"] = "Wrong username or password";
                    return View();
                
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
