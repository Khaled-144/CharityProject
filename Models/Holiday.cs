namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }
        public string Type { get; set; }
        public string Duration { get; set; }
        public string DurationUnit { get; set; }
        public int AllowedDuration { get; set; }
    }

}
