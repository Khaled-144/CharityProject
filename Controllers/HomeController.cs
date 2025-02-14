using CharityProject.Data;
using CharityProject.Models;
using CharityProject.Services;  // Ensure this is included for EmailService
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace CharityProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        // Constructor for dependency injection
        public HomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        private int GetEmployeeIdFromSession()
        {
            var employeeIdString = HttpContext.Session.GetString("Id");
            return employeeIdString != null ? int.Parse(employeeIdString) : 0;
        }

        public async Task<IActionResult> Index()
        {
            int currentUserId = GetEmployeeIdFromSession();

            ViewBag.NewTransactionsCount = await GetTransactionCountAsync("مرسلة", currentUserId);
            ViewBag.OngoingTransactionsCount = await GetTransactionCountAsync("منهاة", currentUserId, false);
            ViewBag.CompletedTransactionsCount = await GetTransactionCountAsync("منهاة", currentUserId, true);

            return View();
        }

        private async Task<int> GetTransactionCountAsync(string status, int userId, bool isCompleted = false)
        {
            var query = _context.Transactions.AsQueryable();
            if (isCompleted)
            {
                query = query.Where(t => t.status == status);
            }
            else
            {
                query = query.Where(t => t.status != status);
            }
            return await query
                .Where(t => t.to_emp_id == userId || t.Referrals.Any(r => r.to_employee_id == userId))
                .GroupBy(t => t.transaction_id)
                .Select(g => g.FirstOrDefault())
                .CountAsync();
        }

        public async Task<IActionResult> GetHolidays()
        {
            var holidays = await _context.HolidayHistories.Include(h => h.holiday).ToListAsync();
            return holidays != null ? View(holidays) : Problem("Entity set 'ApplicationDbContext.HolidayHistories' is null.");
        }

        public IActionResult Letters() => View();
        public IActionResult AddEmp() => View();
        public IActionResult Privacy() => View();
        public IActionResult Test() => View();
        public IActionResult Transactions() => View();
        public IActionResult Transaction() => View();
        public IActionResult CreateTransaction() => View();
        public IActionResult Charter() => View();
        public IActionResult WorkingLoginPage() => View();
        public IActionResult EmpHomePage() => View();
        public IActionResult NotAuthorized()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPage(string userid, string pass)
        {
            var encryptedPassword = Encrypt.EncryptPassword(pass);

            var user = await _context.employee
                .Include(e => e.EmployeeDetails)
                    .ThenInclude(ed => ed.Department)
                .FirstOrDefaultAsync(e => e.employee_number.ToString() == userid && e.password == encryptedPassword);

            if (user != null)
            {
                await SetUserSession(user); // Await the async operation
                return RedirectToActionBasedOnPermission(user.EmployeeDetails.permission_position);
            }

            ViewData["Message"] = "Wrong username or password";
            return View();
        }

        // Change to async Task instead of async void
        private async Task SetUserSession(employee user)
        {
            HttpContext.Session.SetString("Id", user.employee_id.ToString());
            HttpContext.Session.SetString("EmployeeNumber", user.employee_number.ToString());
            HttpContext.Session.SetString("Name", user.name);
            HttpContext.Session.SetString("Position", user.EmployeeDetails.position);
            HttpContext.Session.SetString("Permission_position", user.EmployeeDetails.permission_position);
            HttpContext.Session.SetString("DepartmentId", user.EmployeeDetails.departement_id.ToString());
            HttpContext.Session.SetString("DepartmentName", user.EmployeeDetails.Department.departement_name);
        }

        private IActionResult RedirectToActionBasedOnPermission(string permissionPosition)
        {
            return permissionPosition switch
            {
                "موظف" or "السكرتير" => RedirectToAction("Index", "Employees"),
                "مدير الموارد البشرية والمالية" => RedirectToAction("Index", "HR"),
                "مدير التنمية المالية والاستدامة" => RedirectToAction("Index", "FinancialSustainabilityDevelopmentManager"),
                "مدير خدمة المستفيدين" => RedirectToAction("Index", "customerServiceManager"),
                "المدير التنفيذي" => RedirectToAction("Index", "CEO"),
                _ => View("LoginPage"),
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Forgot Password Functionality
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendVerificationCode(string userid, string email)
        {
            var user = _context.employee
                    .Include(e => e.EmployeeDetails)
                    .FirstOrDefault(e => e.employee_number.ToString() == userid && e.EmployeeDetails.email == email);

            if (user != null)
            {
                var verificationCode = new Random().Next(100000, 999999).ToString();
                HttpContext.Session.SetString("VerificationCode", verificationCode);
                HttpContext.Session.SetString("ResetUserId", userid.ToString());

                bool emailSent = false;
                try
                {
                    emailSent = _emailService.SendEmail(email, "رمز التحقق", $"رمز التحقق الخاص بك هو: {verificationCode}");
                }
                catch (Exception)
                {
                    // Error handling without logger
                }

                ViewData["Message"] = emailSent ? "تم إرسال رمز التحقق إلى بريدك الإلكتروني" : "فشل في إرسال البريد الإلكتروني";
                return emailSent ? RedirectToAction("VerifyCode") : View("ForgotPassword");
            }

            ViewData["Message"] = "الرقم الوظيفي او البريد الإلكتروني غير صحيح";
            return View("ForgotPassword");
        }



        public IActionResult VerifyCode()
        {
            ViewData["Message"] = "تم إرسال رمز التحقق";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCode(string verificationCode)
        {
            var storedCode = HttpContext.Session.GetString("VerificationCode");
            var userId = HttpContext.Session.GetString("ResetUserId");

            if (verificationCode == storedCode && !string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("ResetPasswordForm");
            }

            ViewData["Message"] = "رمز التحقق غير صحيح";
            return View();
        }

        public IActionResult ResetPasswordForm() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordForm(string newPassword)
        {
            var userId = HttpContext.Session.GetString("ResetUserId");

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.employee.FirstOrDefaultAsync(e => e.employee_number.ToString() == userId);
                var encryptedPassword = Encrypt.EncryptPassword(newPassword);

                if (user != null)
                {
                    user.password = encryptedPassword;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    ViewData["Message"] = "تم تغيير كلمة المرور بنجاح";
                    return RedirectToAction("LoginPage");
                }
            }

            ViewData["Message"] = "حدث خطأ، حاول مرة أخرى" + userId;
            return View("ResetPasswordForm");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCurrentPassword([FromBody] string currentPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                return BadRequest(new { message = "Password is required", password = currentPassword });
            }

            var userId = HttpContext.Session.GetString("Id");

            var encryptedPassword = Encrypt.EncryptPassword(currentPassword);


            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.employee.FirstOrDefaultAsync(e => e.employee_id.ToString() == userId);

                if (user != null)
                {
                    if (user.password == encryptedPassword)
                    {
                        return Ok(new { message = "كلمة المرور صحيحة" });
                    }
                    else
                    {
                        return Unauthorized(new { message = "كلمة المرور خاطئة" });
                    }
                }
            }

            return BadRequest(new { message = "Error, try again." });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            try
            {
                // Get the user's ID from the session
                var userId = HttpContext.Session.GetString("Id");

                // Find the user in the database
                var user = await _context.employee.FirstOrDefaultAsync(e => e.employee_id.ToString() == userId);

                if (user == null)
                {
                    return RedirectToAction("LoginPage");  // Redirect if user is not found
                }

                var encryptedPassword = Encrypt.EncryptPassword(newPassword);

                user.password = encryptedPassword;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Clear the session to log out the user
                HttpContext.Session.Clear();

                // Return a success response with a message to be displayed
                return Ok(new { success = true, message = "تم تغيير كلمة المرور بنجاح", redirectToLogin = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء تغيير كلمة المرور", error = ex.Message });
            }
        }



    }
}
