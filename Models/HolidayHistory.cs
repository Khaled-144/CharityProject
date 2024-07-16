namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("holidays_history")]
    public class HolidayHistory
    {
        [Key]
        public int holidays_history_id { get; set; }
        public int holiday_id { get; set; }  // This should match the foreign key in the database
        public int duration { get; set; }
        public int emp_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string status { get; set; }

        [ForeignKey("holiday_id")]
        public Holiday holiday { get; set; }
    }
}
