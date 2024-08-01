using CharityProject.Data;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CharityProject.Controllers
{
	public class HRController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HRController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult ManageSalaries(int selectedMonth, int selectedYear)
		{
			// If no month/year is selected, default to the current month and year
			if (selectedMonth == 0) selectedMonth = DateTime.Now.Month;
			if (selectedYear == 0) selectedYear = DateTime.Now.Year;

			// Get all employees
			var employees = _context.employee.ToList();

			// Get salaries for the selected month/year
			var salaries = _context.SalaryHistories
				.Where(s => s.date.Month == selectedMonth && s.date.Year == selectedYear)
				.ToList();

			// Create the ViewModel
			var viewModel = new SalariesViewModel
			{
				Employees = employees,
				Salaries = salaries,
				SelectedMonth = selectedMonth,
				SelectedYear = selectedYear
			};

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult SaveSalaries(SalariesViewModel viewModel)
		{
			// Process the data and save to the database
			// Implement the logic to save the salaries

			// Redirect or return a view after saving
			return RedirectToAction("ManageSalaries");
		}


	}
}
