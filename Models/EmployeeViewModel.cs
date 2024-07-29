using System.ComponentModel.DataAnnotations;

namespace CharityProject.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Position { get; set; }
        public string PermissionPosition { get; set; }

    }
}
