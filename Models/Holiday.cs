namespace CharityProject.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Holiday
    {
        [Key]
        public int holiday_id { get; set; }
        public string type { get; set; }
        public string duration { get; set; }
        public string duration_Unit { get; set; }
        public int allowedDuration { get; set; }
    }

}
