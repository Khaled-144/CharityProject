namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class employee
    {
        [Key]
        public int employee_id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string? search_role { get; set; }
    }

}
