using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CharityProject.Data;
using CharityProject.Models;

namespace CharityProject.Controllers
{
	public class TransactionsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public TransactionsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Transactions
		public async Task<IActionResult> Index()
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



		public async Task<IActionResult> SearchById(int id = 0, string sortOrder = "")
		{
			var transactions = from t in _context.Transactions
							   select t;

			if (id != 0)
			{
				transactions = transactions.Where(n => n.transaction_id == id);
			}

			switch (sortOrder)
			{
				case "oldest":
					transactions = transactions.OrderBy(t => t.create_date);
					break;
				case "newest":
					transactions = transactions.OrderByDescending(t => t.create_date);
					break;
				default:
					break;
			}

			var transactionList = await transactions.ToListAsync();

			if (!transactionList.Any())
			{
				// Return a partial view with a message indicating no results found
				return PartialView("_NoResults");
			}

			// Return the partial view with the transaction list
			return PartialView("_getAllTransactions", transactionList);
		}



		public IActionResult CreateNewTransaction()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNewTransaction([Bind("close_date,status,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
		{
			if (ModelState.IsValid)
			{
				transaction.create_date = DateTime.Now;
				_context.Add(transaction);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(transaction);
		}

		// GET: Transactions/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions
				.FirstOrDefaultAsync(m => m.transaction_id == id);
			if (transaction == null)
			{
				return NotFound();
			}

			return View(transaction);
		}

		// GET: Transactions/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Transactions/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("create_date,close_date,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
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
				return RedirectToAction(nameof(Index));
			}
			return View(transaction);
		}


		// GET: Transactions/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction == null)
			{
				return NotFound();
			}
			return View(transaction);
		}

		// POST: Transactions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("transaction_id,type,create_date,close_date,status,title,description,files,from_emp_id,to_emp_id,department_id")] Transaction transaction)
		{
			if (id != transaction.transaction_id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(transaction);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TransactionExists(transaction.transaction_id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(transaction);
		}

		// GET: Transactions/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var transaction = await _context.Transactions
				.FirstOrDefaultAsync(m => m.transaction_id == id);
			if (transaction == null)
			{
				return NotFound();
			}

			return View(transaction);
		}

		// POST: Transactions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var transaction = await _context.Transactions.FindAsync(id);
			if (transaction != null)
			{
				_context.Transactions.Remove(transaction);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool TransactionExists(int id)
		{
			return _context.Transactions.Any(e => e.transaction_id == id);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(int transaction_id)
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


		public IActionResult Create_Holiday()
		{
			ViewData["holiday_id"] = new SelectList(_context.Holidays, "holiday_id", "holiday_id");
			return View();
		}

		// POST: HolidayHistories/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create_Holiday([Bind("title,description,duration,emp_id,start_date,end_date,files,holiday_id")] HolidayHistory holidayHistory)
		{

			holidayHistory.creation_date = DateOnly.FromDateTime(DateTime.Now); // Set to current date
			holidayHistory.status = "تم الإرسال"; // Set default status
			_context.Add(holidayHistory);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		}
	}
}
