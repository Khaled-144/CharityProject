using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CharityProject.Controllers
{
   

    public class _employeeProfileController : Controller
    {
        // Show profile view
        public IActionResult _employeeProfile()
        {
            
            
                // Ensure the password is set in the session (for testing purposes)
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("Password")))
                {
                    HttpContext.Session.SetString("Password", "currentPassword123"); // Set an example current password
                }

                return View();
            
        }

        // Handle password change request
        [HttpPost]
        public IActionResult ChangePassword(string current_password, string new_password, string confirm_password)
        {
            // Retrieve the current password stored in the session
            var storedPassword = HttpContext.Session.GetString("Password");

            if (string.IsNullOrEmpty(storedPassword))
            {
                TempData["ErrorMessage"] = "حدث خطأ: كلمة المرور غير موجودة في الجلسة.";
                return RedirectToAction("_employeeProfile");
            }

            // Check if the current password matches the stored password
            if (storedPassword != current_password)
            {
                TempData["ErrorMessage"] = "كلمة المرور الحالية غير صحيحة.";
                return RedirectToAction("_employeeProfile");
            }

            // Check if new password matches confirm password
            if (new_password != confirm_password)
            {
                TempData["ErrorMessage"] = "كلمة المرور الجديدة وتأكيد كلمة المرور غير متطابقتين.";
                return RedirectToAction("_employeeProfile");
            }

            // Update the password in the session (or database)
            HttpContext.Session.SetString("Password", new_password);

            // Show success message
            TempData["SuccessMessage"] = "تم تحديث كلمة المرور بنجاح.";
            return RedirectToAction("_employeeProfile");
        }
    }
}
