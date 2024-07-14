namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmployeeDetails
    {
        [Key]
        public int EmployeeDetailsId { get; set; }
        public int IdentityNumber { get; set; }
        public int DepartmentId { get; set; }
        public string Position { get; set; }
        public string PermissionPosition { get; set; }
        public string ContractType { get; set; }
        public string? NationalAddress { get; set; }
        public string? EducationLevel { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool Active { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string? Files { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }

}
