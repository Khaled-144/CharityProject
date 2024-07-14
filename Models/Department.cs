namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int SupervisorId { get; set; }
    }

}
