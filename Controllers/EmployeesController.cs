using CharityProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CharityProject.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Transactions()
        {
            return View(await _context.Transactions.ToListAsync());
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return PartialView("_getAllTransactions", transactions);
        }


        public async Task<IActionResult> GetAllHolidays()
        {
            var holidays = await _context.HolidayHistories.ToListAsync();
            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLetters()
        {
            var letters = await _context.Letters.ToListAsync();
            return PartialView("_getAllLetters", letters);
        }

    }
}
