using CharityProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharityProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET Actions -------------------------------------------------------- no details needed ( all used thrugh partial view )

        public async Task<IActionResult> Transactions()
        {
            return View(await _context.Transactions.ToListAsync());
        }

        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions
                .OrderByDescending(t => t.transaction_id) // Replace TransactionId with the actual ID column name
                .ToListAsync();
            return PartialView("_getAllTransactions", transactions);
        }

        public async Task<IActionResult> GetAllHolidays()
        {
            var holidays = await _context.HolidayHistories
                .OrderByDescending(h => h.holidays_history_id) // Replace HolidaysHistoryId with the actual ID column name
                .ToListAsync();
            return PartialView("_getAllHolidays", holidays);
        }

        public async Task<IActionResult> GetAllLetters()
        {
            var letters = await _context.Letters
                .OrderByDescending(l => l.letters_id) // Replace LettersId with the actual ID column name
                .ToListAsync();
            return PartialView("_getAllLetters", letters);
        }

        // Create Actions  --------------------------------------------------------


        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Transaction([Bind("create_date,close_date,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.create_date == null)
                {
                    transaction.create_date = DateTime.Now;
                }

                transaction.status = "مرسلة";  // Set default status value
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Transactions));
            }
            return View(transaction);
        }

        public IActionResult Create_Holiday()
        {
            ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Holiday([Bind("title,description,duration,emp_id,start_date,end_date,files,holiday_id")] HolidayHistory holidayHistory)
        {

            holidayHistory.creation_date = DateOnly.FromDateTime(DateTime.Now); // Set to current date
            holidayHistory.status = "مرسلة"; // Set default status
            _context.Add(holidayHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Letter([Bind("title,description,type,from_emp_id,to_emp_id,files")] Letter letter)
        {
            if (ModelState.IsValid)
            {
                letter.date = DateTime.Now; // Set the current date
                letter.departement_id = 3;
                _context.Add(letter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(letter);
        }


        // Update Actions --------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTransactionStatus(int transaction_id)
        {
            var transaction = await _context.Transactions.FindAsync(transaction_id);
            if (transaction == null)
            {
                return NotFound();
            }

            // Update the status to "Closed"
            transaction.status = "منهاة";
            transaction.close_date = DateTime.Now;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Delete Actions --------------------------------------------------------

    }
}
