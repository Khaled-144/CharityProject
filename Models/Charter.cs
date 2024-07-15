namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Charter
    {
        [Key]
        public int CharterId { get; set; }
        public int ToEmpId { get; set; }
        public string Devices { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
