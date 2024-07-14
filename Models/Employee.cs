namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? SearchRole { get; set; }
    }

}
