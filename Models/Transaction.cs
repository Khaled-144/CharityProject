namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Files { get; set; }
        public int FromEmpId { get; set; }
        public int? ToEmpId { get; set; }
        public int DepartmentId { get; set; }
    }

}
