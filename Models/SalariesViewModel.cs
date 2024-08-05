namespace CharityProject.Models
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class SalariesViewModel
	{
		public List<employee> Employees { get; set; }
		public List<salaries_history> Salaries { get; set; }
		public int SelectedMonth { get; set; }
		public int SelectedYear { get; set; }
	}
}

