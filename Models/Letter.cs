namespace CharityProject.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Letter
    {
        [Key]
        public int letters_id { get; set; }
        public string title { get; set; }
        public string? type { get; set; }
        public string? description { get; set; }
        public DateTime date { get; set; }
        public int from_emp_id { get; set; }
        public int? to_emp_id { get; set; }
        public string? files { get; set; }
        public int departement_id { get; set; }

        public string? Confidentiality { get; set; }
        public string? Urgency { get; set; }
        public string? Importance { get; set; }
    }

}
