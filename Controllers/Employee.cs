using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CharityProject.Controllers
{
    public class Employee : Controller
    {
        private readonly ApplicationDbContext _context;

        public Employee(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateEmployee()
        {
            var departments = _context.Department.ToList();
            ViewData["Departments"] = departments;
            return View();
        }
  /*      public IActionResult EditEmployee()
        {
            return View();
        }
        public IActionResult EmployeeProfile()
        {

            return View();
        }
      */

        public IActionResult EmployeeView()
        {
            var employeeList = _context.employee
                .Include(e => e.EmployeeDetails) // Include employee details
                .ThenInclude(ed => ed.Department) // Include department details from employee details
                .Select(e => new
                {
                    e.employee_id,
                    e.name,
                    e.username,
                    Position = e.EmployeeDetails != null ? e.EmployeeDetails.position : "No Position",
                    PermissionPosition = e.EmployeeDetails != null ? e.EmployeeDetails.permission_position : "No Permission",
                    DepartmentName = e.EmployeeDetails != null && e.EmployeeDetails.Department != null
                        ? e.EmployeeDetails.Department.departement_name
                        : "No Department"
                })
                .ToList();
            var departments = _context.Department.ToList();
            ViewData["Departments"] = departments;
            ViewData["EmployeeList"] = employeeList;
            return View();
        }


      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> InsertEmployee(
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
    var employee = new employee
    {
        name = employee_name,
        username = employee_username,
        password = employee_password,
        search_role = employee_search_role
    };

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        _context.employee.Add(employee);
        await _context.SaveChangesAsync();

        var employeeDetails = new employee_details
        {
            employee_details_id = employee.employee_id, // Ensuring both IDs are the same
            employee_id = employee.employee_id,
            identity_number = int.Parse(employee_identity_number),
            departement_id = int.Parse(employee_departement_id),
            position = employee_position,
            permission_position = employee_permission_position,
            contract_type = employee_contract_type,
            national_address = employee_national_address,
            education_level = employee_education_level,
            hire_date = employee_hire_date,
            leave_date = employee_leave_date,
            email = employee_email,
            phone_number = employee_phone_number,
            gender = employee_gender,
            active = employee_active
        };

        _context.employee_details.Add(employeeDetails);
        await _context.SaveChangesAsync();

        // Check if the position contains 'مدير'
        if (employee_position.Contains("مدير"))
        {
            var department = _context.Department.FirstOrDefault(d => d.departement_id == employeeDetails.departement_id);
            if (department != null)
            {
                var oldSupervisorId = department.supervisor_id;

                // Extract the part of the position after 'مدير'
                var positionSuffix = employee_position.Replace("مدير", "").Trim();

                // Check if the department name matches the position suffix
                if (department.departement_name != positionSuffix)
                {
                    // Return error if department name does not match
                    await transaction.RollbackAsync();
                    return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
                }

                // Check if the old supervisor ID is not the same as the current employee ID
                if (oldSupervisorId != employee.employee_id)
                {
                    // Update the old supervisor's position and permission_position
                    var oldSupervisor = _context.employee.Include(e => e.EmployeeDetails).FirstOrDefault(e => e.employee_id == oldSupervisorId);
                    if (oldSupervisor != null)
                    {
                        var oldSupervisorDetails = oldSupervisor.EmployeeDetails;
                        if (oldSupervisorDetails != null)
                        {
                            oldSupervisorDetails.position = "موظف";
                            oldSupervisorDetails.permission_position = "موظف";
                        }
                    }

                    // Set the new supervisor
                    department.supervisor_id = employee.employee_id;
                }
            }
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Json(new { success = true, message = "تم إنشاء موظف جديد بنجاح!" });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return Json(new { success = false, message = "لم يتم إنشاء الموظف." });
    }
}







        public IActionResult UpdateEmployee(int id)
        {
            var employee = _context.employee
          .Include(e => e.EmployeeDetails)
          .FirstOrDefault(e => e.employee_id == id);

            if (employee == null)
            {
                return NotFound();
            }
            var departments = _context.Department.ToList();
            var employeeData = new
            {
                employee.employee_id,
                employee.name,
                employee.username,
                employee.password,
                employee.search_role,
                employee.EmployeeDetails.identity_number,
                employee.EmployeeDetails.departement_id,
                employee.EmployeeDetails.position,
                employee.EmployeeDetails.permission_position,
                employee.EmployeeDetails.contract_type,
                employee.EmployeeDetails.national_address,
                employee.EmployeeDetails.education_level,
                employee.EmployeeDetails.hire_date,
                employee.EmployeeDetails.leave_date,
                employee.EmployeeDetails.email,
                employee.EmployeeDetails.phone_number,
                employee.EmployeeDetails.gender,
                employee.EmployeeDetails.active
            };

            ViewData["EmployeeData"] = employeeData;
            ViewData["Departments"] = departments;

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmployee(int employee_id, string name, string username, string password, string search_role,
 int identity_number, int departement_id, string position, string permission_position, string contract_type,
 string national_address, string education_level, DateTime hire_date, DateTime leave_date, string email,
 string phone_number, string gender, bool active)
        {
            // Find the existing employee
            var employee = _context.employee
                .Include(e => e.EmployeeDetails)
                .Include(e => e.EmployeeDetails.Department) // Include Department to access supervisor_id
                .FirstOrDefault(e => e.employee_id == employee_id);

            if (employee == null)
            {
                return Json(new { success = false, message = "موظف غير موجود." });
            }

            // Store the previous position to check if the employee was a manager
            var previousPosition = employee.EmployeeDetails.position;

            // Update employee properties
            employee.name = name;
            employee.username = username;
            employee.password = password;
            employee.search_role = search_role;

            // Update employee details
            var details = employee.EmployeeDetails;
            details.identity_number = identity_number;
            details.departement_id = departement_id;
            details.position = position;
            details.permission_position = permission_position;
            details.contract_type = contract_type;
            details.national_address = national_address;
            details.education_level = education_level;
            details.hire_date = hire_date;
            details.leave_date = leave_date;
            details.email = email;
            details.phone_number = phone_number;
            details.gender = gender;
            details.active = active;

            // Initialize the success message
            string successMessage = "تم تعديل موظف بنجاح!";

            // Check if the position contains 'مدير'
            if (position.Contains("مدير"))
            {
                var department = _context.Department.FirstOrDefault(d => d.departement_id == departement_id);
                if (department != null)
                {
                    var oldSupervisorId = department.supervisor_id;

                    // Extract the part of the position after 'مدير'
                    var positionSuffix = position.Replace("مدير", "").Trim();

                    // Check if the department name matches the position suffix
                    if (department.departement_name != positionSuffix)
                    {
                        return Json(new { success = false, message = "اسم القسم والمنصب غير متوافقين" });
                    }

                    // Check if the old supervisor ID is not the same as the current employee ID
                    if (oldSupervisorId != employee_id)
                    {
                        // Update the old supervisor's position and permission_position
                        var oldSupervisor = _context.employee.Include(e => e.EmployeeDetails).FirstOrDefault(e => e.employee_id == oldSupervisorId);
                        if (oldSupervisor != null)
                        {
                            var oldSupervisorDetails = oldSupervisor.EmployeeDetails;
                            if (oldSupervisorDetails != null)
                            {
                                oldSupervisorDetails.position = "موظف";
                                oldSupervisorDetails.permission_position = "موظف";
                            }
                        }

                        // Set the new supervisor
                        department.supervisor_id = employee_id;
                    }
                }
            }

            // If the employee was a manager and is now set to مدير or موظف
            if (previousPosition.Contains("مدير"))
            {
                var oldDepartment = _context.Department.FirstOrDefault(d => d.supervisor_id == employee_id);
                if (oldDepartment != null)
                {
                    oldDepartment.supervisor_id = null;

                    // Append the additional message about the department being vacant
                    successMessage += " القسم السابق الذي كان يديره هذا المدير الآن شاغر بلا مدير.";
                }
            }

            // Save changes to the database and return the JSON response with the final success message
            try
            {
                _context.SaveChanges();
                return Json(new { success = true, message = successMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "لم يتم تعديل الموظف.", error = ex.Message });
            }
        }




    }

}



