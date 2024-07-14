namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Letter
    {
        [Key]
        public int LetterId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public int FromEmpId { get; set; }
        public int? ToEmpId { get; set; }
        public string? Files { get; set; }
        public int DepartmentId { get; set; }
    }

}
