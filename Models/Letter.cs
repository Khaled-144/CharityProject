namespace CharityProject.Models
{
	using System;
	using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class letter
	{
		[Key]


		public int letters_id { get; set; }
		public string title { get; set; }
		public string? type { get; set; }
		public string? description { get; set; }
		public DateTime date { get; set; }
		public int from_emp_id { get; set; }

		public string? files { get; set; }
		public int departement_id { get; set; }

        [ForeignKey("from_emp_id")]
        public employee employee { get; set; }

        public string? Confidentiality { get; set; }
		public string? Urgency { get; set; }
		public string? Importance { get; set; }

		public int? to_emp_id { get; set; }
		public string? to_departement_name { get; set; }


	}

}
