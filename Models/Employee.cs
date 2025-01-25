namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class employee
    {
        [Key]
        public int employee_id { get; set; }


        public int employee_number { get; set; } // New property for "الرقم الوظيفي"

        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string? search_role { get; set; }

        public virtual employee_details EmployeeDetails { get; set; }
        public List<salaries_history> SalaryHistory { get; set; }
    }

}
