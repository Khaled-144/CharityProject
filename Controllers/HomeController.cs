using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
/*using SendGrid;
using SendGrid.Helpers.Mail;*/
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;




namespace CharityProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
             /// <summary>
             /// /////////////
  /*           /// </summary>
        private readonly EmailService _emailService;
      

     

        public HomeController(EmailService emailService)
        {
            _emailService = emailService;
        }*/

        /// <summary>
        /// ////////////////
        /// </summary>
       

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

                else if (permission_position == "موظف" || permission_position == "السكرتير")
                {
                    return RedirectToAction("Index", "Employees");
                }

                else if (permission_position == "مدير الموارد البشرية والمالية")
                {
                    return RedirectToAction("Index", "HR");
                }

                else if (permission_position == "مدير التنمية المالية والاستدامة")
                {
                    return RedirectToAction("Index", "FinancialSustainabilityDevelopmentManager");
                }
                else if (permission_position == "مدير خدمة المستفيدين")
                {
                    return RedirectToAction("Index", "customerServiceManager");
                }
                else if (permission_position == "المدير التنفيذي")
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






        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



              


      /*  public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userid, string newPassword)
        {
            // Fetch the user by their employee ID
            var user = await _context.employee.FirstOrDefaultAsync(e => e.employee_id.ToString() == userid);

            if (user != null)
            {
                // Update the user's password
                user.password = newPassword;
                _context.Update(user);
                await _context.SaveChangesAsync();

                ViewData["Message"] = "تم تغيير كلمة المرور بنجاح";
                return RedirectToAction("LoginPage");
            }

            ViewData["Message"] = "المستخدم غير موجود";
            return View("ForgotPassword");
        }


       [HttpPost]
        public IActionResult SendVerificationCode(string userEmail)
        {
         *//*   bool emailSent = _emailService.SendEmail(userEmail, "Subject Here", "Body Here");*//*

            if (emailSent)
            {
                ViewData["Message"] = "Verification code sent to your email.";
            }
            else
            {
                ViewData["Message"] = "Failed to send email.";
            }

            return View();
        }
        // View to input the verification code
        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(string verificationCode)
        {
            // Check if the code matches the one in the session
            var storedCode = HttpContext.Session.GetString("VerificationCode");
            var userId = HttpContext.Session.GetString("ResetUserId");

            if (verificationCode == storedCode && !string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("ResetPasswordForm");
            }

            ViewData["Message"] = "رمز التحقق غير صحيح";
            return View();
        }

        public IActionResult ResetPasswordForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword)
        {
            var userId = HttpContext.Session.GetString("ResetUserId");

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.employee.FirstOrDefaultAsync(e => e.employee_id.ToString() == userId);

                if (user != null)
                {
                    user.password = newPassword;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    ViewData["Message"] = "تم تغيير كلمة المرور بنجاح";
                    return RedirectToAction("LoginPage");
                }
            }

            ViewData["Message"] = "حدث خطأ، حاول مرة أخرى";
            return View("ForgotPassword");
        }
*/


   /*     public class EmailService
        {
            private readonly SmtpSettings _smtpSettings;

            // Constructor that uses IOptions<SmtpSettings>
            public EmailService(IOptions<SmtpSettings> smtpSettings)
            {
                _smtpSettings = smtpSettings.Value;  // Get actual settings from IOptions
            }

            public bool SendEmail(string toEmail, string subject, string body)
            {
                try
                {
                    // Use the settings to configure your SMTP client
                    var smtpClient = new SmtpClient(_smtpSettings.Server)
                    {
                        Port = _smtpSettings.Port,
                        Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                        EnableSsl = _smtpSettings.EnableSsl
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.Username), // Sender's email
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    smtpClient.Send(mailMessage);  // Send the email
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    return false;
                }
            }
        }*/

















    }
}
