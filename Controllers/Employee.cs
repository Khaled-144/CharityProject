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

