namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HolidayHistory
    {
        [Key]
        public int HolidayHistoryId { get; set; }
        public int HolidayId { get; set; }
        public int Duration { get; set; }
        public int EmpId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        [ForeignKey("HolidayId")]
        public Holiday Holiday { get; set; }
    }

}
