namespace CharityProject.Models
{
    using CharityProject.Controllers;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("holidays_history")]
    public class HolidayHistory
    {
        [Key]
        public int holidays_history_id { get; set; }
        public int holiday_id { get; set; }  // This should match the foreign key in the database
        public string title { get; set; }
        public string? description { get; set; }
        public int duration { get; set; }
        public int emp_id { get; set; }
        [ForeignKey("emp_id")]
        public employee_details Employee_detail { get; set; }

        public DateOnly creation_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string? files { get; set; }
        public string? status { get; set; }

        [ForeignKey("holiday_id")]
        public virtual Holiday holiday { get; set; }

    }

}
